using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Convenience accessors for reading clock-style time out of a GameState's
    /// normalized 12-hour cycle (0-1 = start through the end of the half day).
    /// </summary>
    public static class GameStateExtensions
    {
        private const int HoursPerCycle = 12;
        private const int MinutesPerHour = 60;

        public static float GetNormalizedTime(this GameState gameState)
        {
            return gameState.NormalizedTime;
        }

        /// <summary>Current hour index, 0-11.</summary>
        public static int GetCurrentHour(this GameState gameState)
        {
            float totalHours = gameState.GetNormalizedTime() * HoursPerCycle;
            return Mathf.Clamp(Mathf.FloorToInt(totalHours), 0, HoursPerCycle - 1);
        }

        /// <summary>Current minute within the current hour, 0-59.</summary>
        public static int GetCurrentMinute(this GameState gameState)
        {
            float progress = gameState.GetCurrentHourProgress();
            return Mathf.Clamp(Mathf.FloorToInt(progress * MinutesPerHour), 0, MinutesPerHour - 1);
        }

        /// <summary>How far through the current hour we are, 0-1.</summary>
        public static float GetCurrentHourProgress(this GameState gameState)
        {
            float totalHours = gameState.GetNormalizedTime() * HoursPerCycle;
            int hour = gameState.GetCurrentHour();
            return Mathf.Clamp01(totalHours - hour);
        }
    }
}
