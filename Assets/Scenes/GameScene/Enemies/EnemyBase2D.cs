using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// 2D counterpart to EnemyBase. There is no NavMesh in the 2D scene, so instead of
    /// a NavMeshAgent this chases the target directly using the same Collider2D.Cast
    /// obstacle-avoidance pattern as CharacterMovement2D, so enemies respect the same
    /// level boundary the player does.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyBase2D : MonoBehaviour
    {
        public enum AIState { Chasing, Guarding }

        [Header("Base Properties")]
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float skinDistance = 0.1f;

        [Header("Navigation & Targeting")]
        public AIState currentState = AIState.Chasing;
        public Transform target;

        [Header("References")]
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D collision;

        protected float currentHealth;

        // Wobble variables to spread out pathing
        private float _wobbleSpeed;
        private float _wobbleIntensity;
        private float _wobbleOffset;

        private readonly RaycastHit2D[] hits = new RaycastHit2D[10];

        protected virtual void Awake()
        {
            body.freezeRotation = true;

            // Give each enemy instance a unique wobble personality
            _wobbleSpeed = Random.Range(0.8f, 2.5f);
            _wobbleIntensity = Random.Range(1.0f, 3.5f);
            _wobbleOffset = Random.Range(0f, 10f);
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;

            // find player by tag if no target is assigned
            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    target = playerObj.transform;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (currentState == AIState.Chasing && target != null)
            {
                ChaseTarget();
            }
        }

        private void ChaseTarget()
        {
            // Calculate circular jitter offset using Sin/Cos so enemies don't stack on the same path
            float x = Mathf.Sin(Time.time * _wobbleSpeed + _wobbleOffset) * _wobbleIntensity;
            float y = Mathf.Cos(Time.time * _wobbleSpeed + _wobbleOffset) * _wobbleIntensity;
            Vector2 jitter = new Vector2(x, y);

            Vector2 toTarget = (Vector2)target.position + jitter - body.position;
            if (toTarget.sqrMagnitude < 0.0001f) return;

            Vector2 direction = toTarget.normalized;
            var distance = moveSpeed * Time.fixedDeltaTime;
            var hitCount = collision.Cast(direction, hits, distance + Mathf.Epsilon);
            for (var i = 0; i < hitCount; i++)
            {
                distance = Mathf.Min(distance, hits[i].distance - skinDistance);
            }

            body.MovePosition(body.position + direction * distance);
        }

        public virtual void TakeDamage(float amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            // Notify spawner so it can decrement active enemy count
            if (EnemySpawner2D.Instance != null)
            {
                EnemySpawner2D.Instance.EnemyDied();
            }

            Destroy(gameObject);
        }
    }
}
