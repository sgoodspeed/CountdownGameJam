using UnityEngine;
using UnityEngine.Serialization;

namespace Countdown
{
    [DisallowMultipleComponent]
    public abstract class DamageModule : MonoBehaviour, IDamageable
    {
        [FormerlySerializedAs("knockbackForce")]
        [Header("Hit Reactions")]
        [SerializeField] private float knockbackDistance = 5f;
        [SerializeField] private float stunDuration = 0.3f;

        public float KnockbackDistance => knockbackDistance;
        public float StunDuration => stunDuration;
        public bool IsDead { get; protected set; }

        public abstract void TakeDamage(float amount, Vector2 hitDirection);
    }
}
