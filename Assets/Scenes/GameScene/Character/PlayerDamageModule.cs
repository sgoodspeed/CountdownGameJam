using System;
using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Player-side DamageModule. Unlike enemies, the player has no Animator, so
    /// hit/death feedback is a DOTween colour flash/fade on its sprites, and a
    /// brief invulnerability window is applied after each hit so overlapping
    /// enemy contacts in the same frame/animation can't chain-kill the player.
    /// </summary>
    public class PlayerDamageModule : DamageModule
    {
        [Header("Player Damage")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;

        [Header("Player Effects")]
        [SerializeField] private SpriteRenderer[] flashRenderers;
        [SerializeField] private Color hitFlashColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.2f;
        [SerializeField] private float deathFadeDuration = 0.6f;

        [Header("References")]
        [SerializeField] private CharacterMovement2D movement;
        [SerializeField] private CharacterMelee2D melee;

        private float _invulnerableUntil;
        private Sequence _hitFlashSequence;

        private void Awake()
        {
            GameState.Instance.ClockRanOut += Die;
        }
        
        private void OnDestroy()
        {
            GameState.Instance.ClockRanOut -= Die;
        }

        public override void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;
            if (Time.time < _invulnerableUntil) return;

            _invulnerableUntil = Time.time + invulnerabilityDuration;
            OnDamaged(amount);
            GameState.Instance.GameClock.ApplyDamage(amount, invulnerabilityDuration);
        }

        private void OnDamaged(float amount)
        {
            _hitFlashSequence?.Kill();
            _hitFlashSequence = DOTween.Sequence();
            foreach (var flashRenderer in flashRenderers)
            {
                _hitFlashSequence.Join(flashRenderer.DOColor(hitFlashColor, hitFlashDuration * 0.5f).SetLoops(2, LoopType.Yoyo));
            }
        }

        private void Die()
        {
            _hitFlashSequence?.Kill();

            if (movement != null) movement.enabled = false;
            if (melee != null) melee.enabled = false;

            foreach (var flashRenderer in flashRenderers)
            {
                flashRenderer.DOFade(0f, deathFadeDuration);
            }
        }
    }
}
