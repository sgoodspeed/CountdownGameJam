using System.Collections;
using UnityEngine;

namespace Countdown
{
    public class TowerEnemy2D : EnemyBase2D
    {
        public enum TowerState { Walking, Idle, Shooting, Waiting }

        [Header("Walking")]
        [SerializeField] private float arrivalDistance = 0.5f;

        [Header("Tower Cycle Timing")]
        [SerializeField] private float shootDuration = 3f;
        [SerializeField] private float waitDuration = 2f;

        [Header("Shooting")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private Transform firePoint;

        [Header("Components")]
        [SerializeField] private Animator animator;

        private TowerState _state = TowerState.Walking;
        private Vector2 _guardPosition;
        private Transform _walkTarget;
        private Transform _playerTransform;
        private float _nextFireTime;

        public TowerState State => _state;

        public void SetGuardPosition(Vector2 position)
        {
            _guardPosition = position;

            if (_walkTarget == null)
            {
                var go = new GameObject("TowerWalkTarget");
                _walkTarget = go.transform;
            }

            _walkTarget.position = position;
            target = _walkTarget;
        }

        protected override void Awake()
        {
            base.Awake();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        protected override void Start()
        {
            base.Start();
            currentState = AIState.Chasing;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _playerTransform = playerObj.transform;
        }

        protected override void FixedUpdate()
        {
            if (_state == TowerState.Walking)
            {
                base.FixedUpdate();

                float dist = Vector2.Distance(body.position, _guardPosition);
                if (dist <= arrivalDistance)
                {
                    body.MovePosition(_guardPosition);
                    currentState = AIState.Guarding;
                    StartCoroutine(ShootCycleRoutine());
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_state == TowerState.Shooting && _playerTransform != null && Time.time >= _nextFireTime)
            {
                FireAtPlayer();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        private IEnumerator ShootCycleRoutine()
        {
            while (true)
            {
                _state = TowerState.Shooting;
                _nextFireTime = Time.time;
                yield return new WaitForSeconds(shootDuration);

                _state = TowerState.Waiting;
                yield return new WaitForSeconds(waitDuration);
            }
        }

        private void FireAtPlayer()
        {
            if (projectilePrefab == null || _playerTransform == null) return;

            Vector2 origin = firePoint != null ? (Vector2)firePoint.position : body.position;
            Vector2 direction = ((Vector2)_playerTransform.position - origin).normalized;
            GameObject projObj = Instantiate(projectilePrefab, origin, Quaternion.identity);
            if (projObj.TryGetComponent(out Projectile2D projectile))
            {
                projectile.Fire(origin, direction);
            }
        }

        private void OnDestroy()
        {
            if (_walkTarget != null)
                Destroy(_walkTarget.gameObject);
        }
    }
}
