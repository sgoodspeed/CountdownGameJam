using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Model for the Game HUD. Holds the countdown duration for a round.
    /// </summary>
    public class GameHudModel : MonoBehaviour
    {
        [SerializeField] private float gameDuration = 60f;

        public float GameDuration => gameDuration;
    }
}
