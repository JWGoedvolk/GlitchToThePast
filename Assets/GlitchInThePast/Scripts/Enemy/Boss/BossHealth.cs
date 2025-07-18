using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies.Boss
{
    public class BossHealth : MonoBehaviour
    {
        private int stage = 0;
        [SerializeField] private List<int> healths = new List<int>() {0};
        [SerializeField] private List<int> maxHitCounts = new List<int>() {0};
        private int currentHitCount = 0;
        private bool isDamagable = false;
        private bool isMaxHit = false;
        
        [Header("Events")]
        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public UnityEvent OnStageChanged;
        public UnityEvent OnMaxHitsReached;

        public void SetDamagable(bool isDamagable)
        {
            this.isDamagable = isDamagable;
        }

        public void OnInteractiblesReset()
        {
            isDamagable = false;
            currentHitCount = 0;
            isMaxHit = false;
        }

        /// <summary>
        /// Causes the boss to take 1 damage to the current stage's health. Automatically switches to the next stage if the current stage drops to 0 health
        /// </summary>
        public void TakeDamage()
        {
            if (!isDamagable || isMaxHit)
            {
                Debug.Log("Boss is invincible or max hit");
                return;
            }
            
            currentHitCount++;
            isMaxHit = currentHitCount >= maxHitCounts[stage];
            if (isMaxHit)
            {
                Debug.Log("Max hit reached");
                OnMaxHitsReached?.Invoke();
            }
            
            healths[stage]--;
            OnDamaged?.Invoke();
            
            if (healths[stage] <= 0)
            {
                stage++;
                OnStageChanged?.Invoke();
                
                if (stage >= healths.Count)
                {
                    OnDeath?.Invoke();
                }
            }
        }
    }
}