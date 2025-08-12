using UnityEngine;

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

        protected override GameObject InstantiateEnemy()
        {
            GameObject enemy = base.InstantiateEnemy();
            enemy.GetComponent<EnemyHealth>().spawner = this; // It inherits so we need this specific one
            
            // Set up the ranged enemies altitudes
            RangedMovement rm = enemy.GetComponent<RangedMovement>();
            rm.CruisingAltitude = CruisingAltitudeTransform.position.y;
            rm.AttackAltitude = AttackAltitudeTransform.position.y;
            return enemy;
        }
    }
}