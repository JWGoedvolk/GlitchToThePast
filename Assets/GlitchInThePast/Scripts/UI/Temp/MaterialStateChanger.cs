using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.FadingEffect.Temp
{
    public class MaterialStateChanger : MonoBehaviour
    {
        public float ChangeTime = 0.5f;
        public Material StartingMaterial;
        public Material ChangeToMaterial;
        private MeshRenderer meshRenderer;
        Coroutine flashRoutine;

        void OnEnable()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (flashRoutine != null && meshRenderer.material != ChangeToMaterial) // Flip the coroutine to null when finished
            {
                flashRoutine = null;
            }
        }

        public void PlayMaterialChange()
        {
            if (flashRoutine == null) // Only run the coroutine if it is not running now
            {
                flashRoutine = StartCoroutine(nameof(MaterialChange));
            }
        }

        private IEnumerator MaterialChange()
        {
            StartingMaterial = meshRenderer.material;
            meshRenderer.material = ChangeToMaterial;
            yield return new WaitForSeconds(ChangeTime);
            meshRenderer.material = StartingMaterial;
        }
    }
}