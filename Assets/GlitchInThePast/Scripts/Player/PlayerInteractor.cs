using System;
using JW.Roguelike.Objects.Interactibles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GlitchInThePast.Scripts.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        // PlayerInput
        private PlayerInput playerInput;
        
        // Range
        [HideInInspector] public Interactible interactingObject;
        
        [Header("Events")]
        public UnityEvent OnInteract;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        void OnEnable()
        {
            playerInput.actions["Interact"].performed += OnInteraction;
        }

        void OnDisable()
        {
            playerInput.actions["Interact"].performed -= OnInteraction;
        }

        private void OnInteraction(InputAction.CallbackContext context)
        {
            if (interactingObject != null)
            {
                interactingObject.Interact();
                OnInteract?.Invoke();
            }
        }
    }
}