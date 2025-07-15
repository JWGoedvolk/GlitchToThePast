using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.FadingEffect.Temp
{
    public class MaterialStateChanger : MonoBehaviour
    {
        [Tooltip("Material to switch to during the flash.")]
        [SerializeField] private Material flashMaterial;

        [Tooltip("Duration of the flash.")]
        [SerializeField] private float duration;
        
        [SerializeField] private bool isLooping = false;
        
        // The SpriteRenderer that should flash.
        private MeshRenderer meshRenderer;
        
        // The material that was in use, when the script started.
        private Material originalMaterial;

        // The currently running coroutine.
        private Coroutine flashRoutine;

        void Start()
        {
            // Get the SpriteRenderer to be used,
            // alternatively you could set it from the inspector.
            meshRenderer = GetComponent<MeshRenderer>();

            // Get the material that the SpriteRenderer uses, 
            // so we can switch back to it after the flash ended.
            originalMaterial = meshRenderer.material;
        }

        public void PlayMaterialChange()
        {
            // If the flashRoutine is not null, then it is currently running.
            if (flashRoutine != null)
            {
                // In this case, we should stop it first.
                // Multiple FlashRoutines the same time would cause bugs.
                return; // so return
            }
            else
            {
                // Start the Coroutine, and store the reference for it.
                flashRoutine = StartCoroutine(FlashRoutine());
            }
        }

        private IEnumerator FlashRoutine()
        {
            // Swap to the flashMaterial.
            meshRenderer.material = flashMaterial;

            // Pause the execution of this function for "duration" seconds.
            yield return new WaitForSeconds(duration);

            // After the pause, swap back to the original material.
            meshRenderer.material = originalMaterial;

            // Set the routine to null, signaling that it's finished.
            flashRoutine = null;

            if (isLooping)
            {
                flashRoutine = null;
                PlayMaterialChange();
                yield break;
            }
        }
    }
}