using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Roguelike.Objects.Interactibles
{
    public class CoopInteractibles : MonoBehaviour
    {
        [SerializeField] private List<Interactible> interactibles;

        private bool isActivated;
        [SerializeField] private UnityEvent onActivated;
        [SerializeField] private UnityEvent onDeactivated;

        // [Header("State Material Display")] 
        // [SerializeField] private SpriteRenderer sr;
        // [SerializeField] private Material activeMaterial;
        // [SerializeField] private Material inactiveMaterial;
        
        private void Update()
        {
            bool allActive = true;
            foreach (var interactible in interactibles)
            {
                if (interactible.IsActivated)
                {
                    continue;
                }
                else
                {
                    allActive = false;
                    break;
                }
            }

            if (allActive && !isActivated)
            {
                Debug.Log("Coop activated");
                onActivated?.Invoke();
                isActivated = true;
            }

            if (isActivated && !allActive)
            {
                Debug.Log("Coop deactivated");
                onDeactivated?.Invoke();
                isActivated = false;
            }
            
            //sr.material = isActivated ? activeMaterial : inactiveMaterial;
        }
    }
}