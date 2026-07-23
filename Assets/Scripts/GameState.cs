using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Holds shared round state - currently the countdown's max and current time -
    /// accessible from anywhere via Singleton&lt;GameState&gt;.Instance.
    /// </summary>
    public class GameState : Singleton<GameState>
    {
        private const float HoursPerCycle = 12f;
        private const float MinutesPerCycle = HoursPerCycle * 60f;

        public float MaxTime { get; private set; }
        public float CurrentTime { get; private set; }

        public float NormalizedTime => MaxTime > 0f ? Mathf.Clamp01(CurrentTime / MaxTime) : 0f;

        public void SetMaxTime(float maxTime)
        {
            MaxTime = maxTime;
        }

        public void SetCurrentTime(float currentTime)
        {
            CurrentTime = currentTime;
        }

        /// <summary>Moves the clock forward (positive) or backward (negative) by a number of in-game minutes.</summary>
        public void AddMinutes(float minutes)
        {
            AddNormalizedTime(minutes / MinutesPerCycle);
        }

        /// <summary>Moves the clock forward (positive) or backward (negative) by a number of in-game hours.</summary>
        public void AddHours(float hours)
        {
            AddNormalizedTime(hours / HoursPerCycle);
        }

        private void AddNormalizedTime(float normalizedDelta)
        {
            if (MaxTime <= 0f)
            {
                return;
            }

            CurrentTime = Mathf.Clamp(CurrentTime + normalizedDelta * MaxTime, 0f, MaxTime);
        }
    }
}
