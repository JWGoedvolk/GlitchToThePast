using System;
using UnityEngine;

namespace GlitchInThePast.Scripts.Player.RespawnTest
{
    public class PlayerRespawner : MonoBehaviour
    {
        public Transform RespawnPoint;
        
        private CharacterController characterController;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void Respawn()
        {
            characterController.enabled = false;
            spriteRenderer.enabled = false;
            transform.position = RespawnPoint.position;
            characterController.enabled = true;
            spriteRenderer.enabled = true;
        }
    }
}