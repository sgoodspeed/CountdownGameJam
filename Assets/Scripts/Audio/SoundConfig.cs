using UnityEngine;

namespace Countdown
{
    [CreateAssetMenu(fileName = "New SoundConfig", menuName = "Countdown/Sound Config")]
    public class SoundConfig : ScriptableObject
    {
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitchMin = 1f;
        [Range(0.5f, 2f)] public float pitchMax = 1f;
    }
}
