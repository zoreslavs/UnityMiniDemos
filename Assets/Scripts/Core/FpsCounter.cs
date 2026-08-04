using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Core
{
    public sealed class FpsCounter : MonoBehaviour
    {
        private const float DefaultUpdateInterval = 0.25f;
        private const float MinimumUpdateInterval = 0.1f;
        private const float InitialFrameDuration = 1f / 60f;
        private const float SmoothingSpeed = 8f;

        [SerializeField] private TMP_Text _counterText;
        [SerializeField, Min(MinimumUpdateInterval)] private float _updateInterval = DefaultUpdateInterval;

        private float _smoothedFrameDuration;
        private float _updateTimer;
        private int _lastDisplayedFps = -1;

        private void OnEnable()
        {
            _smoothedFrameDuration = InitialFrameDuration;
            _updateTimer = 0f;
            _lastDisplayedFps = -1;
            UpdateDisplay();
        }

        private void Update()
        {
            if (_counterText == null)
                return;

            var frameDuration = Time.unscaledDeltaTime;
            if (frameDuration <= 0f)
                return;

            var smoothingFactor = 1f - Mathf.Exp(-SmoothingSpeed * frameDuration);
            _smoothedFrameDuration = Mathf.Lerp(_smoothedFrameDuration, frameDuration, smoothingFactor);
            _updateTimer += frameDuration;

            var updateInterval = Mathf.Max(_updateInterval, MinimumUpdateInterval);
            if (_updateTimer < updateInterval)
                return;

            _updateTimer -= updateInterval;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_counterText == null || _smoothedFrameDuration <= 0f)
                return;

            var fps = Mathf.RoundToInt(1f / _smoothedFrameDuration);
            if (fps == _lastDisplayedFps)
                return;

            _lastDisplayedFps = fps;
            _counterText.SetText("FPS: {0}", fps);
        }
    }
}