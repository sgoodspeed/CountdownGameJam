using TMPro;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Self-bootstrapping FPS overlay. Spawns itself before the first scene loads so it
    /// doesn't need to be wired into any scene, and persists across scene loads.
    /// </summary>
    public class FpsCounter : MonoBehaviour
    {
        private const float SampleInterval = 0.5f;
        private const float GreenThreshold = 60f;
        private const float YellowThreshold = 30f;

        [SerializeField] private TextMeshProUGUI label;
        
        private float elapsed;
        private int frames;

        private void Update()
        {
            elapsed += Time.unscaledDeltaTime;
            frames++;

            if (elapsed < SampleInterval)
            {
                return;
            }

            float fps = frames / elapsed;
            elapsed = 0f;
            frames = 0;

            label.text = $"{fps:0} FPS";
            label.color = fps > GreenThreshold ? Color.green
                : fps > YellowThreshold ? Color.yellow
                : Color.red;
        }
    }
}
