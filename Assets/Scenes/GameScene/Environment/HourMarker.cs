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
        
        [SerializeField] private Color textStartColor;
        [SerializeField] private Color textEndColor;

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
            UpdateColors();
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

        private void UpdateColors()
        {
            GameState gameState = GameState.Instance;
            var lerp = gameState.GetTargetHourProgress(hour);
            sprite.color = Color.Lerp(startColor, endColor, lerp);
            hourText.color = Color.Lerp(textStartColor, textEndColor, lerp);
        }
    }
}