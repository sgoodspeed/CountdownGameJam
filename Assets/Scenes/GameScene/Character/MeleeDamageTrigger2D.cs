using System.Collections.Generic;
using UnityEngine;

namespace Countdown
{
    public class MeleeDamageTrigger2D : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 10f;
        [SerializeField] private float knockbackDistance = 5f;
        [SerializeField] private float stunDuration = 0.3f;
        [Tooltip("The character that owns this weapon (e.g. TestCharacter). Used to calculate outward knockback direction.")]
        [SerializeField] private Transform owner;

        private readonly HashSet<Collider2D> _hitTargetsThisSwing = new HashSet<Collider2D>();

        private void OnEnable()
        {
            ClearHits();
        }

        public void ClearHits()
        {
            _hitTargetsThisSwing.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore if we already hit this collider during this swing
            if (_hitTargetsThisSwing.Contains(other)) return;

            if (other.TryGetComponent(out IDamageable damageable))
            {
                _hitTargetsThisSwing.Add(other);

                // Always calculate knockback directly AWAY from the player center
                Vector2 hitDirection;
                if (owner != null)
                {
                    hitDirection = ((Vector2)other.transform.position - (Vector2)owner.position).normalized;
                }
                else
                {
                    hitDirection = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                }

                // Safety fallback if enemy and player overlap perfectly on exact same spot
                if (hitDirection == Vector2.zero)
                {
                    hitDirection = Vector2.right;
                }

                damageable.TakeDamage(damageAmount, hitDirection, knockbackDistance, stunDuration);
            }
        }
    }
}