using UnityEngine;

namespace Countdown
{
    public class HourMarker : MonoBehaviour
    {
        [SerializeField] private int hour;
        [SerializeField] private SpriteRenderer sprite;
        
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

        private void Update()
        {
            UpdateAlpha();
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