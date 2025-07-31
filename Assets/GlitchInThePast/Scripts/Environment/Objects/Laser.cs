using Player.Health;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hazard.Laser
{
    public class Laser : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Vector3 respawnOffset = new Vector3(-5f, 0f, 0f);
        [SerializeField] private float damageInterval = 0.5f;

        private SpawningManager spawningManager;
        #endregion

        private void Start()
        {
            spawningManager = FindObjectOfType<SpawningManager>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!(other.CompareTag("Player1") || other.CompareTag("Player2"))) return;

            var playerInput = other.GetComponent<PlayerInput>();
            var movement = other.GetComponent<Player.GenericMovement.PlayerMovement>();
            var health = other.GetComponent<PlayerHealthSystem>();

            if (health == null || playerInput == null) return;
            if (health.isInvincible) return;

            if (Time.time - health.lastDamageTime >= damageInterval && health.currentHealth >= 0)
            {
                health.lastDamageTime = Time.time;
                health.TakeDamage(1);
            }
            else if (health.currentHealth <= 0 && spawningManager != null)
            {
                Vector3 spawnPos = transform.position + respawnOffset;
            }
        }
    }
}