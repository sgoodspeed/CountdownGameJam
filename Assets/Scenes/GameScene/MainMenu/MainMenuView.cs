using System;
using UnityEngine;
using UnityEngine.UI;

namespace Countdown
{
    /// <summary>
    /// View for the Main Menu. Only touches UI widgets and raises events for user input -
    /// no game logic lives here.
    /// </summary>
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        public event Action PlayButtonClicked;
        public event Action QuitButtonClicked;

        private void Awake()
        {
            playButton.onClick.AddListener(HandlePlayClicked);
            quitButton.onClick.AddListener(HandleQuitClicked);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(HandlePlayClicked);
            quitButton.onClick.RemoveListener(HandleQuitClicked);
        }

        private void HandlePlayClicked() => PlayButtonClicked?.Invoke();
        private void HandleQuitClicked() => QuitButtonClicked?.Invoke();
    }
}
