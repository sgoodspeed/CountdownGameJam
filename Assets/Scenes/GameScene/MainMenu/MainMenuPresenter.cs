using System;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Presenter for the Main Menu. Mediates between MainMenuView (UI) and MainMenuModel (data),
    /// and exposes app-level events for GameSceneManager to react to.
    /// </summary>
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private MainMenuModel model;
        [SerializeField] private MainMenuView view;

        public event Action PlayRequested;
        public event Action QuitRequested;

        private void OnEnable()
        {
            view.PlayButtonClicked += HandlePlayButtonClicked;
            view.QuitButtonClicked += HandleQuitButtonClicked;
        }

        private void OnDisable()
        {
            view.PlayButtonClicked -= HandlePlayButtonClicked;
            view.QuitButtonClicked -= HandleQuitButtonClicked;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void HandlePlayButtonClicked() => PlayRequested?.Invoke();
        private void HandleQuitButtonClicked() => QuitRequested?.Invoke();
    }
}
