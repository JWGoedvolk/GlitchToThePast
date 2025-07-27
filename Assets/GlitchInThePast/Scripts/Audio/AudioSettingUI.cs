using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class AudioSettingsUI : MonoBehaviour
    {
        #region Variables
        public Slider masterSlider;
        public Slider musicSlider;
        // public Slider sfxSlider;
        public Slider voiceSlider;
        #endregion

        private void Start()
        {
            masterSlider.value = AudioManager.Instance.GetVolume(AudioManager.Instance.masterVolumeParam);
            musicSlider.value = AudioManager.Instance.GetVolume(AudioManager.Instance.musicVolumeParam);
            // sfxSlider.value = AudioManager.Instance.GetVolume(AudioManager.Instance.sfxVolumeParam);
            voiceSlider.value = AudioManager.Instance.GetVolume(AudioManager.Instance.voiceVolumeParam);

            masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetTheMasterVolume);
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetTheMusicVolume);
            // sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetTheSFXVolume);
            voiceSlider.onValueChanged.AddListener(AudioManager.Instance.SetTheVoiceVolume);
        }
    }
}