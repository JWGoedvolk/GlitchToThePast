using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SpawningManager : MonoBehaviour, IPauseable
{
    public float respawnDelay = 10f;

    //keeps track of the checkpoints before respawning the player (IF ONLY ONE IS DEAD)
    private Dictionary<int, Transform> currentCheckpoints = new Dictionary<int, Transform>();

    //''    ''     '' which players courtine iswaitign for the respawn
    private Dictionary<int, Coroutine> respawnCoroutines = new Dictionary<int, Coroutine>();

    public Transform deafultCheckpoint;

    //keeps list of which players are dead
    private HashSet<int> deadplayers = new();
    
    [SerializeField] private UnityEvent onRespawn;

    void Start()
    {
        GamePauser.Instance?.RegisterPauseable(this);
    }

    void OnDestroy()
    {
        GamePauser.Instance?.UnregisterPauseable(this);
    }

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

        Vector3 respawnPos = gameObject.transform.position;
        if (currentCheckpoints.TryGetValue(playerIndex, out var savedCp))
        {
            respawnPos = savedCp.position;
        }
        else if (deafultCheckpoint != null)
        {
            respawnPos = deafultCheckpoint.position;
        }

        TeleportPlayer(gameObject, respawnPos);

        PlayerHealthSystem hs = gameObject.GetComponent<PlayerHealthSystem>();
        if (hs != null) hs.ResetHealth();
        onRespawn?.Invoke();

        Debug.Log($"Player {playerIndex} respawned at {gameObject.transform.position}");
    }

    public void UpdateCheckpoint(int playerIndex, Transform checkpoint)
    {
        currentCheckpoints[playerIndex] = checkpoint;
    }

    public void ExplodeRespawnAll(Vector3 explosionPoint, Vector3 offset)
    {
        deadplayers.Clear();

        foreach (Coroutine coroutine in respawnCoroutines.Values)
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }

        respawnCoroutines.Clear();

        Vector3 basePosition = explosionPoint + offset;

        int i = 0;
        foreach (PlayerInput pi in PlayerInput.all.OrderBy(p => p.playerIndex))
        {
            GameObject go = pi.gameObject;

            Vector3 spawnPos = basePosition + new Vector3(i * 1.2f, 0f, 0f);
            i++;

            TeleportPlayer(go, spawnPos);

            PlayerHealthSystem healthSystem = go.GetComponent<PlayerHealthSystem>();
            if (healthSystem != null) healthSystem.ResetHealth();
        }
    }
    public void RespawnSinglePlayerAtPosition(PlayerInput playerInput, Vector3 position)
    {
        if (playerInput == null) return;

        GameObject gameObject = playerInput.gameObject;

        TeleportPlayer(gameObject, position);

        if (gameObject.TryGetComponent(out PlayerHealthSystem healthSystem))
        {
            healthSystem.ResetHealth();
        }

        Debug.Log($"Player {playerInput.playerIndex} respawned at {position} (Laser hit)");
    }
    private void TeleportPlayer(GameObject gameObject, Vector3 position)
    {
        if (gameObject.TryGetComponent(out CharacterController cc))
        {
            cc.enabled = false;
        }

        gameObject.SetActive(false);
        gameObject.transform.position = position;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        if (gameObject.TryGetComponent(out CharacterController ccEnable))
        {
            ccEnable.enabled = true;
        }
    }

    #region IPauseable functions
    public void OnPause()
    {
        enabled = false;
    }

    public void OnUnpause()
    {
        enabled = true;
    }
    #endregion
}