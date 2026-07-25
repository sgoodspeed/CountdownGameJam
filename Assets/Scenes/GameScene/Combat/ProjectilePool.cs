using UnityEngine;
using UnityEngine.Pool;

namespace Countdown
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private Projectile2D prefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxSize = 50;

        private ObjectPool<Projectile2D> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<Projectile2D>(
                createFunc: () =>
                {
                    var projectile = Instantiate(prefab);
                    projectile.SetPool(_pool);
                    return projectile;
                },
                actionOnGet: p => p.gameObject.SetActive(true),
                actionOnRelease: p => p.gameObject.SetActive(false),
                actionOnDestroy: p => Destroy(p.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        public Projectile2D Get()
        {
            return _pool.Get();
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }
    }
}
