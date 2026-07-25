using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    public class PlayerDamageModule : DamageModule
    {
        [Header("Player Damage")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;

        [Header("Flash Effects")]
        [SerializeField] private SpriteRenderer[] flashRenderers;
        [SerializeField] private SpriteFlashSettings hitFlash;
        [SerializeField] private SpriteFlashSettings deathFlash;
        [SerializeField] private float deathFadeDuration = 0.6f;

        [Header("References")]
        [SerializeField] private CharacterMovement2D movement;
        [SerializeField] private CharacterMelee2D melee;

        private float _invulnerableUntil;
        private Tween _flashTween;

        private void Awake()
        {
            GameState.Instance.ClockRanOut += Die;
        }

        private void OnDestroy()
        {
            _flashTween?.Kill();
            if (GameState.Instance != null)
                GameState.Instance.ClockRanOut -= Die;
        }

        public override void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;
            if (Time.time < _invulnerableUntil) return;

            _invulnerableUntil = Time.time + invulnerabilityDuration;
            _flashTween?.Kill(true);
            _flashTween = SpriteFlash.Play(flashRenderers, hitFlash);
            GameState.Instance.GameClock.ApplyDamage(amount, invulnerabilityDuration);
        }

        private void Die()
        {
            _flashTween?.Kill(true);

            if (movement != null) movement.enabled = false;
            if (melee != null) melee.enabled = false;

            _flashTween = SpriteFlash.Play(flashRenderers, deathFlash);
            if (_flashTween != null)
                _flashTween.OnComplete(StartDeathFade);
            else
                StartDeathFade();
        }

        private void StartDeathFade()
        {
            foreach (var r in flashRenderers)
                r.DOFade(0f, deathFadeDuration);
        }
    }
}
