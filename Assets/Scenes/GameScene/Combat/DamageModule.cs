using UnityEngine;

namespace Countdown
{
    [DisallowMultipleComponent]
    public abstract class DamageModule : MonoBehaviour, IDamageable
    {
        [Header("Hit Reactions")]
        [SerializeField] private float stunDuration = 0.3f;

        public float StunDuration => stunDuration;
        public bool IsDead { get; protected set; }

        public abstract void TakeDamage(float amount, Vector2 hitDirection, float knockbackDistance = 0f);
    }
}
