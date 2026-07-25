using System;
using UnityEngine;

namespace Countdown
{
    public class GameState : Singleton<GameState>
    {
        private const float HoursPerCycle = 12f;
        private const float MinutesPerCycle = HoursPerCycle * 60f;

        public float MaxTime { get; private set; }
        public float CurrentTime { get; private set; }
        public GameClockController GameClock { get; private set; }

        public float NormalizedTime => MaxTime > 0f ? Mathf.Clamp01(CurrentTime / MaxTime) : 0f;

        public event Action ClockRanOut;

        protected override void Awake()
        {
            base.Awake();
            GameClock = new GameClockController();
            GameClock.TimeExpired += () => ClockRanOut?.Invoke();
        }

        private void Update()
        {
            if (GameClock == null) return;
            GameClock.Tick(Time.deltaTime);
            if (GameClock.GameDuration > 0f)
            {
                MaxTime = GameClock.GameDuration;
                CurrentTime = GameClock.GameDuration - GameClock.RemainingSeconds;
            }
        }

        private void OnDestroy()
        {
            GameClock?.Stop();
        }

        public void SetMaxTime(float maxTime) => MaxTime = maxTime;
        public void SetCurrentTime(float currentTime) => CurrentTime = currentTime;

        public void AddMinutes(float minutes) => AddNormalizedTime(minutes / MinutesPerCycle);
        public void AddHours(float hours) => AddNormalizedTime(hours / HoursPerCycle);

        private void AddNormalizedTime(float normalizedDelta)
        {
            if (MaxTime <= 0f) return;
            CurrentTime = Mathf.Clamp(CurrentTime + normalizedDelta * MaxTime, 0f, MaxTime);
        }
    }
}
