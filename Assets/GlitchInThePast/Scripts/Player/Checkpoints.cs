using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealthSystem hs = collision.GetComponent<PlayerHealthSystem>();
        if (hs != null && hs.spawningManager != null)
        {
            string playerTag = collision.tag; 
            hs.spawningManager.UpdateCheckpoint(playerTag, transform);
            Debug.Log("Checkpoint updated for " + playerTag);
        }

    }
}
    
