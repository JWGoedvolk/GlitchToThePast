using UnityEngine;
using UnityEngine.Pool;

namespace Systems.Enemies
{
    public class PooledEnemySpawner : MonoBehaviour
    {
        public GameObject Prefab;
        public Transform SpawnPoint;
        public ObjectPool<GameObject> Pool;
        [Header("Timers")] [SerializeField] float timer = 0f;
        public float SpawnDuration = 6f;

        [Tooltip("This is how long it waits after the previous group is defeated before spawning the new group")]
        public float GroupSpawnDelay = 10f;

        [Header("Counts")] public int GroupSize = 3;
        public int SpawnCount = 0;
        public int KillCount = 0;

        public enum State
        {
            Spawning,
            SpawnDelay,
            WaitingForKills
        }

        [Header("States")] public State CurrentState = State.Spawning;
        public bool IsSpawning = false;
        public bool IsWaiting = false;

        private void Awake()
        {
            Pool = new ObjectPool<GameObject>(SpawnEnemy, TakeEnemy, ReturnEnemy, DestroyEnemy, true, 10, 15);
        }

        private void Update()
        {
            switch (CurrentState)
            {
                case State.Spawning:
                    timer += Time.deltaTime;
                    if (timer >= SpawnDuration / GroupSize)
                    {
                        timer = 0f;
                        Pool.Get();
                        SpawnCount++;
                        if (SpawnCount == GroupSize)
                        {
                            CurrentState = State.WaitingForKills;
                        }
                    }

                    break;
                case State.SpawnDelay:
                    timer += Time.deltaTime;
                    if (timer >= GroupSpawnDelay)
                    {
                        timer = 0f;
                        CurrentState = State.Spawning;
                    }

                    break;
                case State.WaitingForKills:
                    if (KillCount == GroupSize)
                    {
                        CurrentState = State.SpawnDelay;
                        KillCount = 0;
                        SpawnCount = 0;
                    }

                    break;
            }
        }

        #region Object Pool Functions
        protected virtual GameObject InstantiateEnemy()
        {
            GameObject enemy = Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
            enemy.GetComponent<EnemyHealth>().spawner = this;
            return enemy;
        }

        private GameObject SpawnEnemy()
        {
            var enemy = InstantiateEnemy();
            return enemy;
        }

        private void TakeEnemy(GameObject enemy)
        {
            Debug.Log("[EnemySpawner][Melee] Activating enemy");
            enemy.transform.position = SpawnPoint.position;
            enemy.transform.rotation = SpawnPoint.rotation;
            enemy.SetActive(true);
        }

        private void ReturnEnemy(GameObject enemy)
        {
            Debug.Log("[EnemySpawner][Melee] Deactivating enemy");
            enemy.SetActive(false);
        }

        private void DestroyEnemy(GameObject enemy)
        {
            Debug.Log("[EnemySpawner][Melee] Destroying enemy");
            Destroy(enemy);
        }
        #endregion
    }
}