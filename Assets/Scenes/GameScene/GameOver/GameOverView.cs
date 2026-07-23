using System;
using UnityEngine;
using UnityEngine.UI;

namespace Countdown
{
    /// <summary>
    /// View for the Game Over screen. Only touches UI widgets and raises events for user input.
    /// </summary>
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private Button againButton;
        [SerializeField] private Button quitButton;

        public event Action AgainButtonClicked;
        public event Action QuitButtonClicked;

        private void Awake()
        {
            againButton.onClick.AddListener(HandleAgainClicked);
            quitButton.onClick.AddListener(HandleQuitClicked);
        }

        private void OnDestroy()
        {
            againButton.onClick.RemoveListener(HandleAgainClicked);
            quitButton.onClick.RemoveListener(HandleQuitClicked);
        }

        private void HandleAgainClicked() => AgainButtonClicked?.Invoke();
        private void HandleQuitClicked() => QuitButtonClicked?.Invoke();
    }
}
