using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Circular play-area boundary. Builds its own visual at runtime and exposes
    /// ClampInside so other objects (e.g. the player character) can be kept within its radius.
    /// </summary>
    [DisallowMultipleComponent]
    public class CircleBoundary : MonoBehaviour
    {
    }
}
