using System;
using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    public class GameClockController
    {
        private const float HoursPerCycle = 12f;

        private float gameDuration;
        private float remainingSeconds;
        private bool running;
        private Tween damageTween;

        public float RemainingSeconds => remainingSeconds;
        public float GameDuration => gameDuration;

        public event Action TimeExpired;

        public void Start(float duration)
        {
            gameDuration = duration;
            remainingSeconds = duration;
            running = true;
        }

        public void Stop()
        {
            running = false;
            damageTween?.Kill();
            damageTween = null;
        }

        public void Tick(float deltaTime)
        {
            if (!running) return;
            if (damageTween != null && damageTween.IsActive() && damageTween.IsPlaying()) return;

            remainingSeconds -= deltaTime;
            CheckExpired();
        }

        public void SetHoursInternal(float hour, float lerpDuration)
        {
            if (!running) return;
            
            damageTween?.Kill();
            
            hour = Mathf.Min(gameDuration, hour);
            damageTween = DOTween.To(
                () => remainingSeconds,
                x => remainingSeconds = x,
                hour,
                lerpDuration
            ).OnComplete(() =>
            {
                damageTween = null;
                CheckExpired();
            });
        }

        public void SetHoursRemaining(float hours, float lerpDuration)
        {
            float secondsPerHour = gameDuration / HoursPerCycle;
            float target = Mathf.Max(0f, hours * secondsPerHour); 
            
            SetHoursInternal(target, lerpDuration);
        }

        public void AddHours(float hours, float lerpDuration)
        {
            float secondsPerHour = gameDuration / HoursPerCycle;
            float target = Mathf.Max(0f, remainingSeconds + hours * secondsPerHour); 
            
            SetHoursInternal(target, lerpDuration);
        }

        private void CheckExpired()
        {
            if (remainingSeconds > 0f) return;
            remainingSeconds = 0f;
            running = false;
            TimeExpired?.Invoke();
        }
    }
}
