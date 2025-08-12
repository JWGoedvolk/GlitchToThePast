using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Systems.Enemies
{
    public class EnemyMeleeSpawner : MonoBehaviour
    {
        public GameObject Prefab;
        public Transform SpawnPoint;
        public ObjectPool<GameObject> Pool;

        [Header("Timers")] 
        [SerializeField] float timer = 0f;
        public float SpawnDuration = 6f;
        [Tooltip("This is how long it waits after the previous group is defeated before spawning the new group")]
        public float GroupSpawnDelay = 10f;

        [Header("Counts")]
        public int GroupSize = 3;
        public int SpawnCount = 0;
        public int KillCount = 0;
        
        [Header("States")]
        public bool IsSpawning = false;
        public bool IsWaiting = false;

        private void Awake()
        {
            Pool = new ObjectPool<GameObject>(SpawnMelee, TakeMelee, ReturnMelee, DestroyMelee, true, 10, 15);
        }

        private void Update()
        {
            if (!IsSpawning)
            {
                if (KillCount == GroupSize)
                {
                    IsWaiting = true;
                    IsSpawning = false; // paranoia
                }
                else
                {
                    return;
                }
            }
            
            timer += Time.deltaTime;

            if (IsSpawning)
            {
                if (timer >= SpawnDuration / (float)GroupSize)
                {
                    timer = 0f;
                    Pool.Get();

                    if (SpawnCount == GroupSize)
                    {
                        IsSpawning = false;
                    }
                    
                    return;
                }
                else
                {
                    return;
                }
            }

            if (IsWaiting)
            {
                if (timer >= GroupSpawnDelay)
                {
                    timer = 0f;
                    IsWaiting = false;
                    IsSpawning = true;
                }
            }
        }

        #region Object Pool Functions

        private GameObject SpawnMelee()
        {
            GameObject enemy = Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
            enemy.GetComponent<EnemyHealth>().MeleeSpawner = this;
            return enemy;
        }
        private void TakeMelee(GameObject enemy)
        {
            enemy.transform.position = SpawnPoint.position;
            enemy.transform.rotation = SpawnPoint.rotation;
            
            enemy.SetActive(true);

            SpawnCount++;
        }
        private void ReturnMelee(GameObject enemy)
        {
            enemy.SetActive(false);
        }
        private void DestroyMelee(GameObject enemy)
        {
            Destroy(enemy);
        }
        #endregion
    }
}