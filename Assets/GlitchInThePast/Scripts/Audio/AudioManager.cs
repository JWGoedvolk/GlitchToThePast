using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Variables
        public static AudioManager Instance;

        [Header("Audio Mixer")]
        public AudioMixer masterMixer;

        [Header("Exposed Parameters")]
        public string masterVolumeParam = "MasterVolume";
        public string musicVolumeParam = "MusicVolume";
        public string sfxVolumeParam = "SFXVolume";
        public string voiceVolumeParam = "VoiceVolume";

        [Header("Mixer Groups")]
        [SerializeField] private AudioMixerGroup masterGroup;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup voiceGroup;

        public AudioSource Music { get; private set; }
        public AudioSource Sfx { get; private set; }
        public AudioSource Voice { get; private set; }

        [Range(0f, 3f)] public float defaultFadeSeconds = 0.25f;
        private Coroutine _musicFade;

        private const string TAG_MUSIC = "Music";
        private const string TAG_SFX = "SFX";
        private const string TAG_VOICE = "Voice";
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            BindSceneEndpoints();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #region Public Functions
        /// <summary>
        /// Clears the cached references and searches current scene by tag.
        /// Applies mixer groups if a source has none.
        /// </summary>
        public void BindSceneEndpoints()
        {
            ClearCachedAudioVariables();

            Music = FindAudioSourceByTag(TAG_MUSIC);
            Sfx = FindAudioSourceByTag(TAG_SFX);
            Voice = FindAudioSourceByTag(TAG_VOICE);

            ApplyMixerGroupIfEmpty(Music, musicGroup);
            ApplyMixerGroupIfEmpty(Sfx, sfxGroup);
            ApplyMixerGroupIfEmpty(Voice, voiceGroup);
        }

        public void ClearCachedAudioVariables()
        {
            if (_musicFade != null) StopCoroutine(_musicFade);
            _musicFade = null;
            Music = Sfx = Voice = null;
        }

        #region Volume Getting and Setting
        public void SetTheMasterVolume(float value) => SetVolume(masterVolumeParam, value);
        public void SetTheMusicVolume(float value) => SetVolume(musicVolumeParam, value);
        public void SetTheSFXVolume(float value) => SetVolume(sfxVolumeParam, value);
        public void SetTheVoiceVolume(float value) => SetVolume(voiceVolumeParam, value);

        public float GetVolume(string parameter)
        {
            if (masterMixer != null && masterMixer.GetFloat(parameter, out float value))
                return Mathf.Pow(10f, value / 20f);
            return 1f;
        }
        #endregion

        #region Playing Music Functions
        /// <summary>
        /// Crossfades the scene Music source to 'clip'.
        /// </summary>
        public void PlayMusic(AudioClip clip, bool loop = true, float fadeSeconds = -1f, bool restartIfSame = false)
        {
            if (Music == null || clip == null) return;
            if (!restartIfSame && Music.clip == clip && Music.isPlaying) return;

            if (_musicFade != null) StopCoroutine(_musicFade);
            if (fadeSeconds < 0f) fadeSeconds = defaultFadeSeconds;
            _musicFade = StartCoroutine(FadeSwap(Music, clip, loop, fadeSeconds));
        }

        public void StopMusic(float fadeSeconds = -1f)
        {
            if (Music == null || !Music.isPlaying) return;
            if (_musicFade != null) StopCoroutine(_musicFade);
            if (fadeSeconds < 0f) fadeSeconds = defaultFadeSeconds;
            _musicFade = StartCoroutine(FadeOut(Music, fadeSeconds));
        }

        public void PlaySFXOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            var audioSource = Sfx ?? Music ?? Voice;
            if (audioSource != null) audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void PlayUiOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            var audioSource = Sfx ?? Music;
            if (audioSource != null) audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void PlayVoiceOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            var audioSource = Voice ?? Sfx ?? Music;
            if (audioSource != null) audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
        #endregion

        #endregion

        #region Private Functions

        private void SetVolume(string parameter, float sliderValue)
        {
            if (masterMixer == null || string.IsNullOrEmpty(parameter)) return;
            float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
            masterMixer.SetFloat(parameter, db);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            BindSceneEndpoints();
        }

        private static AudioSource FindAudioSourceByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return null;
            try
            {
                var gameObject = GameObject.FindGameObjectWithTag(tag);
                return gameObject != null ? gameObject.GetComponent<AudioSource>() : null;
            }
            catch
            {
                return null;
            }
        }

        private static void ApplyMixerGroupIfEmpty(AudioSource src, AudioMixerGroup group)
        {
            if (src != null && group != null && src.outputAudioMixerGroup == null)
                src.outputAudioMixerGroup = group;
        }

        #region Music Fading
        private System.Collections.IEnumerator FadeSwap(AudioSource audioSource, AudioClip next, bool loop, float seconds)
        {
            float startVolume = audioSource.volume;
            float timee = 0f;
            while (audioSource.isPlaying && timee < seconds)
            {
                timee += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, seconds <= 0f ? 1f : timee / seconds);
                yield return null;
            }

            audioSource.Stop();
            audioSource.clip = next;
            audioSource.loop = loop;
            audioSource.volume = 0f;
            audioSource.Play();

            timee = 0f;
            while (timee < seconds)
            {
                timee += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(0f, startVolume, seconds <= 0f ? 1f : timee / seconds);
                yield return null;
            }
            audioSource.volume = startVolume;
        }

        private System.Collections.IEnumerator FadeOut(AudioSource audioSource, float seconds)
        {
            float startVolume = audioSource.volume;
            float timee = 0f;
            while (timee < seconds)
            {
                timee += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, seconds <= 0f ? 1f : timee / seconds);
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = startVolume;
        }
        #endregion
        #endregion
    }
}