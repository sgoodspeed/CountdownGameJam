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

        [Header("Gameplay Behaviours (disabled during menu/game-over)")]
        [SerializeField] private CharacterMovement2D playerMovement;
        [SerializeField] private CharacterMelee2D playerMelee;
        [SerializeField] private EnemySpawner2D enemySpawner;

        private void Awake()
        {
            mainMenuPresenter.PlayRequested += HandlePlayRequested;
            mainMenuPresenter.QuitRequested += HandleQuitRequested;
            GameState.Instance.ClockRanOut += HandleTimeExpired;
            gameOverPresenter.AgainRequested += HandleAgainRequested;
            gameOverPresenter.QuitRequested += HandleQuitRequested;
        }

        private void OnDestroy()
        {
            mainMenuPresenter.PlayRequested -= HandlePlayRequested;
            mainMenuPresenter.QuitRequested -= HandleQuitRequested;
            if (GameState.Instance != null)
                GameState.Instance.ClockRanOut -= HandleTimeExpired;
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
            SetGameplayActive(false, GamePhase.Menu);
        }

        private void HandlePlayRequested()
        {
            mainMenuPresenter.Hide();
            gameHudPresenter.Show();
            SetGameplayActive(true, GamePhase.Playing);
        }

        private void HandleTimeExpired()
        {
            gameHudPresenter.Hide();
            gameOverPresenter.Show();
            SetGameplayActive(false, GamePhase.GameOver);
        }

        private void SetGameplayActive(bool active, GamePhase phase)
        {
            GameState.Instance.SetPhase(phase);

            if (playerMovement != null) playerMovement.enabled = active;
            if (playerMelee != null) playerMelee.enabled = active;

            if (enemySpawner != null)
            {
                if (active)
                    enemySpawner.StartSpawning();
                else
                    enemySpawner.StopSpawning();
            }
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
