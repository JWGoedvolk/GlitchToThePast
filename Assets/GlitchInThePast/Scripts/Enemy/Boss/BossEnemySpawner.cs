using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies.Boss
{
    public class BossEnemySpawner : MonoBehaviour
    {
        [Header("States")]
        public bool IsSpawning = false;
        public bool AllEnemiesSpawned = false;
        public bool AllEnemiesDead = false;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject rangedEnemyPrefab;
        
        [Header("Spawnpoints")]
        [SerializeField] private Transform meleeSpawnPoint;
        [SerializeField] private Transform rangedSpawnPoint;
        
        [Header("Enemy Counts")]
        // How many to spawn
        [SerializeField] private int meleeSpawnCount; 
        [SerializeField] private int rangedSpawnCount;
        // Spawn Rates
        [SerializeField] private float meleeInterval;
        [SerializeField] private float rangedInterval;
        // How many have been spawned
        [SerializeField] private int meleeCountSpawned;
        [SerializeField] private int meleeCountKilled;
        // How many are alive
        [SerializeField] private int rangedCountSpawned;
        [SerializeField] private int rangedCountKilled;

        [Header("Events")]
        public UnityEvent OnEnemiesSpawningStart;
        public UnityEvent OnEnemiesSpawningEnd;
        public UnityEvent OnAllEnemiesKilled;

        private void OnEnable()
        {
            IsSpawning = true;
        }

        public void StartSpawning()
        {
            
        }
    }
}