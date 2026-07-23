using UnityEngine;
using UnityEngine.SceneManagement;

namespace Countdown
{
    /// <summary>
    /// Main script for InitialScene.
    /// This scene exists purely to load as fast as possible (solid black background,
    /// empty or very light UI such as a logo) and then immediately hand off to LoadingScene.
    /// </summary>
    public class InitialSceneManager : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene(SceneNames.Loading);
        }
    }
}
