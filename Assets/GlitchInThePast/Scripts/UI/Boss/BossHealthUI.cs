using System.Collections.Generic;
using Systems.Enemies.Boss;
using UnityEngine;

namespace UI.FadingEffect.Boss
{
    public class BossHealthUI : MonoBehaviour
    {
        private BossHealth bossHealth;
        [SerializeField] private int bossStage = 0;
        
        [SerializeField] private List<BossStageUI> bossStageUIs;

        void OnEnable()
        {
            bossHealth = FindObjectOfType<BossHealth>();
            
            bossHealth.OnDamagedAction += TakeDamage;

            for (int i = 1; i < bossStageUIs.Count; i++)
            {
                bossStageUIs[i].Panel.SetActive(false);
            }
        }

        private void OnDisable()
        {
            bossHealth.OnDamagedAction -= TakeDamage;
        }
        
        public void TakeDamage(int amount)
        {
            bossStageUIs[bossStage].TakeDamage();
        }

        public void OnStageChanged()
        {
            bossStageUIs[bossStage].EndStage();
            bossStage++;
            if (bossStage == bossStageUIs.Count)
            {
                return;
            }
            bossStageUIs[bossStage].StartStage();
        }

        public void AddStageUI(BossStageUI bossStageUI)
        {
            if (this.bossStageUIs == null)
            {
                this.bossStageUIs = new List<BossStageUI>();
            }
            this.bossStageUIs.Add(bossStageUI);
        }
    }
}