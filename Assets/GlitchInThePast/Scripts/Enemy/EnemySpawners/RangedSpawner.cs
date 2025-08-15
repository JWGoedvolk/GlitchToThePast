using UnityEngine;
using UnityEngine.Pool;

namespace Systems.Enemies
{
    /// <summary>
    /// Author: JW
    /// This is a spawner for ranged enemies inheriting from the pooled object spawner
    /// </summary>
    public class RangedSpawner : PooledEnemySpawner
    {
        public Transform CruisingAltitudeTransform;
        public Transform AttackAltitudeTransform;

        protected override void Awake()
        {
            base.Awake();

            Pool = new ObjectPool<GameObject>(InstantiateEnemy, TakeEnemy, ReturnEnemy, DestroyEnemy, true, 10, 15);
        }

        protected override GameObject InstantiateEnemy()
        {
            /*GameObject enemy = base.InstantiateEnemy();*/
            GameObject enemy = Instantiate(Prefab, SpawnPoint.position, SpawnPoint.rotation);
            if (enemy == null)
            {
                Debug.LogError("Enemy doesn't exist");
                return null;
            }
            
            enemy.GetComponent<EnemyHealth>().spawner = this;
            enemy.GetComponent<EnemyHealth>().spawner = this; // It inherits so we need this specific one
            
            // Set up the ranged enemies altitudes
            RangedMovement rm = enemy.GetComponent<RangedMovement>();
            if (rm == null) // Leave if we are a dummy range enemy as they don't have movement scripts
            {
                return enemy;
            }
            rm.CruisingAltitude = CruisingAltitudeTransform.position.y;
            rm.AttackAltitude = AttackAltitudeTransform.position.y;
            return enemy;
        }
    }
}