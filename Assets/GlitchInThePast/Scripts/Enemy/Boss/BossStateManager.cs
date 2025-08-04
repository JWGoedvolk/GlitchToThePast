using System;
using UnityEngine;

namespace Systems.Enemies.Boss
{
    public class BossStateManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        public BossHealth bossHealth;
        public BossAttackManager bossAttackManager;

        private void OnEnable()
        {
            bossHealth = FindObjectOfType<BossHealth>();
            bossAttackManager = FindObjectOfType<BossAttackManager>();
            
            bossHealth.OnStageChangedAction += OnStageChanged;
        }

        private void OnDisable()
        {
            bossHealth.OnStageChangedAction -= OnStageChanged;
        }

        public void OnStageChanged()
        {
            animator.SetTrigger("EnterPhaseTwo");
        }

        public void OnAttack()
        {
            animator.SetTrigger("Attack");
        }
    }
}