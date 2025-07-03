using UnityEngine;
using UnityEngine.InputSystem;

public class Laser : MonoBehaviour
{

    #region Variables
    public Vector3 respawnOffset = new Vector3(-5f, 0f, 0f);

    public bool isLaserActive = false;

    private SpawningManager spawningManager;
    #endregion

    private void Start()
    {
        spawningManager = FindObjectOfType<SpawningManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLaserActive) return;

        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null && spawningManager != null)
            {
                Vector3 spawnPos = transform.position + respawnOffset;
                spawningManager.RespawnSinglePlayerAtPosition(playerInput, spawnPos);
            }
        }
    }
}