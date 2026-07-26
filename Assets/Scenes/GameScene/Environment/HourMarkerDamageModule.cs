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
        [SerializeField] private Collider2D collision;

        [Header("Health")]
        [SerializeField] private float maxHealth = 10f;

        [Header("Flash Effects")]
        [SerializeField] private SpriteRenderer[] flashRenderers;
        [SerializeField] private SpriteFlashSettings hitFlash;
        [SerializeField] private SpriteFlashSettings deathFlash;

        public float CurrentHealth { get; private set; }
        public float NormalizedHealth => maxHealth > 0f ? CurrentHealth / maxHealth : 0f;

        private Tween _flashTween;
        private Tween _deathTween;
        
        public void Reset()
        {
            CurrentHealth = maxHealth;
            IsDead = false;
            collision.enabled = false;
        }

        public void Activate()
        {
            collision.enabled = true;
        }

        private void Awake()
        {
            Reset();
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
            
            hourMarker.OnDestroyed();
            
            if (!container.AnyActiveMarkerAbove(hourMarker.Hour))
            {
                var lowestDestroyed = container.FindNextLowestHour(hourMarker.Hour) - 1;
                float currentHours = GameState.Instance.NormalizedTime * 12f;
                float hoursToRestore = currentHours - lowestDestroyed;
                if (hoursToRestore > 0f)
                    GameState.Instance.GameClock.SetHoursRemaining(12 - lowestDestroyed, restoreLerpDuration);
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
