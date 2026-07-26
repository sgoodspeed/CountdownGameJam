using System.Collections;
using UnityEngine;

namespace Countdown
{
    public class TowerEnemy2D : EnemyBase2D
    {
        public enum TowerState { Walking, Burrowed, Emerging, Shooting, Waiting, Burrowing }

        [Header("Walking")]
        [SerializeField] private float arrivalDistance = 0.5f;

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

        private TowerState _state = TowerState.Walking;
        private Vector2 _guardPosition;
        private Transform _walkTarget;
        private Transform _playerTransform;
        private float _nextFireTime;
        private float _burrowedY;

        private static readonly int IsEmerged = Animator.StringToHash("IsEmerged");

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
            if (hitCollider == null) hitCollider = GetComponent<Collider2D>();
        }

        protected override void Start()
        {
            base.Start();
            currentState = AIState.Chasing;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _playerTransform = playerObj.transform;

            if (visuals != null)
                _burrowedY = visuals.localPosition.y - emergeHeight;
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
                    StartCoroutine(TowerCycleRoutine());
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

        private IEnumerator TowerCycleRoutine()
        {
            SetBurrowed();
            yield return new WaitForSeconds(burrowedDuration);

            while (true)
            {
                _state = TowerState.Emerging;
                if (hitCollider != null) hitCollider.enabled = true;
                if (animator != null) animator.SetBool(IsEmerged, true);
                yield return StartCoroutine(AnimateEmerge(true));

                _state = TowerState.Shooting;
                _nextFireTime = Time.time;
                yield return new WaitForSeconds(shootDuration);

                _state = TowerState.Waiting;
                yield return new WaitForSeconds(waitAfterShootDuration);

                _state = TowerState.Burrowing;
                if (animator != null) animator.SetBool(IsEmerged, false);
                yield return StartCoroutine(AnimateEmerge(false));
                SetBurrowed();

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
