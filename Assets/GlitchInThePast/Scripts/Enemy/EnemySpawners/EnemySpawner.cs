using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        // States
        [SerializeField] private bool isActive = true;
        
        // Melee
        public bool IsMeleeBossSpawner = false;
        [SerializeField] private GameObject meleeEnemy;
        [SerializeField] private Transform meleeSpawnPoint;
        [SerializeField] private float meleeSpawnInterval = 1f;
        [SerializeField] private float meleeSpawnTimer = 0f;
        [ReadOnly] public int MeleeEnemyCount = 0;
        public int MeleeSpawnCount = 0;
        public int BossMeleeSpawnCount = 5;
        [SerializeField] private int maxMeleeCount = 5;
    
        // Ranged
        public bool IsRangedBossSpawner = false;
        [SerializeField] private GameObject rangedEnemy;
        [SerializeField] private Transform rangedSpawnPoint;
        [SerializeField] private float rangedSpawnInterval = 1f;
        [SerializeField] private float rangedSpawnTimer = 0f;
        public int RangedEnemyCount = 0;
        public int RangedSpawnCount = 0;
        public int BossRangedSpawnCount = 5;
        [SerializeField] private int maxRangedCount = 5;
        [SerializeField] private Transform rangedCruisingAltitude;

        // Event
        [SerializeField] private UnityEvent onEnemySpawned;
        [SerializeField] private UnityEvent onMeleeEnemySpawned;
        [SerializeField] private UnityEvent onRangedEnemySpawned;
        [SerializeField] private UnityEvent onSpawnerDisabled;
        [SerializeField] private UnityEvent onSpawnerEnabled;


        public void OnReset()
        {
            isActive = true;
        }

        void Update()
        {
            if (!isActive) return; // Will only continue spawning when the lever is not pulled yet
            
            // Increase timers
            meleeSpawnTimer += Time.deltaTime;
            rangedSpawnTimer += Time.deltaTime;
            
            // Check timers for spawn intervals

            #region Melee Spawning
            if (meleeSpawnTimer >= meleeSpawnInterval)
            {
                if (IsMeleeBossSpawner)
                {
                    if (MeleeSpawnCount <= BossMeleeSpawnCount)
                    {
                        SpawnMelee();
                        BossMeleeSpawnCount++;
                    }
                }
                else if (MeleeEnemyCount < maxMeleeCount || maxMeleeCount < 0)
                {
                    SpawnMelee();
                }
            }
            #endregion

            #region Ranged Spawning
            if (rangedSpawnTimer >= rangedSpawnInterval)
            {
                if (IsRangedBossSpawner)
                {
                    if (RangedSpawnCount <= BossRangedSpawnCount)
                    {
                        SpawnRanged();
                        RangedSpawnCount++;
                    }
                }
                else if (RangedEnemyCount < maxRangedCount || maxRangedCount < 0)
                {
                    SpawnRanged();
                }
            }
            #endregion
        }

        private void SpawnRanged()
        {
            rangedSpawnTimer = 0f;
            GameObject newRanged = Instantiate(rangedEnemy, rangedSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
            EnemyHealth enemyHealth = newRanged.GetComponent<EnemyHealth>(); // Get the health system to set it up
            enemyHealth.spawner = this;
            enemyHealth.EnemyType = EnemyHealth.EnemyTypes.Ranged;
            RangedMovement movement = newRanged.GetComponent<RangedMovement>();
            movement.cruisingAltitude = rangedCruisingAltitude.position.y;
                
            RangedEnemyCount++;
        }

        private void SpawnMelee()
        {
            meleeSpawnTimer = 0f;
            GameObject newMelee = Instantiate(meleeEnemy, meleeSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
            EnemyHealth enemyHealth = newMelee.GetComponent<EnemyHealth>(); // Get the health system to set it up
            enemyHealth.spawner = this;
            enemyHealth.EnemyType = EnemyHealth.EnemyTypes.Melee;
                
            MeleeEnemyCount++;
        }

        public void DisableSpawner()
        {
            isActive = false;
        }

        private void OnDrawGizmosSelected()
        {
            // Spawn point visuals
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(meleeSpawnPoint.position, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(rangedSpawnPoint.position, 0.1f);
            
            // Cruising altitude visual
            RangedMovement movement = rangedEnemy.GetComponent<RangedMovement>();
            float cruisingVariance = movement.cruisingAltitudeError;
            Debug.DrawRay(rangedSpawnPoint.position, Vector3.left * 100, Color.green);
            Debug.DrawRay(rangedSpawnPoint.position + Vector3.up * cruisingVariance, Vector3.left * 100, new Color(0, 1, 0, 0.1f));
            Debug.DrawRay(rangedSpawnPoint.position - Vector3.up * cruisingVariance, Vector3.left * 100, new Color(0, 1, 0, 0.1f));
        }
    }
}