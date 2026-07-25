using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    public class EnemyDamageModule : DamageModule
    {
        [Header("Enemy Effects")]
        [SerializeField] private Animator animator;
        [SerializeField] private string hitTrigger = "IsHit";
        [SerializeField] private string deathTrigger = "IsDead";
        [SerializeField] private float deathAnimDuration = 1.0f;
        [Tooltip("Optional - any attack hitbox this enemy owns, force-disabled on death in case it died mid-swing.")]
        [SerializeField] private GameObject attackHitbox;

        [Header("Flash Effects")]
        [SerializeField] private SpriteRenderer[] flashRenderers;
        [SerializeField] private SpriteFlashSettings hitFlash;
        [SerializeField] private SpriteFlashSettings deathFlash;

        [Header("Health")]
        [SerializeField] protected float maxHealth = 10f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; protected set; }

        private EnemyBase2D _enemy;
        private Collider2D _collision;
        private Tween _deathTween;
        private Tween _flashTween;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            _enemy = GetComponent<EnemyBase2D>();
            _collision = GetComponent<Collider2D>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (flashRenderers == null || flashRenderers.Length == 0)
                flashRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public override void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            OnDamaged(amount);

            if (CurrentHealth <= 0f)
            {
                IsDead = true;
                Die();
            }
        }

        private void OnDamaged(float amount)
        {
            _flashTween?.Kill(true);
            _flashTween = SpriteFlash.Play(flashRenderers, hitFlash);

            if (animator != null && !string.IsNullOrEmpty(hitTrigger))
                animator.SetTrigger(hitTrigger);
        }

        private void Die()
        {
            _flashTween?.Kill(true);

            if (_enemy != null)
            {
                _enemy.StopAllCoroutines();
                _enemy.enabled = false;
            }

            if (_collision != null) _collision.enabled = false;
            if (attackHitbox != null) attackHitbox.SetActive(false);
            if (EnemySpawner2D.Instance != null) EnemySpawner2D.Instance.EnemyDied();

            _flashTween = SpriteFlash.Play(flashRenderers, deathFlash);

            if (animator != null && !string.IsNullOrEmpty(deathTrigger))
                animator.SetTrigger(deathTrigger);

            float destroyDelay = Mathf.Max(deathAnimDuration, deathFlash != null ? deathFlash.duration : 0f);
            _deathTween = DOVirtual.DelayedCall(destroyDelay, () => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            _flashTween?.Kill();
            _deathTween?.Kill();
        }
    }
}
