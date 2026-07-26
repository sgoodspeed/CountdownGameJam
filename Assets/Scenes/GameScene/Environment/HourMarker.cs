using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    public enum HourMarkerPhase { Rising, Active, Destroyed }

    public class HourMarker : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private int hour;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private HourMarkerContainer container;
        [SerializeField] private HourMarkerDamageModule damageModule;

        [Header("Rising / Active Colors")]
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

        [Header("Destroyed Colors")]
        [SerializeField] private Color destroyedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("Enemy Spawning")]
        [SerializeField] private int baseSpawnCount = 1;
        [SerializeField] private float spawnScatter = 2f;

        private const int MaxDamageState = 6;
        private static readonly int DamageStateParam = Animator.StringToHash("DamageState");

        private Sequence animationSequence;
        private int _deathCount = 0;

        public int Hour => hour;
        public HourMarkerPhase Phase { get; private set; } = HourMarkerPhase.Rising;

        private static readonly (int Value, string Numeral)[] RomanNumerals =
        {
            (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
            (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
            (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I"),
        };

        private void Start()
        {
            sprite.color = startColor;
            SetDamageState(0);
        }

        private void Update()
        {
            var progress = GameState.Instance.GetTargetHourProgress(hour);
            if (Phase == HourMarkerPhase.Destroyed)
            { // if destroyed leave visible
                if (progress < 1f)
                { // but allow progress to revert if time moves backwards
                    Phase = HourMarkerPhase.Rising;
                    damageModule.Reset();
                    animationSequence?.Kill();
                }
                else
                { // stay in damaged / destroyed looking visual state
                    return;
                }
            }

            if (progress >= 1f && Phase == HourMarkerPhase.Rising)
            {
                Phase = HourMarkerPhase.Active;
                damageModule.Activate();
                ApplyActivatedVisual();
            }

            if (progress < 1f && Phase == HourMarkerPhase.Active)
            {
                Phase = HourMarkerPhase.Rising;
                damageModule.Reset();
            }

            UpdateDamageState();
        }

        public void OnDestroyed()
        {
            Phase = HourMarkerPhase.Destroyed;
            _deathCount++;
            SpawnEnemies();
        }
        
        public void ApplyActivatedVisual()
        {
            animationSequence?.Kill();
            animationSequence = DOTween.Sequence()
                .Append(sprite.DOColor(endColor, 1f)).SetEase(Ease.InOutCubic);
            SetDamageState(0);
            animator.SetBool("IsDead", false);
        }

        public void ApplyDestroyedVisuals()
        {
            animationSequence?.Kill();
            animationSequence = DOTween.Sequence()
                .Append(sprite.DOColor(startColor, 1f)).SetEase(Ease.InOutCubic);
            SetDamageState(MaxDamageState);
            animator.SetBool("IsDead", true);
        }

        private void SpawnEnemies()
        {
            if (EnemySpawner2D.Instance == null) return;
            int count = baseSpawnCount + _deathCount;
            EnemySpawner2D.Instance.SpawnNear(transform.position, count, spawnScatter);
        }

        private void UpdateDamageState()
        {
            if (Phase != HourMarkerPhase.Active)
            {
                SetDamageState(Phase == HourMarkerPhase.Destroyed ? MaxDamageState : 0);
                return;
            }

            float normalizedHealth = damageModule.NormalizedHealth;
            int state = Mathf.Clamp(Mathf.RoundToInt((1f - normalizedHealth) * MaxDamageState), 0, MaxDamageState);
            SetDamageState(state);
        }

        private void SetDamageState(int state)
        {
            animator.SetFloat(DamageStateParam, state / (float)MaxDamageState);
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
