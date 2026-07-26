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

            if (target == null) return;

            if (Time.time >= _nextFireTime)
            {
                FireAtPlayer();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        protected override void FixedUpdate()
        {
            Vector2 toDestination = _destination - body.position;
            if (toDestination.sqrMagnitude < 0.5f)
            {
                PickNewDestination();
                return;
            }

            Vector2 direction = toDestination.normalized;
            body.MovePosition(body.position + direction * (flySpeed * Time.fixedDeltaTime));
        }

        private void PickNewDestination()
        {
            Vector2 center = _boundary != null
                ? (Vector2)_boundary.transform.position
                : Vector2.zero;
            float radius = _boundary != null ? _boundary.radius : 8f;

            float currentAngle = Mathf.Atan2(
                body.position.y - center.y,
                body.position.x - center.x) * Mathf.Rad2Deg;

            float newAngle = currentAngle + 180f + Random.Range(-45f, 45f);
            float dist = radius + 1f;

            float rad = newAngle * Mathf.Deg2Rad;
            _destination = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * dist;
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
