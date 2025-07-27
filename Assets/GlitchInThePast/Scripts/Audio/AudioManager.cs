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
        // public string sfxVolumeParam = "SFXVolume";
        public string voiceVolumeParam = "VoiceVolume";
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
        }

        #region Public Functions
        public void SetTheMasterVolume(float value) => SetVolume(masterVolumeParam, value);
        public void SetTheMusicVolume(float value) => SetVolume(musicVolumeParam, value);
        // public void SetTheSFXVolume(float value) => SetVolume(sfxVolumeParam, value);
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
        #endregion
    }
}