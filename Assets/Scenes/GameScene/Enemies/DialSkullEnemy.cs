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
        public enum SkullState { Chasing, Attacking }

        [Header("Dial Skull Properties")]
        [SerializeField] private SkullState skullState = SkullState.Chasing;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float attackPauseDuration = 0.8f;

        [Header("Components")]
        [SerializeField] protected Animator animator;
        [Tooltip("Trigger hitbox enabled for the duration of the swipe - disabled the rest of the time.")]
        [SerializeField] private GameObject attackHitbox;

        private bool _isAttacking = false;

        protected override void Awake()
        {
            base.Awake();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        protected override void FixedUpdate()
        {
            if (target == null) return;

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
            skullState = SkullState.Chasing;

            // Wait out cooldown before next attack
            yield return new WaitForSeconds(attackCooldown);
            _isAttacking = false;
        }
    }
}