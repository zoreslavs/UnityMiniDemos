using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.MagicWords
{
    public sealed class DialogueItemView : MonoBehaviour
    {
        [SerializeField] private RectTransform _bubble;
        [SerializeField] private RectTransform _user;
        [SerializeField] private Image _bubbleImage;
        [SerializeField] private Image _tailImage;
        [SerializeField] private RawImage _avatarImage;
        [SerializeField] private GameObject _emptyAvatar;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Color _leftBubbleColor = new Color32(58, 82, 114, 255);
        [SerializeField] private Color _rightBubbleColor = new Color32(72, 103, 137, 255);
        [SerializeField] private int _horizontalPadding = 24;
        [SerializeField] private float _minBubbleWidth = 280f;
        [SerializeField] private float _maxBubbleWidth = 500f;
        [SerializeField] private float _minBubbleHeight = 120f;
        [SerializeField] private float _bubbleHorizontalPadding = 48f;
        [SerializeField] private float _bubbleVerticalPadding = 40f;
        [SerializeField] private float _tailHorizontalInset = 74f;
        [SerializeField] private float _tailVerticalOffset = 31f;
        [SerializeField] private float _minItemHeight = 300f;
        [SerializeField] private float _itemVerticalPadding = 40f;

        private RectTransform _container;
        private RectTransform _message;
        private HorizontalLayoutGroup _layoutGroup;

        public void Configure(string name, string message, Texture2D avatar, bool alignLeft)
        {
            if (!HasValidSetup())
                return;

            _nameText.SetText(name ?? "Unknown");
            _messageText.SetText(message ?? string.Empty);
            _avatarImage.texture = avatar;
            _avatarImage.enabled = avatar != null;
            _emptyAvatar.SetActive(avatar == null);
            SetAlignment(alignLeft);
            ResizeMessage();
        }

        private void SetAlignment(bool alignLeft)
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

            var bubbleColor = alignLeft ? _leftBubbleColor : _rightBubbleColor;
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

            var contentRect = _messageText.rectTransform.parent as RectTransform;
            contentRect.sizeDelta = new Vector2(-_bubbleHorizontalPadding, -_bubbleVerticalPadding);

            var textRect = _messageText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;

            var tailPosition = _tailImage.rectTransform.anchoredPosition;
            tailPosition.x = bubbleWidth * 0.5f - _tailHorizontalInset;
            tailPosition.y = -bubbleHeight * 0.5f - _tailVerticalOffset;
            _tailImage.rectTransform.anchoredPosition = tailPosition;

            var itemRect = transform as RectTransform;
            var itemHeight = Mathf.Max(_minItemHeight, bubbleHeight + _itemVerticalPadding);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemHeight);
            _container.sizeDelta = new Vector2(_container.sizeDelta.x, itemHeight);
        }

        private bool HasValidSetup()
        {
            if (_bubble == null)
            {
                Debug.LogError("DialogueItemView requires bubble reference.", this);
                return false;
            }

            _container = FindContainer();

            if (_container == null)
            {
                Debug.LogError("DialogueItemView requires a container child.", this);
                return false;
            }

            _message = FindMessage();

            if (_message == null)
            {
                Debug.LogError("DialogueItemView requires a message child inside the container.", this);
                return false;
            }

            if (_user == null || _user.parent != _container)
                _user = _container.Find("User") as RectTransform;

            if (_user == null)
            {
                Debug.LogError("DialogueItemView requires a User child.", this);
                return false;
            }

            _layoutGroup = _container.GetComponent<HorizontalLayoutGroup>();

            if (_layoutGroup == null)
            {
                Debug.LogError("DialogueItemView requires a horizontal layout group on the container.", this);
                return false;
            }

            if (_bubbleImage == null || _tailImage == null || _avatarImage == null)
            {
                Debug.LogError("DialogueItemView requires image references.", this);
                return false;
            }

            if (_nameText == null || _messageText == null)
            {
                Debug.LogError("DialogueItemView requires name and message text references.", this);
                return false;
            }

            return true;
        }

        private RectTransform FindContainer()
        {
            var current = _bubble.parent as RectTransform;
            while (current != null && current.parent != transform)
            {
                current = current.parent as RectTransform;
            }

            return current;
        }

        private RectTransform FindMessage()
        {
            var current = _bubble.parent as RectTransform;
            while (current != null && current.parent != _container)
            {
                current = current.parent as RectTransform;
            }

            return current;
        }
    }
}