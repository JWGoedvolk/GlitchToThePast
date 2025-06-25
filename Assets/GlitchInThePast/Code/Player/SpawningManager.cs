using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public float respawnDelay = 5f;

    
    public void HandleRespawning(string playerID)
    {
        StartCoroutine(RespawnCour(playerID));
    }

    IEnumerator RespawnCour(string playerID)
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject playerWaitingToRespawn = (playerID == "player1") ? player1 : player2;

        if (playerWaitingToRespawn != null) 
        {
            //checkpoint

            HealthSystem hs = playerWaitingToRespawn.GetComponent<HealthSystem>();
            if (hs != null) 
            {
                hs.currentHealth = hs.maxHealth;
                hs.enabled = true;
            }

            Debug.Log(playerID + "respawned");
        }
    }

    public void UpdateCheckpoint()
    {
        //checkpoitn stuff
    }
}
