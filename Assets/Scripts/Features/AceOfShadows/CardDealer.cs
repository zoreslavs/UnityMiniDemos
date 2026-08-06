using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.AceOfShadows
{
    public sealed class CardDealer : MonoBehaviour
    {
        private const float AnimPeakTime = 0.5f;
        private const string CompletionMessage = "All cards moved!";

        [SerializeField] private CardView _cardPrefab;
        [SerializeField] private DeckView _leftDeck;
        [SerializeField] private DeckView _rightDeck;
        [SerializeField] private RectTransform _leftStackRoot;
        [SerializeField] private RectTransform _rightStackRoot;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private Sprite[] _suitSprites;
        [SerializeField, Min(0.01f)] private float _moveDuration = 0.5f;
        [SerializeField, Min(0.01f)] private float _cardInterval = 1f;
        [SerializeField, Min(0f)] private float _jumpHeight = 80f;
        [SerializeField, Min(1f)] private float _peakScale = 1.15f;
        [SerializeField] private Vector2 _stackOffset = new Vector2(0f, -1f);
        [SerializeField] private AnimationCurve _animCurve = new AnimationCurve
        (
            new Keyframe(0f, 0f),
            new Keyframe(AnimPeakTime, 1f),
            new Keyframe(1f, 0f)
        );

        private Transform _animationRoot;
        private Coroutine _dealRoutine;
        private float _speedMultiplier = 1f;

        public void SetSpeedMultiplier(float speedMultiplier)
        {
            _speedMultiplier = Mathf.Max(0.01f, speedMultiplier);
        }

        private void Start()
        {
            _dealRoutine = StartCoroutine(DealCards());
        }

        private void OnDisable()
        {
            if (_dealRoutine != null)
            {
                StopCoroutine(_dealRoutine);
                _dealRoutine = null;
            }
        }

        private IEnumerator DealCards()
        {
            if (!HasValidSetup())
                yield break;

            var deck = CardDeck.Create();
            CardDeck.Shuffle(deck);
            var cards = CreateCards(deck);
            InitializeView();

            for (var index = 0; index < cards.Length; index++)
            {
                if (index > 0)
                    yield return WaitBetweenCards();

                var card = cards[index];
                LiftFromLeftStack(card);
                yield return AnimateCard(card);
                DropOnRightStack(card);
                UpdateDeckCounters(index + 1);
            }

            ShowCompletionMessage();
            _dealRoutine = null;
        }

        private CardView[] CreateCards(IReadOnlyList<CardData> deck)
        {
            var cards = new CardView[deck.Count];
            var deckPosition = _leftDeck.RectTransform.position;
            var stackOffset = GetStackOffset();

            _animationRoot = _leftStackRoot.parent;

            for (var index = deck.Count - 1; index >= 0; index--)
            {
                var card = Instantiate(_cardPrefab, _leftStackRoot);
                var cardTransform = card.RectTransform;
                cardTransform.position = deckPosition + stackOffset * index;
                cardTransform.localRotation = Quaternion.identity;
                card.Configure(deck[index], _suitSprites[(int)deck[index].Suit]);
                cards[index] = card;
            }

            return cards;
        }

        private void LiftFromLeftStack(CardView card)
        {
            var cardTransform = card.RectTransform;
            cardTransform.SetParent(_animationRoot, true);
            cardTransform.SetAsLastSibling();
        }

        private void DropOnRightStack(CardView card)
        {
            var stackOffset = GetStackOffset();
            _rightStackRoot.position += stackOffset;
            _leftStackRoot.position -= stackOffset;

            var cardTransform = card.RectTransform;
            cardTransform.SetParent(_rightStackRoot, true);
            cardTransform.SetAsLastSibling();
        }

        private IEnumerator AnimateCard(CardView card)
        {
            var cardTransform = card.RectTransform;
            var startPosition = cardTransform.position;
            var endPosition = _rightDeck.RectTransform.position;

            var baseScale = cardTransform.localScale;
            var peakScale = baseScale * _peakScale;
            var progress = 0f;

            while (progress < 1f)
            {
                progress = Mathf.Clamp01(progress + Time.deltaTime * _speedMultiplier / _moveDuration);
                var easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                var horizontalPosition = Vector3.Lerp(startPosition, endPosition, easedProgress);
                var animHeight = Mathf.Max(0f, _animCurve.Evaluate(easedProgress)) * _jumpHeight;
                var scaleProgress = Mathf.Sin(easedProgress * Mathf.PI);

                cardTransform.position = horizontalPosition + Vector3.up * animHeight;
                cardTransform.localScale = Vector3.Lerp(baseScale, peakScale, scaleProgress);

                yield return null;
            }

            cardTransform.position = endPosition;
            cardTransform.localScale = baseScale;
        }

        private IEnumerator WaitBetweenCards()
        {
            var elapsed = 0f;

            while (elapsed < GetRemainingWaitDuration())
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private void InitializeView()
        {
            _leftDeck.SetCount(CardDeck.Count);
            _rightDeck.SetCount(0);
            _resultText.gameObject.SetActive(false);
        }

        private void UpdateDeckCounters(int movedCardCount)
        {
            _leftDeck.SetCount(CardDeck.Count - movedCardCount);
            _rightDeck.SetCount(movedCardCount);
        }

        private void ShowCompletionMessage()
        {
            _resultText.SetText(CompletionMessage);
            _resultText.gameObject.SetActive(true);
        }

        private float GetRemainingWaitDuration()
        {
            return Mathf.Max(0f, _cardInterval - _moveDuration) / _speedMultiplier;
        }

        private Vector3 GetStackOffset()
        {
            return new Vector3(_stackOffset.x, _stackOffset.y, 0f);
        }

        private bool HasValidSetup()
        {
            if (_cardPrefab == null || _leftDeck == null || _rightDeck == null || _resultText == null)
            {
                Debug.LogError("CardDealer requires a card prefab, two decks and a result text.", this);
                return false;
            }

            if (!_leftDeck.IsConfigured || !_rightDeck.IsConfigured)
            {
                Debug.LogError("CardDealer requires a counter on both decks.", this);
                return false;
            }

            if (_leftStackRoot == null || _rightStackRoot == null || _leftStackRoot.parent == null)
            {
                Debug.LogError("CardDealer requires a left and a right stack root with a shared parent.", this);
                return false;
            }

            if (_suitSprites == null || _suitSprites.Length != CardDeck.SuitCount)
            {
                Debug.LogError("CardDealer requires four suit sprites in enum order: Clubs, Spades, Hearts, Diamonds.", this);
                return false;
            }

            for (var index = 0; index < _suitSprites.Length; index++)
            {
                if (_suitSprites[index] == null)
                {
                    Debug.LogError($"CardDealer is missing suit sprite at index {index}.", this);
                    return false;
                }
            }

            return _moveDuration > 0f && _cardInterval > 0f && _animCurve != null;
        }
    }
}