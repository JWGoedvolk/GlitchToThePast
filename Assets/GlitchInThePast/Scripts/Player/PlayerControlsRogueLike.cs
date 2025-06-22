using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JW.Movement
{
    public class PlayerControlsRogueLike : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float speed;
        [SerializeField] private InputAction playerControls;
        private Vector2 movDir;
        private Rigidbody2D rb;

        private void OnEnable()
        {
            playerControls.Enable();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            movDir = playerControls.ReadValue<Vector2>();
            movDir.Normalize();
            movDir *= speed;
            
            rb.velocity = movDir;
        }
    }
}
