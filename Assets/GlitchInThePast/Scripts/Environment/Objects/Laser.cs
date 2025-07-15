using UnityEngine;
using UnityEngine.InputSystem;

public class Laser : MonoBehaviour
{

    #region Variables
    public Vector3 respawnOffset = new Vector3(-5f, 0f, 0f);

    private SpawningManager spawningManager;
    #endregion

    private void Start()
    {
        spawningManager = FindObjectOfType<SpawningManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            var playerInput = other.GetComponent<PlayerInput>();
            var movement = other.GetComponent<Player.GenericMovement.PlayerMovement>();

            if (movement != null && movement.IsDashing)
            {
                return;
            }

            if (playerInput != null && spawningManager != null)
            {
                Vector3 spawnPos = transform.position + respawnOffset;
                spawningManager.RespawnSinglePlayerAtPosition(playerInput, spawnPos);
            }
        }
    }
}