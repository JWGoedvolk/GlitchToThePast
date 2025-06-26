using UnityEngine;

namespace Systems.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("States")] [SerializeField] private bool isActive = true;
        
        [Header("Melee")]
        [SerializeField] private GameObject meleeEnemy;
        [SerializeField] private Transform meleeSpawnPoint;
        [SerializeField] private float meleeSpawnInterval = 1f;
        private float meleeSpawnTimer = 0f;
    
        [Header("Ranged")]
        [SerializeField] private GameObject rangedEnemy;
        [SerializeField] private Transform rangedSpawnPoint;
        [SerializeField] private float rangedSpawnInterval = 1f;
        private float rangedSpawnTimer = 0f;

        public void OnReset()
        {
            isActive = true;
        }

        void Update()
        {
            if (!isActive) return; // Will only continue spawning when the lever is not pulled yet
            
            // Increaase timers
            meleeSpawnTimer += Time.deltaTime;
            rangedSpawnTimer += Time.deltaTime;

            // Check timers for spawn intervals
            if (meleeSpawnTimer >= meleeSpawnInterval)
            {
                Instantiate(meleeEnemy, meleeSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
                meleeSpawnTimer = 0f;
            }
            if (rangedSpawnTimer >= rangedSpawnInterval)
            {
                Instantiate(rangedEnemy, rangedSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
                rangedSpawnTimer = 0f;
            }
        }
    }
}