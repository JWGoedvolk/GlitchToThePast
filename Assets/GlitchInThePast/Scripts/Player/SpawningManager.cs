using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawningManager : MonoBehaviour
{
    public float respawnDelay = 10f;

    //keeps track of the checkpoints before respawning the player (IF ONLY ONE IS DEAD)
    private Dictionary<int, Transform> currentCheckpoints = new Dictionary<int, Transform>();

    //''    ''     '' which players courtine iswaitign for the respawn
    private Dictionary<int, Coroutine> respawnCoroutines = new Dictionary<int, Coroutine>();


    public Transform deafultCheckpoint;

    //keeps list of which players are dead
    private HashSet<int> deadplayers = new();


    public void HandleRespawning(PlayerInput playerInput)
    {
        int playerID = playerInput.playerIndex;
        deadplayers.Add(playerID);

        // if *both* are dead, cancel timers & immediately respawn both
        if (deadplayers.Count == 2)
        {
            foreach (var ct in respawnCoroutines.Values)
                if (ct != null) StopCoroutine(ct);

            //clar all stored coroutines
            respawnCoroutines.Clear();

            //Respawn both player immediately
            Respawn(0);
            Respawn(1);


            deadplayers.Clear();

        }
        else
        {
            //if 1 player died then coniue as normal
            if (!respawnCoroutines.ContainsKey(playerID))
            {
                respawnCoroutines[playerID] = StartCoroutine(RespawnCoroutine(playerInput));
            }
        }
    }

    IEnumerator RespawnCoroutine(PlayerInput pi)
    {
        yield return new WaitForSeconds(respawnDelay);

        Respawn(pi.playerIndex);
        deadplayers.Remove(pi.playerIndex);
        respawnCoroutines.Remove(pi.playerIndex);
    }

    void Respawn(int playerIndex)
    {
        PlayerInput playerInput = PlayerInput.all.FirstOrDefault(p => p.playerIndex == playerIndex);
        if (playerInput == null) return;

        GameObject gameObject = playerInput.gameObject;

        gameObject.SetActive(true);

        if (currentCheckpoints.TryGetValue(playerIndex, out var savedCp))
        {
            gameObject.transform.position = savedCp.position;
        }
        else
        {
            gameObject.transform.position = gameObject.transform.position;
        }

        PlayerHealthSystem hs = gameObject.GetComponent<PlayerHealthSystem>();
        if (hs != null) hs.ResetHealth();

        Debug.Log($"Player {playerIndex} respawned at {gameObject.transform.position}");
    }


    public void UpdateCheckpoint(int playerIndex, Transform checkpoint)
    {
        currentCheckpoints[playerIndex] = checkpoint;
    }
}