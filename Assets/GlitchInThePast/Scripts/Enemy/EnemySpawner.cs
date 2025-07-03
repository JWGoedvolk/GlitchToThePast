using System;
using UnityEngine;

namespace Systems.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("States")] 
        [SerializeField] private bool isActive = true;
        
        [Header("Melee")]
        [SerializeField] private GameObject meleeEnemy;
        [SerializeField] private Transform meleeSpawnPoint;
        [SerializeField] private float meleeSpawnInterval = 1f;
        private float meleeSpawnTimer = 0f;
        public int MeleeEnemyCount = 0;
        [SerializeField] private int maxMeleeCount = 5;
    
        [Header("Ranged")]
        [SerializeField] private GameObject rangedEnemy;
        [SerializeField] private Transform rangedSpawnPoint;
        [SerializeField] private float rangedSpawnInterval = 1f;
        private float rangedSpawnTimer = 0f;
        public int RangedEnemyCount = 0;
        [SerializeField] private int maxRangedCount = 5;


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
            if (meleeSpawnTimer >= meleeSpawnInterval)
            {
                if (MeleeEnemyCount < maxMeleeCount || maxMeleeCount < 0)
                {
                    GameObject newMelee = Instantiate(meleeEnemy, meleeSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
                    EnemyHealth enemyHealth = newMelee.GetComponent<EnemyHealth>(); // Get the health system to set it up
                    enemyHealth.spawner = this;
                    enemyHealth.EnemyType = EnemyHealth.EnemyTypes.Melee;
                
                    MeleeEnemyCount++;
                }
                meleeSpawnTimer = 0f;
            }
            if (rangedSpawnTimer >= rangedSpawnInterval)
            {
                if (RangedEnemyCount < maxRangedCount || maxRangedCount < 0)
                {
                    GameObject newRanged = Instantiate(rangedEnemy, rangedSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
                    EnemyHealth enemyHealth = newRanged.GetComponent<EnemyHealth>(); // Get the health system to set it up
                    enemyHealth.spawner = this;
                    enemyHealth.EnemyType = EnemyHealth.EnemyTypes.Ranged;
                    RangedMovement movement = newRanged.GetComponent<RangedMovement>();
                    movement.cruisingAltitude = rangedSpawnPoint.position.y;
                
                    RangedEnemyCount++;
                }
                rangedSpawnTimer = 0f;
            }
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