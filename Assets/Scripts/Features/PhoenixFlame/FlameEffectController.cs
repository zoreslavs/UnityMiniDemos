using UnityEngine;

namespace UnityMiniDemos.Features.PhoenixFlame
{
    [RequireComponent(typeof(Animator))]
    public sealed class FlameEffectController : MonoBehaviour
    {
        private static readonly int NextColorTrigger = Animator.StringToHash("NextColor");

        [SerializeField] private ParticleSystem[] _colorSystems;

        // Animated by PhoenixFlameColor.controller.
        [HideInInspector] public Color Tint = new Color(1f, 0.35f, 0.02f, 1f);

        private Animator _animator;
        private Color _lastTint;
        private Gradient _flameGradient;
        private GradientColorKey[] _colorKeys;
        private GradientAlphaKey[] _alphaKeys;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _flameGradient = new Gradient();
            _colorKeys = new GradientColorKey[3];
            _alphaKeys = new GradientAlphaKey[2];

            ApplyTint();
            _lastTint = Tint;
        }

        private void Update()
        {
            if (Tint == _lastTint)
                return;

            ApplyTint();
            _lastTint = Tint;
        }

        public void NextColor()
        {
            _animator.SetTrigger(NextColorTrigger);
        }

        private void ApplyTint()
        {
            if (_colorSystems == null || _colorSystems.Length == 0)
                return;

            UpdateFlameGradient(Tint);
            var gradient = new ParticleSystem.MinMaxGradient(_flameGradient);

            foreach (var system in _colorSystems)
            {
                if (system == null)
                    continue;

                var main = system.main;

                // Keeps textures neutral so colorOverLifetime fully controls their tint.
                main.startColor = new ParticleSystem.MinMaxGradient(Color.white);

                var color = system.colorOverLifetime;
                color.enabled = true;
                color.color = gradient;
            }
        }

        private void UpdateFlameGradient(Color color)
        {
            var highlight = Color.Lerp(color, Color.white, 0.15f);
            var shadow = Color.Lerp(color, Color.black, 0.3f);

            _colorKeys[0] = new GradientColorKey(highlight, 0f);
            _colorKeys[1] = new GradientColorKey(color, 0.45f);
            _colorKeys[2] = new GradientColorKey(shadow, 1f);

            _alphaKeys[0] = new GradientAlphaKey(color.a, 0f);
            _alphaKeys[1] = new GradientAlphaKey(0f, 1f);

            _flameGradient.SetKeys(_colorKeys, _alphaKeys);
        }
    }
}