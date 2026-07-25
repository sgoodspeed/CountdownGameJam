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
        public bool IsDead { get; protected set; }

        public abstract void TakeDamage(float amount);
    }
}
