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
        public bool CanAttack = false;
        
        // Animation
        [Header("Animation")]
        [SerializeField] private string armDroParameter;
        private Animator animator => BossStateManager.Instance.BossAnimator;
        
        public GameObject ShockwavePrefab;
        public List<Transform> spawnPoints;

        private void Awake()
        {
            if (stateManager.Phase == 0)
            {
                SetCanAttack(true);
            }
            else
            {
                SetCanAttack(false);
            }
        }

        public void SetCanAttack(bool value)
        {
            CanAttack = value;
            stateManager.CanAttack = CanAttack;
        }

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