using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class DialogueItemView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _message;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _bubble;
        [SerializeField] private RectTransform _user;
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private Image _bubbleImage;
        [SerializeField] private Image _tailImage;
        [SerializeField] private RawImage _avatarImage;
        [SerializeField] private GameObject _emptyAvatar;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private int _horizontalPadding = 24;
        [SerializeField] private float _minBubbleWidth = 280f;
        [SerializeField] private float _maxBubbleWidth = 500f;
        [SerializeField] private float _minBubbleHeight = 120f;
        [SerializeField] private float _bubbleHorizontalPadding = 48f;
        [SerializeField] private float _bubbleVerticalPadding = 40f;
        [SerializeField] private float _minItemHeight = 300f;
        [SerializeField] private float _itemVerticalPadding = 40f;

        public void Configure(string name, string message, Texture2D avatar, bool alignLeft, Color bubbleColor)
        {
            if (!HasValidSetup())
                return;

            _nameText.SetText(name ?? "Unknown");
            _messageText.SetText(message ?? string.Empty);
            _avatarImage.texture = avatar;
            _avatarImage.enabled = avatar != null;
            _emptyAvatar.SetActive(avatar == null);
            SetAlignment(alignLeft, bubbleColor);
            ResizeMessage();
        }

        private void SetAlignment(bool alignLeft, Color bubbleColor)
        {
            if (alignLeft)
            {
                _user.SetAsFirstSibling();
                _message.SetAsLastSibling();
            }
            else
            {
                _message.SetAsFirstSibling();
                _user.SetAsLastSibling();
            }

            _layoutGroup.childAlignment = alignLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
            _layoutGroup.padding.left = alignLeft ? _horizontalPadding : 0;
            _layoutGroup.padding.right = alignLeft ? 0 : _horizontalPadding;

            _bubbleImage.color = bubbleColor;
            _tailImage.color = bubbleColor;
            _bubble.localScale = alignLeft ? new Vector3(-1f, 1f, 1f) : Vector3.one;
        }

        private void ResizeMessage()
        {
            _messageText.enableAutoSizing = false;
            _messageText.margin = Vector4.zero;

            var maxTextWidth = _maxBubbleWidth - _bubbleHorizontalPadding;
            var preferredWidth = _messageText.GetPreferredValues(_messageText.text, maxTextWidth, 0f).x;
            var bubbleWidth = Mathf.Clamp(preferredWidth + _bubbleHorizontalPadding, _minBubbleWidth, _maxBubbleWidth);
            var textWidth = bubbleWidth - _bubbleHorizontalPadding;
            var preferredHeight = _messageText.GetPreferredValues(_messageText.text, textWidth, 0f).y;
            var bubbleHeight = Mathf.Max(_minBubbleHeight, preferredHeight + _bubbleVerticalPadding);

            _message.sizeDelta = new Vector2(bubbleWidth, bubbleHeight);

            _content.sizeDelta = new Vector2(-_bubbleHorizontalPadding, -_bubbleVerticalPadding);

            var textRect = _messageText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;

            var itemRect = transform as RectTransform;
            var itemHeight = Mathf.Max(_minItemHeight, bubbleHeight + _itemVerticalPadding);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemHeight);
            _container.sizeDelta = new Vector2(_container.sizeDelta.x, itemHeight);
        }

        private bool HasValidSetup()
        {
            if (_container == null || _message == null || _content == null || _bubble == null || _user == null || _layoutGroup == null)
            {
                Debug.LogError("DialogueItemView requires container, message, content, bubble, user and layout group references.", this);
                return false;
            }

            if (_bubbleImage == null || _tailImage == null || _avatarImage == null || _emptyAvatar == null)
            {
                Debug.LogError("DialogueItemView requires bubble, tail, avatar and empty avatar references.", this);
                return false;
            }

            if (_nameText == null || _messageText == null)
            {
                Debug.LogError("DialogueItemView requires name and message text references.", this);
                return false;
            }

            return true;
        }
    }
}