using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public List<AudioClip> hitSFX = new List<AudioClip>();
    public AudioClip deathSFX;
    public AudioSource audioSource;

    public List<AudioClip> rangedSFX = new List<AudioClip>();

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log($"SFXManager: Added AudioSource component to {gameObject.name}");
        }
        else
        {
            Debug.Log($"SFXManager: Found existing AudioSource on {gameObject.name}");
        }

        audioSource.playOnAwake = false;

    }

    #region player related sfx
    public void PlayHitSFX()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is missing");
            return;
        }

        if (hitSFX == null || hitSFX.Count == 0)
        {
            Debug.LogWarning(" No hit SFX ");
            return;
        }

        //randomized on diffrent pitches - once tasks finshed
        AudioClip clipToPlay = null;
        if (hitSFX.Count == 1)
        {
            clipToPlay = hitSFX[0];
        }
        else
        {
            int randomIndex = Random.Range(0, hitSFX.Count);
            clipToPlay = hitSFX[randomIndex];
        }

        if (clipToPlay == null)
        {
            Debug.LogWarning("SFXManage missing ");
            return;
        }

       
        audioSource.PlayOneShot(clipToPlay);
    }

    public void PlayDeathSFX()
    {
        if (audioSource == null)
        {
            return;
        }

        if (deathSFX == null)
        {
            return;
        }

        audioSource.PlayOneShot(deathSFX);
    }
    #endregion

    #region wepaons related sfx

    public void PlayRangedWeaponSFX()
    {
        if (audioSource == null)
        {
            Debug.LogError("audio nada");

            return;
        }

        if(rangedSFX == null || rangedSFX.Count == 0)
        {
            Debug.Log("no sfx attached");
            return;
        }



        int randomIndex = Random.Range(0, rangedSFX.Count);
        AudioClip clipPlay = rangedSFX[randomIndex];

        audioSource.PlayOneShot(clipPlay);
    }

    #endregion
}


