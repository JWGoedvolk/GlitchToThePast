using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public List<AudioClip> hitSFX = new List<AudioClip>();
    public AudioClip deathSFX;
    public AudioSource audioSource;

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
}


