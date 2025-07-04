using System;
using System.Collections.Generic;
using GlitchInThePast.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JW.Roguelike.Objects.Interactibles
{
    [RequireComponent(typeof(BoxCollider))]
    public class Interactible : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private TMP_Text interactPromptText;

        [Header("Detection")] 
        [SerializeField] private List<string> whitelist;
        [SerializeField] private List<GameObject> triggeringObjects;
        
        [Header("Events")]
        public UnityEvent OnInteraction;

        private void Update()
        {
            // Show a prompt for how to interact with this object
            interactPrompt.SetActive(triggeringObjects.Count > 0);
        }

        void OnTriggerEnter(Collider other)
        {
            if (whitelist.Contains(other.transform.tag))
            {
                if (!triggeringObjects.Contains(other.gameObject))
                {
                    triggeringObjects.Add(other.gameObject);
                    
                    // Set this object as the thing to interact with
                    PlayerInteractor playerInteractor = other.GetComponent<PlayerInteractor>();
                    if (playerInteractor != null)
                    {
                        playerInteractor.interactingObject = this;
                        if (playerInteractor.GetComponent<PlayerInput>().currentControlScheme == "Controller")
                        {
                            interactPromptText.text = "Press 'X' to interact";
                        }
                        else
                        {
                            interactPromptText.text = "Press 'E' to interact";
                        }
                    }
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (whitelist.Contains(other.transform.tag))
            {
                if (triggeringObjects.Contains(other.gameObject))
                {
                    triggeringObjects.Remove(other.gameObject);
                    
                    // Make it so the play can no longer interact with this object
                    PlayerInteractor playerInteractor = other.GetComponent<PlayerInteractor>();
                    if (playerInteractor != null)
                    {
                        playerInteractor.interactingObject = null;
                    }
                }
            }
        }
        
        public virtual void Interact()
        {
            OnInteraction?.Invoke();
        }
    }
}