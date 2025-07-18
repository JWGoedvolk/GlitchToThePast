using System.Collections.Generic;
using UnityEngine;

namespace Systems.Enemies.Boss
{
    public class BossSlamAttack : MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private GameObject shockwavePrefab;

        public void Attack()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                Instantiate(shockwavePrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
        
        
    }
}