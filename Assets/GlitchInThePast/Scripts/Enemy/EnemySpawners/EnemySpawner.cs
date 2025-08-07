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
        public bool isSpawningMelee = true;
        [SerializeField] private GameObject meleeEnemy;
        [SerializeField] private Transform meleeSpawnPoint;
        [SerializeField] private float meleeSpawnInterval = 1f;
        [SerializeField] private float meleeSpawnTimer = 0f;
        [ReadOnly] public int MeleeEnemyCount = 0; // For non boss spawners. How many enemies are alive
        public int MeleeKillCount = 0; // For boss spawners. How many have been killed
        public int BossMeleeSpawnCount = 5; // For boss spawners. how many to spawn
        [SerializeField] private int maxMeleeCount = 5; // For non boss spawners. How many enemie can be active in the scene
    
        // Ranged
        public bool IsRangedBossSpawner = false;
        public bool isSpawningRanged = true;
        [SerializeField] private GameObject rangedEnemy;
        [SerializeField] private Transform rangedSpawnPoint;
        [SerializeField] private float rangedSpawnInterval = 1f;
        [SerializeField] private float rangedSpawnTimer = 0f;
        public int RangedEnemyCount = 0;
        public int RangedKillCount = 0;
        public int BossRangedSpawnCount = 5;
        [SerializeField] private int maxRangedCount = 5;
        [SerializeField] private Transform rangedCruisingAltitude;

        // Event
        [SerializeField] private UnityEvent onEnemySpawned;
        [SerializeField] private UnityEvent onMeleeEnemySpawned;
        [SerializeField] private UnityEvent onRangedEnemySpawned;
        [SerializeField] private UnityEvent onSpawnerDisabled;
        [SerializeField] private UnityEvent onSpawnerEnabled;
        [SerializeField] private UnityEvent onAllEnemiesSpawned;
        [SerializeField] private UnityEvent onAllEnemiesKilled;
        public bool HasInvokedAllKilled = false;
        public bool HasInvokedAllSpawned = false;

        public void OnReset()
        {
            // Reset flags
            isActive = true;
            HasInvokedAllKilled = false;
            HasInvokedAllSpawned = false;
            
            // Clear kill counts
            MeleeKillCount = 0;
            RangedKillCount = 0;
        }

        void Update()
        {
            if (!isActive) return; // Will only continue spawning when the lever is not pulled yet
            
            // Check if we have killed all enemies of their respective type
            if (IsMeleeBossSpawner && IsRangedBossSpawner)
            {
                bool isAllEnemiesKilled = (MeleeKillCount == BossMeleeSpawnCount) && (RangedKillCount == BossRangedSpawnCount) && !HasInvokedAllKilled;
                if (isAllEnemiesKilled)
                {
                    onAllEnemiesKilled?.Invoke();
                    HasInvokedAllKilled = true;
                }
            }

            // Increase timers
            if (isSpawningMelee)
            {
                meleeSpawnTimer += Time.deltaTime;
            }
            if (isSpawningRanged)
            {
                rangedSpawnTimer += Time.deltaTime;
            }
            
            // Check timers for spawn intervals

            #region Melee Spawning
            if (meleeSpawnTimer >= meleeSpawnInterval)
            {
                if (IsMeleeBossSpawner)
                {
                    if (BossMeleeSpawnCount > 0)
                    {
                        SpawnMelee();
                    }
                    else
                    {
                        isSpawningMelee = false;
                    }
                }
                else if (MeleeEnemyCount < maxMeleeCount || maxMeleeCount < 0)
                {
                    SpawnMelee();
                }
                else
                {
                    isSpawningMelee = false;
                }
            }
            #endregion

            #region Ranged Spawning
            if (rangedSpawnTimer >= rangedSpawnInterval)
            {
                if (IsRangedBossSpawner)
                {
                    if (BossRangedSpawnCount > 0)
                    {
                        SpawnRanged();
                    }
                    else
                    {
                        isSpawningRanged = false;
                    }
                }
                else if (RangedEnemyCount < maxRangedCount || maxRangedCount < 0)
                {
                    SpawnRanged();
                }
                else
                {
                    isSpawningRanged = false;
                }
            }
            #endregion
            
            // Check if all enemies have been spawned
            if (IsMeleeBossSpawner && IsRangedBossSpawner)
            {
                bool isAllSpawned = (0 == BossMeleeSpawnCount) && (0 == BossRangedSpawnCount) && !HasInvokedAllSpawned;
                if (isAllSpawned)
                {
                    onAllEnemiesSpawned?.Invoke();
                    HasInvokedAllSpawned = true;
                }
            }
        }

        public void StartBossSpawner(int meleeCount, int rangedCount)
        {
            isSpawningMelee = true;
            isSpawningRanged = true;
            BossMeleeSpawnCount = meleeCount;
            BossRangedSpawnCount = rangedCount;
            OnReset();
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
            if (IsRangedBossSpawner)
            {
                BossRangedSpawnCount--;
            }
            RangedEnemyCount++;
        }

        private void SpawnMelee()
        {
            meleeSpawnTimer = 0f;
            GameObject newMelee = Instantiate(meleeEnemy, meleeSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
            EnemyHealth enemyHealth = newMelee.GetComponent<EnemyHealth>(); // Get the health system to set it up
            enemyHealth.spawner = this;
            enemyHealth.EnemyType = EnemyHealth.EnemyTypes.Melee;
            if (IsMeleeBossSpawner)
            {
                BossMeleeSpawnCount--;
            }
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