using UnityEngine;
using UnityEngine.UI;

namespace Countdown
{
    /// <summary>
    /// View for the Game HUD. Only touches UI widgets - no timing logic lives here.
    /// </summary>
    public class GameHudView : MonoBehaviour
    {
        [SerializeField] private Text timeLabel;

        public void SetTime(float secondsRemaining)
        {
            int wholeSeconds = Mathf.CeilToInt(Mathf.Max(0f, secondsRemaining));
            timeLabel.text = wholeSeconds.ToString();
        }
    }
}
