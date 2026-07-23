using System;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Presenter for the Game HUD. Drives the countdown timer each frame and pushes the
    /// remaining time to the view. Raises TimeExpired once the countdown reaches zero.
    /// </summary>
    public class GameHudPresenter : MonoBehaviour
    {
        [SerializeField] private GameHudModel model;
        [SerializeField] private GameHudView view;

        private float remainingSeconds;
        private bool countdownFinished;

        public event Action TimeExpired;

        public void Show()
        {
            gameObject.SetActive(true);
            ResetTimer();
        }

        public void Hide() => gameObject.SetActive(false);

        private void ResetTimer()
        {
            remainingSeconds = model.GameDuration;
            countdownFinished = false;
            view.SetTime(remainingSeconds);
        }

        private void Update()
        {
            if (countdownFinished)
            {
                return;
            }

            remainingSeconds -= Time.deltaTime;

            if (remainingSeconds <= 0f)
            {
                remainingSeconds = 0f;
                countdownFinished = true;
                view.SetTime(remainingSeconds);
                TimeExpired?.Invoke();
                return;
            }

            view.SetTime(remainingSeconds);
        }
    }
}
