using UnityEngine;
using UnityEngine.Pool;

namespace Countdown
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile2D : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private float damage = 5f;
        [Tooltip("Which layers this projectile can hit and damage.")]
        [SerializeField] private LayerMask targetLayers = ~0;

        [Header("Movement")]
        [SerializeField] private float speed = 12f;
        [SerializeField] private float lifetime = 3f;

        [Header("Hit Effect")]
        [Tooltip("Prefab spawned at the impact point on collision or expiry.")]
        [SerializeField] private GameObject hitEffectPrefab;

        private Rigidbody2D _body;
        private float _spawnTime;
        private IObjectPool<Projectile2D> _pool;
        private bool _returned;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
        }

        public void SetPool(IObjectPool<Projectile2D> pool)
        {
            _pool = pool;
        }

        public void Fire(Vector2 position, Vector2 direction)
        {
            _returned = false;
            transform.position = position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            _body.linearVelocity = direction.normalized * speed;
            _spawnTime = Time.time;
        }

        private void Update()
        {
            if (!_returned && Time.time - _spawnTime >= lifetime)
            {
                SpawnHitEffect();
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_returned) return;
            if ((targetLayers & (1 << other.gameObject.layer)) == 0) return;

            if (other.TryGetComponent(out IDamageable damageable))
            {
                Vector2 hitDirection = _body.linearVelocity.normalized;
                damageable.TakeDamage(damage, hitDirection);
            }

            SpawnHitEffect();
            ReturnToPool();
        }

        private void SpawnHitEffect()
        {
            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        }

        private void ReturnToPool()
        {
            if (_returned) return;
            _returned = true;
            _body.linearVelocity = Vector2.zero;

            if (_pool != null)
                _pool.Release(this);
            else
                gameObject.SetActive(false);
        }
    }
}
