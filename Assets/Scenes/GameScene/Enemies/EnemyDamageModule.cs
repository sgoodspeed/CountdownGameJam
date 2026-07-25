using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Enemy-side DamageModule. Reaction to damage/death is driven through the
    /// Animator (matching the trigger pattern already used for attacks), and
    /// death disables the enemy's AI/collider and notifies the spawner before
    /// destroying the object once the death animation has had time to play.
    /// </summary>
    public class EnemyDamageModule : DamageModule
    {
        [Header("Enemy Effects")]
        [SerializeField] private Animator animator;
        [SerializeField] private string hitTrigger = "IsHit";
        [SerializeField] private string deathTrigger = "IsDead";
        [SerializeField] private float deathAnimDuration = 1.0f;
        [Tooltip("Optional - any attack hitbox this enemy owns, force-disabled on death in case it died mid-swing.")]
        [SerializeField] private GameObject attackHitbox;
        
        [Header("Health")]
        [SerializeField] protected float maxHealth = 10f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; protected set; }

        private EnemyBase2D _enemy;
        private Collider2D _collision;
        private Tween _deathTween;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            _enemy = GetComponent<EnemyBase2D>();
            _collision = GetComponent<Collider2D>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
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
            if (animator != null && !string.IsNullOrEmpty(hitTrigger))
            {
                animator.SetTrigger(hitTrigger);
            }
        }

        private void Die()
        {
            // Stop AI/attack behaviour (and any in-flight attack coroutine) immediately.
            if (_enemy != null)
            {
                _enemy.StopAllCoroutines();
                _enemy.enabled = false;
            }

            // Let projectiles/melee pass through the corpse during the death animation.
            if (_collision != null) _collision.enabled = false;

            // In case death interrupted an attack coroutine mid-swing (StopAllCoroutines
            // doesn't run its cleanup), make sure the hitbox doesn't stay live on a corpse.
            if (attackHitbox != null) attackHitbox.SetActive(false);

            if (EnemySpawner2D.Instance != null) EnemySpawner2D.Instance.EnemyDied();

            if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            {
                animator.SetTrigger(deathTrigger);
                _deathTween = DOVirtual.DelayedCall(deathAnimDuration, () => Destroy(gameObject));
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _deathTween?.Kill();
        }
    }
}
