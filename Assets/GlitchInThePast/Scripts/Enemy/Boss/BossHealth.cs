using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies.Boss
{
    public class BossHealth : MonoBehaviour
    {
        [SerializeField] private List<int> healths = new List<int>() {0};
        [SerializeField] private List<int> maxHitCounts = new List<int>() {0};
        private int currentHitCount = 0;
        private bool isDamagable = false;
        private bool isMaxHit = false;
        
        [Header("Events")]
        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public Action<int> OnDamagedAction;
        public UnityEvent OnStageChanged;
        public Action OnStageChangedAction;
        public UnityEvent OnMaxHitsReached;

        public void SetDamagable(bool isDamagable)
        {
            this.isDamagable = isDamagable;

            // Reset everything when we are set to invinvible
            if (!isDamagable)
            {
                OnInteractiblesReset();
            }
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
        public void TakeDamage(int amount)
        {
            if (!isDamagable || isMaxHit)
            {
                Debug.Log("Boss is invincible or max hit");
                return;
            }

            currentHitCount++;
            isMaxHit = currentHitCount > maxHitCounts[BossStateManager.Instance.Phase];
            if (isMaxHit)
            {
                Debug.Log("Max hit reached");
                OnMaxHitsReached?.Invoke();
                return;
            }
            
            for (int i = 0; i < amount; i++)
            {
                healths[BossStateManager.Instance.Phase]--;
                OnDamaged?.Invoke();
                OnDamagedAction?.Invoke(1);

                if (healths[BossStateManager.Instance.Phase] == 0) // If our current phase dies
                {
                    // If we still have phase to go through, then go to the next one
                    OnStageChanged?.Invoke();
                    OnStageChangedAction?.Invoke();
                }
            }
            if (healths[BossStateManager.Instance.Phase] <= 0) // If our current phase dies
            {
                // Check if was the final phase
                if (healths.Count >= BossStateManager.Instance.Phase)
                {
                    OnDeath?.Invoke();
                    return;
                }
                    
                // If we still have phase to go through, then go to the next one
                OnStageChanged?.Invoke();
                OnStageChangedAction?.Invoke();
            }
        }

        public void TakeExcessDamage()
        {
            // NOT YET IMPLEMENTED
        }
    }
}