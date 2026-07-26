using System.Collections;
using UnityEngine;

namespace Countdown
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Pool")]
        [SerializeField] private int sfxSourceCount = 8;

        [Header("Music")]
        [SerializeField] private float defaultMusicVolume = 0.5f;

        private AudioSource _musicSource;
        private AudioSource[] _sfxSources;
        private int _nextSource;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            _musicSource = CreateSource("Music");
            _musicSource.loop = true;
            _musicSource.spatialBlend = 0f;

            _sfxSources = new AudioSource[sfxSourceCount];
            for (int i = 0; i < sfxSourceCount; i++)
            {
                _sfxSources[i] = CreateSource($"SFX_{i}");
                _sfxSources[i].spatialBlend = 0f;
            }
        }

        private AudioSource CreateSource(string label)
        {
            var go = new GameObject(label);
            go.transform.SetParent(transform);
            return go.AddComponent<AudioSource>();
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
        {
            if (_musicSource.isPlaying && _musicSource.clip == clip) return;
            StopAllCoroutines();
            StartCoroutine(CrossfadeMusic(clip, fadeDuration));
        }

        public void StopMusic(float fadeDuration = 1f)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut(fadeDuration));
        }

        public void SetMusicVolume(float volume)
        {
            defaultMusicVolume = volume;
            _musicSource.volume = volume;
        }

        public void Play(SoundConfig config)
        {
            if (config == null || config.clips == null || config.clips.Length == 0) return;

            var source = _sfxSources[_nextSource];
            _nextSource = (_nextSource + 1) % _sfxSources.Length;

            var clip = config.clips[Random.Range(0, config.clips.Length)];
            source.clip = clip;
            source.volume = config.volume;
            source.pitch = Random.Range(config.pitchMin, config.pitchMax);
            source.Play();
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
        {
            if (_musicSource.isPlaying)
            {
                yield return FadeOut(duration * 0.5f);
            }

            _musicSource.clip = newClip;
            _musicSource.volume = 0f;
            _musicSource.Play();

            float elapsed = 0f;
            float halfDuration = duration * 0.5f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _musicSource.volume = Mathf.Lerp(0f, defaultMusicVolume, elapsed / halfDuration);
                yield return null;
            }
            _musicSource.volume = defaultMusicVolume;
        }

        private IEnumerator FadeOut(float duration)
        {
            float startVolume = _musicSource.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
            _musicSource.volume = 0f;
            _musicSource.Stop();
        }
    }
}
