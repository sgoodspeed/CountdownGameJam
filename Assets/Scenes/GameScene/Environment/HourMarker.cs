using TMPro;
using UnityEngine;

namespace Countdown
{
    public enum HourMarkerPhase { Rising, Active, Destroyed }

    public class HourMarker : MonoBehaviour
    {
        [SerializeField] private int hour;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TMP_Text hourText;

        [Header("Rising / Active Colors")]
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;
        [SerializeField] private Color textStartColor;
        [SerializeField] private Color textEndColor;

        [Header("Destroyed Colors")]
        [SerializeField] private Color destroyedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private Color destroyedTextColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        public HourMarkerPhase Phase { get; private set; } = HourMarkerPhase.Rising;

        private static readonly (int Value, string Numeral)[] RomanNumerals =
        {
            (1000, "M"), (900, "CM"), (500, "D"), (400, "CD"),
            (100, "C"), (90, "XC"), (50, "L"), (40, "XL"),
            (10, "X"), (9, "IX"), (5, "V"), (4, "IV"), (1, "I"),
        };

        private void Awake()
        {
            hourText.text = ToRomanNumeral(hour);
        }

        private void Update()
        {
            if (Phase == HourMarkerPhase.Destroyed) return;

            GameState gameState = GameState.Instance;
            var lerp = gameState.GetTargetHourProgress(hour);

            if (lerp >= 1f && Phase == HourMarkerPhase.Rising)
                Phase = HourMarkerPhase.Active;

            if (lerp < 1f && Phase == HourMarkerPhase.Active)
                Phase = HourMarkerPhase.Rising;

            sprite.color = Color.Lerp(startColor, endColor, lerp);
            hourText.color = Color.Lerp(textStartColor, textEndColor, lerp);
        }

        public void OnDestroyed()
        {
            Phase = HourMarkerPhase.Destroyed;
            sprite.color = destroyedColor;
            hourText.color = destroyedTextColor;
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
    }
}
