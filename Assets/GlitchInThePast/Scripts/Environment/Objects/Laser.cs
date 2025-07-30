using UnityEngine;
using UnityEngine.InputSystem;

public class Laser : MonoBehaviour
{

    #region Variables
    public Transform LeftRespawn;
    public Transform RightRespawn;

    private SpawningManager spawningManager;
    #endregion

    private void Start()
    {
        spawningManager = FindObjectOfType<SpawningManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            var health = other.GetComponent<PlayerHealthSystem>();
            var characterController = other.GetComponent<CharacterController>();
            var playerInput = other.GetComponent<PlayerInput>();
            var movement = other.GetComponent<Player.GenericMovement.PlayerMovement>();
            Debug.Log($"velocity: {characterController.velocity}");
            if (movement != null && (movement.IsDashing || health.isInvulerable))
            {
                return;
            }

            //health.TakeDamage(1);
            
            if (playerInput != null && spawningManager != null)
            {
                
                var moveDirection = characterController.velocity;
                if (moveDirection.x > 0) // Moving towards the right side of the laser
                {
                    // Teleport them to the left
                    spawningManager.RespawnSinglePlayerAtPosition(playerInput, LeftRespawn.position);
                }
                else if (moveDirection.x < 0) // Moving towards the left side of the laser
                {
                    // Teleport them to the right
                    spawningManager.RespawnSinglePlayerAtPosition(playerInput, RightRespawn.position);
                }
                
                
            }
        }
    }
}