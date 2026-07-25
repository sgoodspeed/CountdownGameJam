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
        [SerializeField] private Rigidbody2D body;

        private float _invulnerableUntil;
        private Tween _flashTween;
        private Tween _stunTween;

        private void Awake()
        {
            if (body == null) body = GetComponent<Rigidbody2D>();
            GameState.Instance.ClockRanOut += Die;
        }

        private void OnDestroy()
        {
            _flashTween?.Kill();
            _stunTween?.Kill();
            if (GameState.Instance != null)
                GameState.Instance.ClockRanOut -= Die;
        }

        public override void TakeDamage(float amount, Vector2 hitDirection)
        {
            if (IsDead || amount <= 0f) return;
            if (Time.time < _invulnerableUntil) return;

            _invulnerableUntil = Time.time + invulnerabilityDuration;

            _flashTween?.Kill(true);
            _flashTween = SpriteFlash.Play(flashRenderers, hitFlash);

            ApplyKnockback(hitDirection);
            ApplyStun();

            GameState.Instance.GameClock.ApplyDamage(amount, invulnerabilityDuration);
        }

        private void ApplyKnockback(Vector2 direction)
        {
            if (body != null && KnockbackDistance > 0f)
                body.MovePosition(body.position + direction * KnockbackDistance);
        }

        private void ApplyStun()
        {
            if (StunDuration <= 0f) return;

            _stunTween?.Kill();
            if (movement != null) movement.enabled = false;
            if (melee != null) melee.enabled = false;

            _stunTween = DOVirtual.DelayedCall(StunDuration, () =>
            {
                if (this == null || IsDead) return;
                if (body != null) body.linearVelocity = Vector2.zero;
                if (movement != null) movement.enabled = true;
                if (melee != null) melee.enabled = true;
            });
        }

        private void Die()
        {
            IsDead = true;
            _flashTween?.Kill(true);
            _stunTween?.Kill();

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
