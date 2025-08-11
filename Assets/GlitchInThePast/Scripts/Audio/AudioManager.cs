using UnityEngine;
using UnityEngine.Audio;

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

        private AudioSource musicSource;
        private AudioSource sfxBus;
        private AudioSource voiceBus;
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

            musicSource = MakeBus("MusicBus", musicGroup);
            sfxBus = MakeBus("SFXBus", sfxGroup);
            voiceBus = MakeBus("VoiceBus", voiceGroup);
        }

        private AudioSource MakeBus(string name, AudioMixerGroup group)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(transform);

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;

            if (group != null) audioSource.outputAudioMixerGroup = group;
            return audioSource;
        }

        #region Public Functions
        public void SetTheMasterVolume(float value) => SetVolume(masterVolumeParam, value);
        public void SetTheMusicVolume(float value) => SetVolume(musicVolumeParam, value);
        public void SetTheSFXVolume(float value) => SetVolume(sfxVolumeParam, value);
        public void SetTheVoiceVolume(float value) => SetVolume(voiceVolumeParam, value);

        private void SetVolume(string parameter, float sliderValue)
        {
            float db = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
            masterMixer.SetFloat(parameter, db);
        }

        public float GetVolume(string parameter)
        {
            if (masterMixer.GetFloat(parameter, out float value))
            {
                return Mathf.Pow(10f, value / 20f);
            }
            return 1f;
        }

        public void PlayMusic(AudioClip clip, bool loop = true, float fadeSeconds = 0.25f, bool restartIfSame = false)
        {
            if (clip == null) return;

            if (!restartIfSame && musicSource.clip == clip && musicSource.isPlaying) return;

            StopAllCoroutines();
            StartCoroutine(FadeSwap(musicSource, clip, loop, fadeSeconds));
        }

        public void StopMusic(float fadeSeconds = 0.25f)
        {
            if (!musicSource.isPlaying) return;
            StopAllCoroutines();
            StartCoroutine(FadeOut(musicSource, fadeSeconds));
        }

        public void PlaySFXOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            sfxBus.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void PlayVoiceOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            voiceBus.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
        #endregion

        #region Private Functions
        private System.Collections.IEnumerator FadeSwap(AudioSource audioSource, AudioClip next, bool loop, float providedTime)
        {
            float startVol = audioSource.volume;
            float time = 0f;
            while (audioSource.isPlaying && time < providedTime)
            {
                time += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVol, 0f, providedTime <= 0f ? 1f : time / providedTime);
                yield return null;
            }

            audioSource.Stop();
            audioSource.clip = next;
            audioSource.loop = loop;
            audioSource.volume = 0f;
            audioSource.Play();

            time = 0f;
            while (time < providedTime)
            {
                time += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(0f, 1f, providedTime <= 0f ? 1f : time / providedTime);
                yield return null;
            }
            audioSource.volume = 1f;
        }

        private System.Collections.IEnumerator FadeOut(AudioSource audioSource, float providedTime)
        {
            float startVolume = audioSource.volume;
            float time = 0f;
            while (time < providedTime)
            {
                time += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, providedTime <= 0f ? 1f : time / providedTime);
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = startVolume;
        }
        #endregion
    }
}