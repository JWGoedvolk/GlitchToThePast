using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointTrigger : MonoBehaviour
{
    public SpawningManager spawningManager;

    void OnTriggerEnter(Collider collision)
    { 
        var pi = collision.GetComponent<PlayerInput>(); // Changed it to use player index instead of tag
        if (pi != null)
        {
            spawningManager.UpdateCheckpoint(pi.playerIndex, transform);
            Debug.Log("Checkpoint updated for " + pi.playerIndex);
        }
    }
}