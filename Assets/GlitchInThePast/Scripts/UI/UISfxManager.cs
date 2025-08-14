using UnityEngine;

public class UISfxManager : MonoBehaviour
{
    public AudioClip navigateClip;

    [Header("Audio source")]
    [SerializeField] private AudioSource sfxSource; // kept for backwards compat but not required

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

    private void PlayTheSoundEffectOfThe(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Clip is missing");
            return;
        }

        // Prefer the central AudioManager. Fall back to local source if present.
        var mgr = Audio.AudioManager.Instance;
        if (mgr != null)
        {
            mgr.PlayUiOneShot(clip);
            return;
        }

        if (sfxSource != null)
        {
            // Debug.Log("Playing SFX: " + clip.name);
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX AudioSource is also missing");
        }
    }
}
