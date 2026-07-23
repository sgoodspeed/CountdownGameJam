using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Holds shared round state - currently the countdown's max and current time -
    /// accessible from anywhere via Singleton&lt;GameState&gt;.Instance.
    /// </summary>
    public class GameState : Singleton<GameState>
    {
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
    }
}
