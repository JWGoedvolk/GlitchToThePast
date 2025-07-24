using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies.Boss
{
    public class BossEnemySpawner : MonoBehaviour
    {
        [SerializeField] private BossHealth bossHealth;
        [SerializeField] private bool isSpawning = false;
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject rangedEnemyPrefab;
        [SerializeField] private Transform meleeSpawnPoint;
        [SerializeField] private Transform rangedSpawnPoint;
        [SerializeField] private List<int> meleeSpawnCounts;
        [SerializeField] private List<int> rangedSpawnCounts;
        [SerializeField] private int meleeCountSpawned;
        [SerializeField] private int meleeCountKilled;
        [SerializeField] private int rangedCountSpawned;
        [SerializeField] private int rangedCountKilled;
        
        public UnityEvent OnAllEnemiesKilled;
        private void Update()
        {
            if (!isSpawning)
            {
                return;
            }

            if (meleeCountSpawned <= meleeSpawnCounts[bossHealth.Stage])
            {
                Instantiate(meleeEnemyPrefab, meleeSpawnPoint);
                meleeCountSpawned++;
            }

            if (rangedCountSpawned <= rangedSpawnCounts[bossHealth.Stage])
            {
                Instantiate(rangedEnemyPrefab, rangedSpawnPoint);
                rangedCountSpawned++;
            }

            if (meleeCountKilled == meleeSpawnCounts[bossHealth.Stage] && rangedCountKilled == rangedSpawnCounts[bossHealth.Stage])
            {
                OnAllEnemiesKilled?.Invoke();
            }
        }
    }
}