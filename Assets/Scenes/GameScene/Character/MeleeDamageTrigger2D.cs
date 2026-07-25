using System.Collections.Generic;
using UnityEngine;

namespace Countdown
{
    public class MeleeDamageTrigger2D : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 10f;
        [Tooltip("The character that owns this weapon. Its facing direction (transform.right) is used as the knockback direction.")]
        [SerializeField] private Transform owner;

        // Track targets hit during the CURRENT swing to prevent hitting the same enemy every frame
        private readonly HashSet<Collider2D> _hitTargetsThisSwing = new HashSet<Collider2D>();

        private void OnEnable()
        {
            // Reset hit list whenever the sword visual becomes active at start of a swing
            _hitTargetsThisSwing.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore if we already hit this collider during this swing
            if (_hitTargetsThisSwing.Contains(other)) return;

            // Check if hit object can take damage (enemy, player, etc.)
            if (other.TryGetComponent(out IDamageable damageable))
            {
                _hitTargetsThisSwing.Add(other);
                Vector2 hitDirection = owner != null
                    ? (Vector2)owner.right
                    : (other.transform.position - transform.position).normalized;
                damageable.TakeDamage(damageAmount, hitDirection);
            }
        }
    }
}
