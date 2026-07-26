using System.Collections;
using UnityEngine;

namespace Countdown
{
    public class TowerEnemy2D : EnemyBase2D
    {
        public enum TowerState { Burrowed, Emerging, Shooting, Waiting, Burrowing }

        [Header("Tower Cycle Timing")]
        [SerializeField] private float burrowedDuration = 4f;
        [SerializeField] private float emergeDuration = 0.6f;
        [SerializeField] private float shootDuration = 3f;
        [SerializeField] private float waitAfterShootDuration = 0.5f;
        [SerializeField] private float burrowDuration = 0.6f;

        [Header("Shooting")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private Transform firePoint;

        [Header("Emerge / Burrow Visuals")]
        [SerializeField] private Transform visuals;
        [SerializeField] private float emergeHeight = 1.2f;

        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private Collider2D hitCollider;

        private TowerState _state = TowerState.Burrowed;
        private float _nextFireTime;
        private float _burrowedY;

        private static readonly int IsEmerged = Animator.StringToHash("IsEmerged");

        public TowerState State => _state;

        protected override void Awake()
        {
            base.Awake();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (hitCollider == null) hitCollider = GetComponent<Collider2D>();
        }

        protected override void Start()
        {
            base.Start();
            currentState = AIState.Guarding;

            if (visuals != null)
                _burrowedY = visuals.localPosition.y - emergeHeight;

            SetBurrowed();
            StartCoroutine(TowerCycleRoutine());
        }

        protected override void FixedUpdate()
        {
            // Tower doesn't move
        }

        protected override void Update()
        {
            base.Update();

            if (_state == TowerState.Shooting && target != null && Time.time >= _nextFireTime)
            {
                FireAtPlayer();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        private IEnumerator TowerCycleRoutine()
        {
            yield return new WaitForSeconds(burrowedDuration);

            while (true)
            {
                // Emerge
                _state = TowerState.Emerging;
                if (hitCollider != null) hitCollider.enabled = true;
                if (animator != null) animator.SetBool(IsEmerged, true);
                yield return StartCoroutine(AnimateEmerge(true));

                // Shoot
                _state = TowerState.Shooting;
                _nextFireTime = Time.time;
                yield return new WaitForSeconds(shootDuration);

                // Wait
                _state = TowerState.Waiting;
                yield return new WaitForSeconds(waitAfterShootDuration);

                // Burrow
                _state = TowerState.Burrowing;
                if (animator != null) animator.SetBool(IsEmerged, false);
                yield return StartCoroutine(AnimateEmerge(false));
                SetBurrowed();

                // Stay underground
                yield return new WaitForSeconds(burrowedDuration);
            }
        }

        private void SetBurrowed()
        {
            _state = TowerState.Burrowed;
            if (hitCollider != null) hitCollider.enabled = false;

            if (visuals != null)
            {
                var pos = visuals.localPosition;
                pos.y = _burrowedY;
                visuals.localPosition = pos;
            }
        }

        private IEnumerator AnimateEmerge(bool emerging)
        {
            if (visuals == null)
            {
                yield return new WaitForSeconds(emerging ? emergeDuration : burrowDuration);
                yield break;
            }

            float duration = emerging ? emergeDuration : burrowDuration;
            float startY = emerging ? _burrowedY : _burrowedY + emergeHeight;
            float endY = emerging ? _burrowedY + emergeHeight : _burrowedY;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                var pos = visuals.localPosition;
                pos.y = Mathf.Lerp(startY, endY, t);
                visuals.localPosition = pos;
                yield return null;
            }

            var final = visuals.localPosition;
            final.y = endY;
            visuals.localPosition = final;
        }

        private void FireAtPlayer()
        {
            if (projectilePrefab == null || target == null) return;

            Vector2 origin = firePoint != null ? (Vector2)firePoint.position : body.position;
            Vector2 direction = ((Vector2)target.position - origin).normalized;
            GameObject projObj = Instantiate(projectilePrefab, origin, Quaternion.identity);
            if (projObj.TryGetComponent(out Projectile2D projectile))
            {
                projectile.Fire(origin, direction);
            }
        }
    }
}
