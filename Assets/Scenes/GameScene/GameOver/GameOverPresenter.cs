using System;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Presenter for the Game Over screen. Mediates between GameOverView (UI) and
    /// GameOverModel (data), and exposes app-level events for GameSceneManager to react to.
    /// </summary>
    public class GameOverPresenter : MonoBehaviour
    {
        [SerializeField] private GameOverModel model;
        [SerializeField] private GameOverView view;

        public event Action AgainRequested;
        public event Action QuitRequested;

        private void OnEnable()
        {
            view.AgainButtonClicked += HandleAgainButtonClicked;
            view.QuitButtonClicked += HandleQuitButtonClicked;
        }

        private void OnDisable()
        {
            view.AgainButtonClicked -= HandleAgainButtonClicked;
            view.QuitButtonClicked -= HandleQuitButtonClicked;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void HandleAgainButtonClicked() => AgainRequested?.Invoke();
        private void HandleQuitButtonClicked() => QuitRequested?.Invoke();
    }
}
