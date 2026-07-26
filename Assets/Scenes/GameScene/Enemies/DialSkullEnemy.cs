using System.Collections;
using UnityEngine;

namespace Countdown
{
    public class DialSkullEnemy : EnemyBase2D
    {
        public enum SkullState { Chasing, WindingUp, Lunging }

        [Header("Dial Skull Properties")]
        [SerializeField] private SkullState skullState = SkullState.Chasing;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float windUpDuration = 0.8f;
        [SerializeField] private float lungeSpeed = 12f;
        [SerializeField] private float lungeDuration = 0.3f;

        [Header("Components")]
        [SerializeField] protected Animator animator;
        [Tooltip("Trigger hitbox enabled during the lunge - disabled the rest of the time.")]
        [SerializeField] private GameObject attackHitbox;

        private bool _isAttacking = false;
        private Vector2 _lungeDirection;

        protected override void Awake()
        {
            base.Awake();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        protected override void FixedUpdate()
        {
            if (target == null) return;

            if (skullState == SkullState.Chasing)
            {
                base.FixedUpdate();

                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if (distanceToTarget <= attackRange && !_isAttacking)
                {
                    StartCoroutine(PerformAttackRoutine());
                }
            }
            else if (skullState == SkullState.Lunging)
            {
                body.MovePosition(body.position + _lungeDirection * (lungeSpeed * Time.fixedDeltaTime));
            }
        }

        private IEnumerator PerformAttackRoutine()
        {
            _isAttacking = true;
            skullState = SkullState.WindingUp;

            _lungeDirection = ((Vector2)target.position - body.position).normalized;

            if (animator != null)
                animator.SetTrigger("IsAttacking");

            yield return new WaitForSeconds(windUpDuration);

            skullState = SkullState.Lunging;
            if (attackHitbox != null)
                attackHitbox.SetActive(true);

            yield return new WaitForSeconds(lungeDuration);

            if (attackHitbox != null)
                attackHitbox.SetActive(false);

            skullState = SkullState.Chasing;

            yield return new WaitForSeconds(attackCooldown);
            _isAttacking = false;
        }
    }
}