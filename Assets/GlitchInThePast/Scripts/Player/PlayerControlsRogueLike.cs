using System;
using System.Collections;
using System.Collections.Generic;
using JW.Roguelike.Objects.Objects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace JW.Movement
{
    public class PlayerControlsRogueLike : MonoBehaviour
    {
        [Header("Movement")][SerializeField] private float speed;
        [SerializeField] private InputAction playerMovementControls;
        private Vector2 movDir;
        private Rigidbody2D rb;

        [Header("Lever")]
        [SerializeField] private InputAction playerInteractControls;
        [SerializeField] private float interactRadius = 1f;
        [SerializeField] private List<string> whitelistedTags;

        private void OnEnable()
        {
            playerMovementControls.Enable();
            playerInteractControls.Enable();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnDisable()
        {
            playerMovementControls.Disable();
            playerInteractControls.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            // Movement
            movDir = playerMovementControls.ReadValue<Vector2>();
            movDir.Normalize();
            movDir *= speed;

            rb.velocity = movDir;

            // Interaction
            if (playerInteractControls.triggered)
            {
                Debug.Log("Interact");
                var circleCast = Physics2D.BoxCastAll(transform.position, Vector2.one * interactRadius, 0f, transform.forward);
                foreach (var item in circleCast)
                {
                    if (whitelistedTags.Contains(item.transform.tag))
                    {
                        Lever lever = item.transform.GetComponent<Lever>();
                        if (lever != null)
                        {
                            lever.onActivated?.Invoke();
                        }
                        Debug.Log(item.collider.name);
                    }

                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * interactRadius);
        }
    }
}