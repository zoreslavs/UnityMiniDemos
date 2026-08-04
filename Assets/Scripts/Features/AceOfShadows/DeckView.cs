using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.AceOfShadows
{
    public sealed class DeckView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _counterText;

        public RectTransform RectTransform => (RectTransform)transform;

        public bool IsConfigured => _counterText != null;

        public void SetCount(int count)
        {
            if (_counterText == null)
                return;

            _counterText.SetText(count.ToString());
        }
    }
}