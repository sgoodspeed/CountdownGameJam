using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Countdown
{
    public enum HourMarkerPhase { Rising, Active, Destroyed }

    public class HourMarker : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private int hour;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TMP_Text hourText;
        [SerializeField] private HourMarkerContainer container;
        [SerializeField] private HourMarkerDamageModule damageModule;

        [Header("Rising / Active Colors")]
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;
        [SerializeField] private Color textStartColor;
        [SerializeField] private Color textEndColor;

        [Header("Destroyed Colors")]
        [SerializeField] private Color destroyedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private Color destroyedTextColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        [Header("Health Indicators")]
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private float indicatorRadius = 3f;
        [SerializeField] private Color indicatorColor = Color.white;

        [Header("Enemy Spawning")]
        [SerializeField] private int baseSpawnCount = 1;
        [SerializeField] private float spawnScatter = 2f;

        private const int IndicatorCount = 9;
        private Sequence animationSequence;
        private int _deathCount = 0;

        public int Hour => hour;
        public HourMarkerPhase Phase { get; private set; } = HourMarkerPhase.Rising;

        private SpriteRenderer[] _healthIndicators;

        private static readonly (int Value, string Numeral)[] RomanNumerals =
        {
            (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
            (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
            (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I"),
        };

        private void Awake()
        {
            hourText.text = ToRomanNumeral(hour);
            CreateHealthIndicators();
            
        }

        private void Start()
        {
            sprite.color = startColor;
            hourText.color = textStartColor;
            SetAllIndicators(false);
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

            UpdateHealthIndicators();
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
                .Append(sprite.DOColor(endColor, 1f)).SetEase(Ease.InOutCubic)
                .Append(hourText.DOColor(textEndColor, 1f)).SetEase(Ease.InOutCubic);
            SetAllIndicators(true);
        }

        public void ApplyDestroyedVisuals()
        {
            animationSequence?.Kill();
            animationSequence = DOTween.Sequence()
                .Append(sprite.DOColor(startColor, 1f)).SetEase(Ease.InOutCubic)
                .Append(hourText.DOColor(textStartColor, 1f)).SetEase(Ease.InOutCubic);
            SetAllIndicators(false);
        }

        private void SpawnEnemies()
        {
            if (EnemySpawner2D.Instance == null) return;
            int count = baseSpawnCount + _deathCount;
            EnemySpawner2D.Instance.SpawnNear(transform.position, count, spawnScatter);
        }

        private static string ToRomanNumeral(int number)
        {
            var result = new System.Text.StringBuilder();
            foreach (var (value, numeral) in RomanNumerals)
            {
                while (number >= value)
                {
                    result.Append(numeral);
                    number -= value;
                }
            }

            return result.ToString();
        }

        private void CreateHealthIndicators()
        {
            _healthIndicators = HourMarkerHealthIndicatorFactory.Create(new HourMarkerHealthIndicatorFactory.Config
            {
                Parent = transform,
                Count = IndicatorCount,
                Radius = indicatorRadius,
                Prefab = indicatorPrefab,
                SortingOrder = sprite.sortingOrder,
            });
        }

        private void UpdateHealthIndicators()
        {
            if (_healthIndicators == null) return;

            if (Phase != HourMarkerPhase.Active)
            {
                SetAllIndicators(false);
                return;
            }

            float normalizedHealth = damageModule.NormalizedHealth;
            int activeCount = Mathf.CeilToInt(normalizedHealth * IndicatorCount);

            for (int i = 0; i < IndicatorCount; i++)
                _healthIndicators[i].gameObject.SetActive(i < activeCount);
        }

        private void SetAllIndicators(bool active)
        {
            if (_healthIndicators == null) return;
            for (int i = 0; i < IndicatorCount; i++)
                _healthIndicators[i].gameObject.SetActive(active);
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
