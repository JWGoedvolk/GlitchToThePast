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
                if (MeleeEnemyCount < maxMeleeCount)
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
                if (RangedEnemyCount < maxRangedCount)
                {
                    GameObject newRanged = Instantiate(rangedEnemy, rangedSpawnPoint); // Spawn melee enemy at the melee spawn point as child of the spawn point
                    EnemyHealth enemyHealth = newRanged.GetComponent<EnemyHealth>(); // Get the health system to set it up
                    enemyHealth.spawner = this;
                    enemyHealth.EnemyType = EnemyHealth.EnemyTypes.Ranged;
                
                    RangedEnemyCount++;
                }
                rangedSpawnTimer = 0f;
            }
        }
    }
}