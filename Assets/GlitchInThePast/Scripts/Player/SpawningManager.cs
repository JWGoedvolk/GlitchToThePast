using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public float respawnDelay = 10f;

    //keeps track of the checkpoints before respawning the player (IF ONLY ONE IS DEAD)
    private Dictionary  <string, Transform> currentCheckpoints = new Dictionary<string, Transform>();

    //''    ''     '' which players courtine iswaitign for the respawn
    private Dictionary  <string, Coroutine> respawnCoroutines = new Dictionary<string, Coroutine>();


    public Transform deafultCheckpoint;

    //keeps list of which players are dead
    public HashSet <string> deadplayers = new HashSet<string>();

    

    public void HandleRespawning(string playerTag)
    {
        if (deadplayers.Contains("Player1") && deadplayers.Contains("Player2")) 
        {
            foreach (var routine in respawnCoroutines.Values) 
            {
                if(routine != null)
                    StopCoroutine(routine);
            }

            //clar all stored coroutines
            respawnCoroutines.Clear();

            //Respawn both player immediately
            Respawn("Player1");
            Respawn("Player2");


            deadplayers.Clear();

        }
        else
        {
            //if 1 player died then coniue as normal
            if(!respawnCoroutines.ContainsKey(playerTag))
            {
                Coroutine c = StartCoroutine(RespawnCour(playerTag));
                respawnCoroutines.Add(playerTag, c);
            }
        }
    }
    IEnumerator RespawnCour(string playerTag)
    {
        yield return new WaitForSeconds(respawnDelay);

        Respawn(playerTag);
        deadplayers.Remove(playerTag);
        respawnCoroutines.Remove(playerTag);

    }
    public void Respawn(string playerTag)
    {
   

        GameObject playerWaitingToRespawn = null;

        if (playerTag == "Player1") playerWaitingToRespawn = player1;
        else if (playerTag == "Player2") playerWaitingToRespawn = player2;

        if (playerWaitingToRespawn != null) 
        {
            //checkpoint
            Transform checkpoint = deafultCheckpoint;

            if (currentCheckpoints.ContainsKey(playerTag)) 

            checkpoint = currentCheckpoints[playerTag];

           playerWaitingToRespawn.transform.position = checkpoint.position;
           playerWaitingToRespawn.SetActive(true);           

           PlayerHealthSystem hs = playerWaitingToRespawn.GetComponent<PlayerHealthSystem>();

            if (hs != null) 
            {
                hs.currentHealth = hs.maxHealth;
                hs.enabled = true;
            }

            Debug.Log(playerTag + "respawned");
        }
    }

    public void UpdateCheckpoint(string playerTag, Transform checkpoint)
    {
        if(currentCheckpoints.ContainsKey(playerTag))
            currentCheckpoints[playerTag] = checkpoint;
        else
            currentCheckpoints.Add(playerTag, checkpoint);
    }
}
