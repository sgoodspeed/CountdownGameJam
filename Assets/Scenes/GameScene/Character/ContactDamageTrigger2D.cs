using UnityEngine;

namespace Countdown
{
    public class ContactDamageTrigger2D : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 10f;
        [SerializeField] private float cooldown = 0.5f;

        private float lastHitTime = 0f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (lastHitTime < Time.time + cooldown)
            {
                lastHitTime = Time.time;
                
                // Check if hit object can take damage (enemy, player, etc.)
                if(!other.CompareTag("Player")) { return; }
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    Vector2 hitDirection = (other.transform.position - transform.position).normalized;
                    damageable.TakeDamage(damageAmount, hitDirection);
                }
            }
        }
    }
}
