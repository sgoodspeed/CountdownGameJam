using TMPro;
using UnityEngine;

namespace Countdown
{
    public class HourMarker : MonoBehaviour
    {
        [SerializeField] private int hour;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TMP_Text hourText;

        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

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
            UpdateAlpha();
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

        private void UpdateAlpha()
        {
            GameState gameState = GameState.Instance;
            int currentHour = gameState.GetCurrentHour() + 1;

            float alpha;
            if (currentHour > hour)
            {
                alpha = 1f;
            }
            else if (currentHour < hour)
            {
                alpha = 0f;
            }
            else
            {
                alpha = gameState.GetCurrentHourProgress();
            }

            sprite.color = Color.Lerp(startColor, endColor, alpha);
        }
    }
}