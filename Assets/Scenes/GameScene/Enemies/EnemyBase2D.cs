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
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float skinDistance = 0.1f;

        [Header("Navigation & Targeting")]
        public AIState currentState = AIState.Chasing;
        public Transform target;

        [Header("References")]
        [SerializeField] protected Rigidbody2D body;
        [SerializeField] private Collider2D collision;
        [Tooltip("Layers considered as obstacles by the chase movement's avoidance cast.")]
        [SerializeField] private LayerMask movementCastLayers = ~0;

        [Header("Boundary Tint")]
        [Tooltip("Color to blend toward when outside the boundary.")]
        [SerializeField] private Color outsideColor = Color.black;
        [Tooltip("How far beyond the boundary the enemy can be before it's fully tinted.")]
        [SerializeField] private float fadeDistance = 5f;

        // Wobble variables to spread out pathing
        private float _wobbleSpeed;
        private float _wobbleIntensity;
        private float _wobbleOffset;

        private readonly RaycastHit2D[] hits = new RaycastHit2D[10];
        private ContactFilter2D movementCastFilter;

        protected CircleBoundary _boundary;
        private SpriteRenderer[] _spriteRenderers;
        private Color[] _originalColors;

        protected virtual void Awake()
        {
            body.freezeRotation = true;

            movementCastFilter = ContactFilter2D.noFilter;
            movementCastFilter.SetLayerMask(movementCastLayers);

            _wobbleSpeed = Random.Range(0.8f, 2.5f);
            _wobbleIntensity = Random.Range(1.0f, 3.5f);
            _wobbleOffset = Random.Range(0f, 10f);

            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _originalColors = new Color[_spriteRenderers.Length];
            for (int i = 0; i < _spriteRenderers.Length; i++)
                _originalColors[i] = _spriteRenderers[i].color;
        }

        protected virtual void Start()
        {
            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    target = playerObj.transform;
            }

            if (_boundary == null)
                _boundary = GameState.Instance.Boundary;
        }

        protected virtual void Update()
        {
            UpdateBoundaryTint();
        }

        protected virtual void FixedUpdate()
        {
            if (currentState == AIState.Chasing && target != null)
            {
                ChaseTarget();
            }
        }

        private void UpdateBoundaryTint()
        {
            if (_boundary == null || _spriteRenderers == null) return;

            float dist = ((Vector2)transform.position - (Vector2)_boundary.transform.position).magnitude;
            float overshoot = dist - _boundary.radius;
            float t = Mathf.Clamp01(overshoot / fadeDistance);

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] != null)
                    _spriteRenderers[i].color = Color.Lerp(_originalColors[i], outsideColor, t);
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
            var hitCount = collision.Cast(direction, movementCastFilter, hits, distance + Mathf.Epsilon);
            for (var i = 0; i < hitCount; i++)
            {
                distance = Mathf.Min(distance, hits[i].distance - skinDistance);
            }

            body.MovePosition(body.position + direction * distance);
        }
    }
}
