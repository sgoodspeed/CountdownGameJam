using UnityEngine;

namespace Countdown
{
    [DisallowMultipleComponent]
    public abstract class DamageModule : MonoBehaviour, IDamageable
    {
        public bool IsDead { get; protected set; }

        public abstract void TakeDamage(float amount, Vector2 hitDirection, float knockbackDistance = 0f, float stunDuration = 0f);
    }
}
