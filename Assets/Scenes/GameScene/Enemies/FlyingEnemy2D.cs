using UnityEngine;

namespace Countdown
{
    public class FlyingEnemy2D : EnemyBase2D
    {
        [Header("Flying Properties")]
        [SerializeField] private float flySpeed = 4f;

        [Header("Shooting")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float fireStartDelay = 0.5f;

        private Vector2 _destination;
        private float _nextFireTime;
        private bool _reachedDestination;

        public void SetDestination(Vector2 destination)
        {
            _destination = destination;
        }

        protected override void Start()
        {
            base.Start();
            currentState = AIState.Guarding;
            _nextFireTime = Time.time + fireStartDelay;
        }

        protected override void Update()
        {
            base.Update();

            if (_reachedDestination || target == null) return;

            if (Time.time >= _nextFireTime)
            {
                FireAtPlayer();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        protected override void FixedUpdate()
        {
            if (_reachedDestination) return;

            Vector2 toDestination = _destination - body.position;
            if (toDestination.sqrMagnitude < 0.5f)
            {
                _reachedDestination = true;
                var col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                if (EnemySpawner2D.Instance != null)
                    EnemySpawner2D.Instance.EnemyDied();
                Destroy(gameObject);
                return;
            }

            Vector2 direction = toDestination.normalized;
            body.MovePosition(body.position + direction * (flySpeed * Time.fixedDeltaTime));
        }

        private void FireAtPlayer()
        {
            if (projectilePrefab == null || target == null) return;

            Vector2 direction = ((Vector2)target.position - body.position).normalized;
            GameObject projObj = Instantiate(projectilePrefab, body.position, Quaternion.identity);
            if (projObj.TryGetComponent(out Projectile2D projectile))
            {
                projectile.Fire(body.position, direction);
            }
        }
    }
}
