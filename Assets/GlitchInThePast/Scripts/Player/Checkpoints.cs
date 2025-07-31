using Player.Health;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Health.Checkpoint
{
    public class CheckpointTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerInput pi) &&
                other.TryGetComponent(out PlayerHealthSystem playerHealthSystem) &&
                playerHealthSystem.spawningManager != null)
            {
                int playerIndex = pi.playerIndex;
                playerHealthSystem.spawningManager.UpdateCheckpoint(playerIndex, transform); 
                Debug.Log($"Checkpoint updated for Player {playerIndex} at {transform.position}");
            }
        }
    }
}
