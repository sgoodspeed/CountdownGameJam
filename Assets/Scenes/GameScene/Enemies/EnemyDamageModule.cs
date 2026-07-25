using System.Collections;
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

        private EnemyBase2D _enemy;
        private Collider2D _collision;

        protected override void Awake()
        {
            base.Awake();

            _enemy = GetComponent<EnemyBase2D>();
            _collision = GetComponent<Collider2D>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        protected override void OnDamaged(float amount)
        {
            if (animator != null && !string.IsNullOrEmpty(hitTrigger))
            {
                animator.SetTrigger(hitTrigger);
            }
        }

        protected override void Die()
        {
            // Stop AI/attack behaviour (and any in-flight attack coroutine) immediately.
            if (_enemy != null)
            {
                _enemy.StopAllCoroutines();
                _enemy.enabled = false;
            }

            // Let projectiles/melee pass through the corpse during the death animation.
            if (_collision != null) _collision.enabled = false;

            if (EnemySpawner2D.Instance != null) EnemySpawner2D.Instance.EnemyDied();

            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            {
                animator.SetTrigger(deathTrigger);
                yield return new WaitForSeconds(deathAnimDuration);
            }

            Destroy(gameObject);
        }
    }
}
