using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Enemies.Boss
{
    public class BossAttackManager : MonoBehaviour
    {
        #region Events And Actions
        public Action OnAttackEndAction;
        #endregion
        
        // Boss States
        private BossStateManager stateManager => BossStateManager.Instance;
        
        // Animation
        [Header("Animation")]
        [SerializeField] private string armDroParameter;
        private Animator animator => BossStateManager.Instance.BossAnimator;
        
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
            OnAttackEndAction?.Invoke();
            BossStateManager.Instance.AttackEnd();
        }
    }
}