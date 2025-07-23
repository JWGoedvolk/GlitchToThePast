using System;
using System.Collections;
using System.Collections.Generic;
using GlitchInThePast.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
        
        [Header("Resettable Settings")]
        [SerializeField] private bool isResetting = false;
        [SerializeField] private bool defaultState = false;
        [SerializeField] private bool isActivated = false;
        [SerializeField] [Min(0f)] private float resetAfter = 1f;
        
        [Header("State Materials")]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material defaultStateMaterial;
        [SerializeField] private Material activeStateMaterial;
        
        [Header("Events")]
        public UnityEvent OnInteraction;
        public UnityEvent OnDeactivated;
        public UnityEvent OnReset;

        #region Public Getters
        public List<string> Whitelist { get { return whitelist; } }
        public List<GameObject> TriggeringObjects { get { return triggeringObjects; } }
        public bool IsActivated { get { return isActivated; } }
        #endregion

        private void OnEnable()
        {
            isActivated = defaultState;
        }

        private void Update()
        {
            // Show a prompt for how to interact with this object
            interactPrompt.SetActive(triggeringObjects.Count > 0);
            
            meshRenderer.material = isActivated ? activeStateMaterial : defaultStateMaterial;
        }
        
        void Reset()
        {
            isActivated = defaultState;
            OnReset?.Invoke();
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
            if (!isActivated)
            {
                OnInteraction?.Invoke();
                isActivated = true;
            }
            else
            {
                OnDeactivated?.Invoke();
                isActivated = false;
            }

            if (isResetting && isActivated != defaultState)
            {
                // Stop and restart the reset countdown coroutine. This makes it so the reset timer is reset every activation
                StopCoroutine(ResetCountdown());
                StartCoroutine(ResetCountdown());
            }
        }

        public IEnumerator ResetCountdown()
        {
            yield return new WaitForSeconds(resetAfter);
            Reset();
        }
    }
}