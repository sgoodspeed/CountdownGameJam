using UnityEngine;

namespace Countdown
{
    public class ClockFace : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float normalizedPosition;
        
        [SerializeField] private Transform hourHand;
        [SerializeField] private Transform minuteHand;
        
        private void Update()
        {
            UpdateClockPositions();
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateClockPositions();
        }
        #endif

        private void UpdateClockPositions()
        {
            // normalizedPosition spans one half-day (12h): hour hand does 1 rotation,
            // minute hand does 12 (1 per hour), matching a real analog clock face.
            float hourAngle = normalizedPosition * 360f;
            float minuteAngle = normalizedPosition * 12f * 360f;

            if (hourHand != null)
            {
                hourHand.localRotation = Quaternion.Euler(0f, 0f, -hourAngle);
            }

            if (minuteHand != null)
            {
                minuteHand.localRotation = Quaternion.Euler(0f, 0f, -minuteAngle);
            }
        }
    }
}