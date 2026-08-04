using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.AceOfShadows
{
    public sealed class AnimationController : MonoBehaviour
    {
        private const int SuitCount = 4;
        private const int CardsPerSuit = 36;
        private const int DeckSize = CardsPerSuit * SuitCount;
        private const float AnimPeakTime = 0.5f;
        private const string CompletionMessage = "All cards moved!";

        [SerializeField] private CardView _cardPrefab;
        [SerializeField] private DeckView _leftDeck;
        [SerializeField] private DeckView _rightDeck;
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

            var deck = CreateDeck();
            ShuffleDeck(deck);
            var cards = CreateCards(deck);
            InitializeView();

            for (var index = 0; index < cards.Length; index++)
            {
                if (index > 0)
                    yield return WaitBetweenCards();

                yield return AnimateCard(cards[index]);
                ArrangeLeftStack(cards, index + 1);
                ArrangeRightStack(cards, index + 1);
                UpdateDeckCounters(index + 1);
            }

            ShowCompletionMessage();
            _dealRoutine = null;
        }

        private CardView[] CreateCards(IReadOnlyList<CardData> deck)
        {
            var cards = new CardView[deck.Count];
            var parent = _leftDeck.transform.parent;
            var deckPosition = _leftDeck.RectTransform.position;
            var stackOffset = GetStackOffset();

            for (var index = deck.Count - 1; index >= 0; index--)
            {
                var card = Instantiate(_cardPrefab, parent);
                var cardTransform = card.RectTransform;
                cardTransform.position = deckPosition + stackOffset * index;
                cardTransform.localRotation = Quaternion.identity;
                card.Configure(deck[index], _suitSprites[(int)deck[index].Suit]);
                cards[index] = card;
            }

            return cards;
        }

        private IEnumerator AnimateCard(CardView card)
        {
            var cardTransform = card.RectTransform;
            cardTransform.SetAsLastSibling();

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
            cardTransform.SetAsLastSibling();
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

        private bool HasValidSetup()
        {
            if (_cardPrefab == null || _leftDeck == null || _rightDeck == null || _resultText == null)
            {
                Debug.LogError("AnimationController requires a card prefab, two decks and a result text.", this);
                return false;
            }

            if (!_leftDeck.IsConfigured || !_rightDeck.IsConfigured)
            {
                Debug.LogError("AnimationController requires a counter on both decks.", this);
                return false;
            }

            if (_suitSprites == null || _suitSprites.Length != SuitCount)
            {
                Debug.LogError("AnimationController requires four suit sprites in enum order: Clubs, Spades, Hearts, Diamonds.", this);
                return false;
            }

            for (var index = 0; index < _suitSprites.Length; index++)
            {
                if (_suitSprites[index] == null)
                {
                    Debug.LogError($"AnimationController is missing suit sprite at index {index}.", this);
                    return false;
                }
            }

            return _moveDuration > 0f && _cardInterval > 0f && _animCurve != null;
        }

        private void InitializeView()
        {
            _leftDeck.SetCount(DeckSize);
            _rightDeck.SetCount(0);
            _resultText.gameObject.SetActive(false);
        }

        private void UpdateDeckCounters(int movedCardCount)
        {
            _leftDeck.SetCount(DeckSize - movedCardCount);
            _rightDeck.SetCount(movedCardCount);
        }

        private void ShowCompletionMessage()
        {
            _resultText.SetText(CompletionMessage);
            _resultText.gameObject.SetActive(true);
        }

        private void ArrangeLeftStack(IReadOnlyList<CardView> cards, int firstRemainingIndex)
        {
            var deckPosition = _leftDeck.RectTransform.position;
            var stackOffset = GetStackOffset();

            for (var index = firstRemainingIndex; index < cards.Count; index++)
            {
                var stackPosition = index - firstRemainingIndex;
                cards[index].RectTransform.position = deckPosition + stackOffset * stackPosition;
            }
        }

        private void ArrangeRightStack(IReadOnlyList<CardView> cards, int movedCardCount)
        {
            var deckPosition = _rightDeck.RectTransform.position;
            var stackOffset = GetStackOffset();

            for (var index = 0; index < movedCardCount; index++)
            {
                var stackPosition = movedCardCount - 1 - index;
                cards[index].RectTransform.position = deckPosition + stackOffset * stackPosition;
            }
        }

        private static List<CardData> CreateDeck()
        {
            var deck = new List<CardData>(DeckSize);

            for (var suitIndex = 0; suitIndex < SuitCount; suitIndex++)
            {
                for (var number = 1; number <= CardsPerSuit; number++)
                {
                    deck.Add(new CardData(number, (CardSuit)suitIndex));
                }
            }

            return deck;
        }

        private static void ShuffleDeck(IList<CardData> deck)
        {
            for (var index = deck.Count - 1; index > 0; index--)
            {
                var swapIndex = Random.Range(0, index + 1);
                (deck[index], deck[swapIndex]) = (deck[swapIndex], deck[index]);
            }
        }

        private float GetRemainingWaitDuration()
        {
            return Mathf.Max(0f, _cardInterval - _moveDuration) / _speedMultiplier;
        }

        private Vector3 GetStackOffset()
        {
            return new Vector3(_stackOffset.x, _stackOffset.y, 0f);
        }
    }
}