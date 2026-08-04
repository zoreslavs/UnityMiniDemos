using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UnityMiniDemos.Features.AceOfShadows
{
    public sealed class SpeedController : MonoBehaviour
    {
        [SerializeField] private Slider _speedSlider;
        [SerializeField] private TMP_Text _speedText;
        [SerializeField] private AnimationController _animationController;
        [SerializeField, Min(0.01f)] private float _minSpeed = 0.1f;
        [SerializeField, Min(0.01f)] private float _maxSpeed = 10f;
        [SerializeField, Min(0.01f)] private float _defaultSpeed = 1f;

        private float _minSpeedLog;
        private float _maxSpeedLog;

        private void Awake()
        {
            if (_speedSlider == null || _animationController == null)
            {
                Debug.LogError("SpeedController requires a slider and an animation controller.", this);
                return;
            }

            _minSpeedLog = Mathf.Log10(_minSpeed);
            _maxSpeedLog = Mathf.Log10(_maxSpeed);
            _speedSlider.minValue = 0f;
            _speedSlider.maxValue = 1f;
            _speedSlider.SetValueWithoutNotify(SpeedToSliderValue(_defaultSpeed));
        }

        private void OnEnable()
        {
            if (_speedSlider == null)
                return;

            _speedSlider.onValueChanged.AddListener(OnSpeedChanged);
            ApplySpeed(_speedSlider.value);
        }

        private void OnDisable()
        {
            _speedSlider?.onValueChanged.RemoveListener(OnSpeedChanged);
        }

        private void OnSpeedChanged(float speedMultiplier)
        {
            ApplySpeed(speedMultiplier);
        }

        private void ApplySpeed(float speedMultiplier)
        {
            if (_animationController == null)
                return;

            var actualSpeed = SliderValueToSpeed(speedMultiplier);
            _animationController.SetSpeedMultiplier(actualSpeed);

            if (_speedText == null)
                return;

            var formattedSpeed = actualSpeed.ToString(
                actualSpeed >= 1f && actualSpeed % 1f == 0f ? "0" : "0.0",
                CultureInfo.InvariantCulture);
            _speedText.SetText($"Speed: {formattedSpeed}x");
        }

        private float SliderValueToSpeed(float sliderValue)
        {
            return Mathf.Pow(10f, Mathf.Lerp(_minSpeedLog, _maxSpeedLog, Mathf.Clamp01(sliderValue)));
        }

        private float SpeedToSliderValue(float speed)
        {
            return Mathf.InverseLerp(
                _minSpeedLog,
                _maxSpeedLog,
                Mathf.Log10(Mathf.Clamp(speed, _minSpeed, _maxSpeed)));
        }
    }
}