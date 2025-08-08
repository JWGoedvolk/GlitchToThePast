using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Player.Health
{
    public class SpawningManager : MonoBehaviour, IPauseable
    {
        #region Variables
        [Header("How long it takes for the player to respawn.")]
        public float respawnDelay = 5f;

        [SerializeField] private UnityEvent onRespawn;
        private Dictionary<int, Transform> currentCheckpoints = new();
        public Transform deafultCheckpoint;

        // which players courtine is waiting for the respawn
        private Dictionary<int, Coroutine> respawnCoroutines = new Dictionary<int, Coroutine>();
        private Dictionary<int, SpriteRenderer> spriteRenderers = new();
        private Dictionary<int, Vector3> lastDeathPositions = new();

        // Keeps list of which players are dead
        private HashSet<int> deadplayers = new();
        #endregion

        private void Start()
        {
            InGameButtons.Instance?.RegisterPauseable(this);

            foreach (PlayerInput player in PlayerInput.all)
            {
                if (player.TryGetComponent(out SpriteRenderer sr))
                {
                    spriteRenderers[player.playerIndex] = sr;
                }
                else if (player.GetComponentInChildren<SpriteRenderer>() is SpriteRenderer childSR)
                {
                    spriteRenderers[player.playerIndex] = childSR;
                }
                else
                {
                    Debug.LogWarning($"Couldn't find a sprite renderer for Player {player.playerIndex}");
                }
            }
        }


        private void OnDestroy()
        {
            InGameButtons.Instance?.UnregisterPauseable(this);
        }

        #region Public Functions
        public void HandleRespawning(PlayerInput playerInput)
        {
            int playerID = playerInput.playerIndex;
            deadplayers.Add(playerID);

            // if *both* are dead, cancel timers & immediately respawn both
            if (deadplayers.Count == 2)
            {
                foreach (var ct in respawnCoroutines.Values)
                    if (ct != null) StopCoroutine(ct);

                lastDeathPositions.Clear();
                Respawn(0);
                Respawn(1);
                deadplayers.Clear();
            }
            else
            {
                // if 1 player died then continue as normal
                if (!respawnCoroutines.ContainsKey(playerID))
                {
                    respawnCoroutines[playerID] = StartCoroutine(RespawnCoroutine(playerInput));
                }
            }
        }

        public void ExplodeRespawnAll(Vector3 explosionPoint, Vector3 offset)
        {
            // This function is causing player animation to get iffy and stuck

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

                TeleportPlayer(go, spawnPos, pi.playerIndex);

                PlayerHealthSystem healthSystem = go.GetComponent<PlayerHealthSystem>();
                if (healthSystem != null) healthSystem.ResetHealth();
            }
        }

        public void RespawnSinglePlayerAtPosition(PlayerInput playerInput, Vector3 position)
        {
            if (playerInput == null) return;

            GameObject gameObject = playerInput.gameObject;

            TeleportPlayer(gameObject, position, playerInput.playerIndex);

            if (gameObject.TryGetComponent(out PlayerHealthSystem healthSystem))
            {
                healthSystem.ResetHealth();
                healthSystem.BeginRespawnInvulnerability();
            }

            // Debug.Log($"Player {playerInput.playerIndex} respawned at {position}");
        }
        public void SaveDeathLocation(int playerIndex, Vector3 pos)
        {
            lastDeathPositions[playerIndex] = pos;
        }

        public void UpdateCheckpoint(int playerIndex, Transform checkpoint)
        {
            currentCheckpoints[playerIndex] = checkpoint;
        }
        #endregion

        #region Private Functions
        private void Respawn(int playerIndex)
        {
            PlayerInput playerInput = PlayerInput.all.FirstOrDefault(p => p.playerIndex == playerIndex);
            if (playerInput == null) return;

            GameObject go = playerInput.gameObject;
            Vector3 respawnPos;

            bool useCheckpoint = !lastDeathPositions.ContainsKey(playerIndex); // checkpoint if no saved death

            if (useCheckpoint)
            {
                if (currentCheckpoints.TryGetValue(playerIndex, out var savedCp))
                    respawnPos = savedCp.position;
                else if (deafultCheckpoint != null)
                    respawnPos = deafultCheckpoint.position;
                else
                    respawnPos = go.transform.position;
            }
            else
            {
                respawnPos = lastDeathPositions[playerIndex];
            }

            TeleportPlayer(go, respawnPos, playerIndex);

            if (go.TryGetComponent(out PlayerHealthSystem hs))
            {
                hs.ResetHealth();
                hs.BeginRespawnInvulnerability();  
            }
            onRespawn?.Invoke();

            // Debug.Log($"Player {playerIndex} respawned at {respawnPos}");
        }

        private IEnumerator RespawnCoroutine(PlayerInput pi)
        {
            GameObject go = pi.gameObject;
            if (go.TryGetComponent(out CharacterController cc))
            {
                cc.enabled = false;
            }
            spriteRenderers[pi.playerIndex].enabled = false;

            yield return new WaitForSeconds(respawnDelay);

            Respawn(pi.playerIndex);
            deadplayers.Remove(pi.playerIndex);
            respawnCoroutines.Remove(pi.playerIndex);
        }

        private void TeleportPlayer(GameObject gameObject, Vector3 position, int playerIndex)
        {
            if (gameObject.TryGetComponent(out CharacterController cc))
            {
                cc.enabled = false;
            }
            spriteRenderers[playerIndex].enabled = false;

            // gameObject.SetActive(false);
            gameObject.transform.position = position;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);

            if (gameObject.TryGetComponent(out CharacterController ccEnable))
            {
                ccEnable.enabled = true;
            }
            spriteRenderers[playerIndex].enabled = true;
        }
        #endregion
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
}