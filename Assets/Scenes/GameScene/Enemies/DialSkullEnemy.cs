// ============================================================================
// PURPOSE:
//   2D implementation of the Dial Skull enemy. Inherits custom 2D movement
//   from EnemyBase2D while adding swipe-attack pause timing and death animations.
// ============================================================================

using System.Collections;
using UnityEngine;

namespace Countdown
{
    public class DialSkullEnemy : EnemyBase2D
    {
        public enum SkullState { Chasing, Attacking, Dead }

        [Header("Dial Skull Properties")]
        [SerializeField] private SkullState skullState = SkullState.Chasing;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float attackPauseDuration = 0.8f;
        [SerializeField] private float deathAnimDuration = 1.0f;

        [Header("Components")]
        [SerializeField] protected Animator animator;

        private bool _isAttacking = false;

        protected override void Awake()
        {
            base.Awake();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        protected override void FixedUpdate()
        {
            if (skullState == SkullState.Dead || target == null) return;

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (skullState == SkullState.Chasing)
            {
                // Run base 2D physics chase movement (includes wobble & raycast collision)
                base.FixedUpdate();

                // Trigger attack when close enough
                if (distanceToTarget <= attackRange && !_isAttacking)
                {
                    StartCoroutine(PerformAttackRoutine());
                }
            }
        }

        private IEnumerator PerformAttackRoutine()
        {
            _isAttacking = true;
            skullState = SkullState.Attacking;

            // Pause movement during swipe (by simply not calling base.FixedUpdate())
            if (animator != null)
            {
                animator.SetTrigger("IsAttacking");
            }

            // Wait for attack swipe animation to complete
            yield return new WaitForSeconds(attackPauseDuration);

            // Resume chase if still alive
            if (skullState != SkullState.Dead)
            {
                skullState = SkullState.Chasing;
            }

            // Wait out cooldown before next attack
            yield return new WaitForSeconds(attackCooldown);
            _isAttacking = false;
        }

        public override void TakeDamage(float amount)
        {
            if (skullState == SkullState.Dead) return;
            base.TakeDamage(amount);
        }

        protected override void Die()
        {
            skullState = SkullState.Dead;

            // Disable collider so bullets pass through during death animation
            if (TryGetComponent(out Collider2D col)) col.enabled = false;

            // Notify 2D spawner immediately
            if (EnemySpawner2D.Instance != null)
            {
                EnemySpawner2D.Instance.EnemyDied();
            }

            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            if (animator != null)
            {
                animator.SetTrigger("IsDead");
            }

            // Wait for death animation to finish before destroying object
            yield return new WaitForSeconds(deathAnimDuration);
            Destroy(gameObject);
        }
    }
}