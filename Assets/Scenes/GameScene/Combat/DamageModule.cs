using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Shared health/damage tracking for anything that can be hurt and die.
    /// Subclasses (PlayerDamageModule, EnemyDamageModule) plug in their own
    /// hit/death reactions via the OnDamaged/Die hooks, and can override
    /// TakeDamage itself for different rules (e.g. invulnerability frames).
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class DamageModule : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] protected float maxHealth = 10f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; protected set; }
        public bool IsDead { get; protected set; }

        protected virtual void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public virtual void TakeDamage(float amount)
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

        /// <summary>Called on every non-fatal (and the fatal) hit, before the death check. Override for hit reactions.</summary>
        protected virtual void OnDamaged(float amount) { }

        /// <summary>Called once when health reaches zero. Override for death reactions/cleanup.</summary>
        protected abstract void Die();
    }
}
