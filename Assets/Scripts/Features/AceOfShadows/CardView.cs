using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.AceOfShadows
{
    public sealed class CardView : MonoBehaviour
    {
        private const float LowerRightSuitRotation = 180f;

        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private Image _topLeftSuit;
        [SerializeField] private Image _bottomRightSuit;
        [SerializeField] private Color _redSuitColor = new Color32(255, 23, 34, 255);
        [SerializeField] private Color _blackSuitColor = Color.black;

        public RectTransform RectTransform => (RectTransform)transform;

        public void Configure(CardData cardData, Sprite suitSprite)
        {
            if (_numberText == null || _topLeftSuit == null || _bottomRightSuit == null)
            {
                Debug.LogError("CardView references are not configured.", this);
                return;
            }

            if (suitSprite == null)
            {
                Debug.LogError("CardView received a null suit sprite.", this);
                return;
            }

            _numberText.SetText(cardData.Number.ToString());
            _numberText.color = IsRedSuit(cardData.Suit) ? _redSuitColor : _blackSuitColor;
            _topLeftSuit.sprite = suitSprite;
            _bottomRightSuit.sprite = suitSprite;
            _bottomRightSuit.rectTransform.localRotation = Quaternion.Euler(0f, 0f, LowerRightSuitRotation);
        }

        private static bool IsRedSuit(CardSuit suit)
        {
            return suit == CardSuit.Hearts || suit == CardSuit.Diamonds;
        }
    }
}