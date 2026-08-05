using UnityEngine;

namespace UnityMiniDemos.Features.PhoenixFlame
{
    [RequireComponent(typeof(Animator))]
    public sealed class FlameEffectController : MonoBehaviour
    {
        private static readonly int NextColorTrigger = Animator.StringToHash("NextColor");

        [SerializeField] private ParticleSystem[] _colorSystems;
        [HideInInspector] public Color Tint = new Color(1f, 0.35f, 0.02f, 1f);

        private Animator _animator;
        private Color _lastTint;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

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

            var gradient = new ParticleSystem.MinMaxGradient(CreateFlameGradient(Tint));

            foreach (var system in _colorSystems)
            {
                if (system == null)
                    continue;

                var main = system.main;
                main.startColor = new ParticleSystem.MinMaxGradient(Color.white);

                var color = system.colorOverLifetime;
                color.enabled = true;
                color.color = gradient;
            }
        }

        private static Gradient CreateFlameGradient(Color color)
        {
            var highlight = Color.Lerp(color, Color.white, 0.15f);
            var shadow = Color.Lerp(color, Color.black, 0.3f);
            var gradient = new Gradient();
            gradient.SetKeys
            (
                new[]
                {
                    new GradientColorKey(highlight, 0f),
                    new GradientColorKey(color, 0.45f),
                    new GradientColorKey(shadow, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(color.a, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            return gradient;
        }
    }
}
