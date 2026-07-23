using UnityEngine;
using UnityEngine.SceneManagement;

namespace Countdown
{
    /// <summary>
    /// Main script for LoadingScene.
    /// This scene's job is to reset the experience back to a clean state before
    /// handing off to GameScene.
    /// </summary>
    public class LoadingSceneManager : MonoBehaviour
    {
        private void Start()
        {
            ResetExperience();
            SceneManager.LoadScene(SceneNames.Game);
        }

        /// <summary>
        /// Reset any persistent/static state (managers, timers, player data, etc.)
        /// before loading into GameScene. Placeholder until those systems exist.
        /// </summary>
        private void ResetExperience()
        {
            // TODO: reset persistent systems as they're added.
        }
    }
}
