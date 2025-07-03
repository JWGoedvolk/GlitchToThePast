using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies
{
    public class EnemyRespawner : MonoBehaviour
    {
        [Header("Transforms")]
        public Transform RespawnPoint;
        
        [Header("Respawn With Delay")]
        [SerializeField] private float respawnDelay;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onRespawn;
        
        // Other references
        private EnemyHealth health;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
        }

        public void Respawn()
        {
            StartCoroutine(RespawnCoroutine());
        }
        private IEnumerator RespawnCoroutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            transform.position = RespawnPoint.position;
            health.Health = health.HealthMax;
            onRespawn?.Invoke();
        }
    }
}