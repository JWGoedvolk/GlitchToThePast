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
        public float respawnDelay = 10f;

        [SerializeField] private UnityEvent onRespawn;

        // which players courtine is waiting for the respawn
        private Dictionary<int, Coroutine> respawnCoroutines = new Dictionary<int, Coroutine>();
        private Dictionary<int, SpriteRenderer> spriteRenderers = new();

        // Keeps list of which players are dead
        private HashSet<int> deadplayers = new();
        #endregion

        private void Start()
        {
            GamePauser.Instance?.RegisterPauseable(this);

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

                // Clear all stored coroutines
                respawnCoroutines.Clear();

                //Respawn both player immediately
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
            }

            Debug.Log($"Player {playerInput.playerIndex} respawned at {position}");
        }

        private void Respawn(int playerIndex)
        {
            PlayerInput playerInput = PlayerInput.all.FirstOrDefault(p => p.playerIndex == playerIndex);
            if (playerInput == null) return;

            GameObject gameObject = playerInput.gameObject;

            Vector3 respawnPos = gameObject.transform.position;
            TeleportPlayer(gameObject, respawnPos, playerIndex);

            PlayerHealthSystem hs = gameObject.GetComponent<PlayerHealthSystem>();
            if (hs != null) hs.ResetHealth();
            onRespawn?.Invoke();

            Debug.Log($"Player {playerIndex} respawned at {gameObject.transform.position}");
        }

        private IEnumerator RespawnCoroutine(PlayerInput pi)
        {
            // Disable character controller and sprite renderer to show the player is dead, and so they can't move
            if (gameObject.TryGetComponent(out CharacterController cc))
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