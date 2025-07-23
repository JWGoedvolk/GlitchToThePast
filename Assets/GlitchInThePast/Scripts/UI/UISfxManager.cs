using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISfxManager : MonoBehaviour
{
    [Header("Audio source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Button Soudn Effects")]
    [SerializeField] private AudioClip buttonCick;
    [SerializeField] private AudioClip buttonClickExit;
    [SerializeField] private AudioClip panelOpen;
    [SerializeField] private AudioClip pannelClose;


    #region Functions
    public void PlayButtonClickSFX()
    {
        PlayTheSoundEffectOfThe(buttonCick);
    }

    public void PlayExitSFX()
    {
        PlayTheSoundEffectOfThe(buttonClickExit);
    }

    public void PlayPannelOpeningSFX()
    {
        PlayTheSoundEffectOfThe(panelOpen);
    }

    public void PlayPannelClosingSFX()
    {
        PlayTheSoundEffectOfThe(pannelClose);
    }

    #endregion

    private void PlayTheSoundEffectOfThe (AudioClip clip)
    {
        if (clip != null && sfxSource != null)
    {
        // Debug.Log("Playing SFX: " + clip.name);
        sfxSource.PlayOneShot(clip);
    }
    else
    {
        if (clip == null) Debug.LogWarning("Clip is missing");
        if (sfxSource == null) Debug.LogWarning("SFX AudioSource is also missing");
    }
    }

}
