using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Enemies.Boss
{
    public class BossAttackManager : MonoBehaviour
    {
        public Action OnAttackAction;
        public Animator AnimationController;
        public GameObject ShockwavePrefab;
        public List<Transform> spawnPoints;

        public void AttackLeft()
        {
            Transform spawnPoint = spawnPoints[0];
            Instantiate(ShockwavePrefab, spawnPoint.position, spawnPoint.rotation);
        }
        public void AttackRight()
        {
            Transform spawnPoint = spawnPoints[1];
            Instantiate(ShockwavePrefab, spawnPoint.position, spawnPoint.rotation);
        }
        
        public void Attack()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                Instantiate(ShockwavePrefab, spawnPoint.position, spawnPoint.rotation);
            }
            OnAttackAction?.Invoke();
            AnimationController.SetTrigger("Attack");
        }
    }
}