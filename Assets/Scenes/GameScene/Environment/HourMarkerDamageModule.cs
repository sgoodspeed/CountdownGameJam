using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    public class HourMarkerDamageModule : DamageModule, ISerializationCallbackReceiver
    {
        [Header("Hour Marker")]
        [SerializeField] private HourMarker hourMarker;
        [SerializeField] private float restoreLerpDuration = 0.5f;
        [SerializeField] private HourMarkerContainer container;

        [Header("Health")]
        [SerializeField] private float maxHealth = 10f;

        [Header("Flash Effects")]
        [SerializeField] private SpriteRenderer[] flashRenderers;
        [SerializeField] private SpriteFlashSettings hitFlash;
        [SerializeField] private SpriteFlashSettings deathFlash;

        public float CurrentHealth { get; private set; }

        private Collider2D _collider;
        private Tween _flashTween;
        private Tween _deathTween;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            _collider = GetComponent<Collider2D>();
            if (hourMarker == null) hourMarker = GetComponent<HourMarker>();
            if (flashRenderers == null || flashRenderers.Length == 0)
                flashRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public override void TakeDamage(float amount, Vector2 hitDirection)
        {
            if (IsDead || amount <= 0f) return;
            if (hourMarker.Phase != HourMarkerPhase.Active) return;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

            _flashTween?.Kill(true);
            _flashTween = SpriteFlash.Play(flashRenderers, hitFlash);

            if (CurrentHealth <= 0f)
            {
                IsDead = true;
                Die();
            }
        }

        private void Die()
        {
            _flashTween?.Kill(true);
            if (_collider != null) _collider.enabled = false;

            hourMarker.OnDestroyed();
            
            if (!container.AnyActiveMarkerAbove(hourMarker.Hour))
            {
                var lowestDestroyed = container.FindNextLowestHour(hourMarker.Hour) - 1;
                float currentHours = GameState.Instance.NormalizedTime * 12f;
                float hoursToRestore = currentHours - lowestDestroyed;
                if (hoursToRestore > 0f)
                    GameState.Instance.GameClock.RestoreTime(hoursToRestore, restoreLerpDuration);
            }

            _flashTween = SpriteFlash.Play(flashRenderers, deathFlash);

            float delay = deathFlash != null ? deathFlash.duration : 0f;
            _deathTween = DOVirtual.DelayedCall(delay, () => hourMarker.ApplyDestroyedVisuals());
        }

        private void OnDestroy()
        {
            _flashTween?.Kill();
            _deathTween?.Kill();
        }

        public void OnBeforeSerialize()
        {
            if (!container)
            {
                container = GetComponentInParent<HourMarkerContainer>();
            }
        }
        public void OnAfterDeserialize() { }
    }
}
