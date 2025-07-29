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

        if (audioSource.volume <= 0f)
        {
            Debug.LogWarning("SFXManager: AudioSource volume is 0 or muted!");
        }
    }

    public void PlayHitSFX()
    {
        if (audioSource == null)
        {
            Debug.LogError("SFXManager: AudioSource is missing! Cannot play hit SFX.");
            return;
        }

        if (hitSFX == null || hitSFX.Count == 0)
        {
            Debug.LogWarning("SFXManager: No hit SFX assigned!");
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
            Debug.LogWarning("SFXManager: Selected hit SFX clip is null!");
            return;
        }

        Debug.Log($"SFXManager: Playing hit SFX: {clipToPlay.name}");
        audioSource.PlayOneShot(clipToPlay);
    }

    public void PlayDeathSFX()
    {
        if (audioSource == null)
        {
            Debug.LogError("SFXManager: AudioSource is missing! Cannot play death SFX.");
            return;
        }

        if (deathSFX == null)
        {
            Debug.LogWarning("SFXManager: No death SFX assigned!");
            return;
        }

        Debug.Log($"SFXManager: Playing death SFX: {deathSFX.name}");
        audioSource.PlayOneShot(deathSFX);
    }
}


