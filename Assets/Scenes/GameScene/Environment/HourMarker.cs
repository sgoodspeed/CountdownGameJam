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
        [SerializeField] private float indicatorRadius = 3f;
        [SerializeField] private float indicatorScale = 0.3f;
        [SerializeField] private Color indicatorColor = Color.white;

        private const int IndicatorCount = 9;

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

        private void Update()
        {
            var progress = GameState.Instance.GetTargetHourProgress(hour);
            if (Phase == HourMarkerPhase.Destroyed)
            { // if destroyed leave visible
                if (progress < 1f)
                { // but allow progress to revert if time moves backwards
                    Phase = HourMarkerPhase.Rising;
                    damageModule.Reset();
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
            }

            if (progress < 1f && Phase == HourMarkerPhase.Active)
            {
                Phase = HourMarkerPhase.Rising;
                damageModule.Reset();
            }

            sprite.color = Color.Lerp(startColor, endColor, progress);
            hourText.color = Color.Lerp(textStartColor, textEndColor, progress);
            UpdateHealthIndicators();
        }

        public void OnDestroyed()
        {
            Phase = HourMarkerPhase.Destroyed;
        }

        public void ApplyDestroyedVisuals()
        {
            sprite.color = destroyedColor;
            hourText.color = destroyedTextColor;
            SetAllIndicators(false);
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
            _healthIndicators = new SpriteRenderer[IndicatorCount];
            var usedSprite = sprite.sprite;

            for (int i = 0; i < IndicatorCount; i++)
            {
                float angle = 90f - (i * 360f / IndicatorCount);
                float rad = angle * Mathf.Deg2Rad;

                var go = new GameObject($"HealthIndicator_{i}");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = new Vector3(
                    Mathf.Cos(rad) * indicatorRadius,
                    Mathf.Sin(rad) * indicatorRadius,
                    0f
                );
                go.transform.localScale = Vector3.one * indicatorScale;

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = usedSprite;
                sr.drawMode = SpriteDrawMode.Simple;
                sr.color = indicatorColor;
                sr.sortingLayerID = sprite.sortingLayerID;
                sr.sortingOrder = sprite.sortingOrder;

                _healthIndicators[i] = sr;
                go.SetActive(false);
            }
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
