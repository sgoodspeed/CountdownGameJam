using UnityEngine;
using UnityEngine.SceneManagement;

namespace Countdown
{
    /// <summary>
    /// Main script for GameScene.
    /// Owns menu navigation: shows the Main Menu on start, switches to the Game HUD
    /// when Play is pressed, switches to the Game Over screen when the HUD countdown
    /// finishes, and handles Again/Quit from there.
    /// </summary>
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField] private MainMenuPresenter mainMenuPresenter;
        [SerializeField] private GameHudPresenter gameHudPresenter;
        [SerializeField] private GameOverPresenter gameOverPresenter;

        private void Awake()
        {
            mainMenuPresenter.PlayRequested += HandlePlayRequested;
            mainMenuPresenter.QuitRequested += HandleQuitRequested;
            gameHudPresenter.TimeExpired += HandleTimeExpired;
            gameOverPresenter.AgainRequested += HandleAgainRequested;
            gameOverPresenter.QuitRequested += HandleQuitRequested;
        }

        private void OnDestroy()
        {
            mainMenuPresenter.PlayRequested -= HandlePlayRequested;
            mainMenuPresenter.QuitRequested -= HandleQuitRequested;
            gameHudPresenter.TimeExpired -= HandleTimeExpired;
            gameOverPresenter.AgainRequested -= HandleAgainRequested;
            gameOverPresenter.QuitRequested -= HandleQuitRequested;
        }

        private void Start()
        {
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            mainMenuPresenter.Show();
            gameHudPresenter.Hide();
            gameOverPresenter.Hide();
        }

        private void HandlePlayRequested()
        {
            mainMenuPresenter.Hide();
            gameHudPresenter.Show();
        }

        private void HandleTimeExpired()
        {
            gameHudPresenter.Hide();
            gameOverPresenter.Show();
        }

        private void HandleAgainRequested()
        {
            SceneManager.LoadScene(SceneNames.Loading);
        }

        private void HandleQuitRequested()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
