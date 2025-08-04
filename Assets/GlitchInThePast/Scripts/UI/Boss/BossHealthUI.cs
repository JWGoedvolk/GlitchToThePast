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
            bossHealth.OnDamagedAction += TakeDamage;
            bossHealth.OnStageChangedAction += OnStageChanged;
            
            bossStageUIs[0].StartStage();
        }

        private void OnDisable()
        {
            bossHealth.OnDamagedAction -= TakeDamage;
            bossHealth.OnStageChangedAction -= OnStageChanged;
        }
        
        public void TakeDamage()
        {
            bossStageUIs[bossStage].TakeDamage();
        }

        public void OnStageChanged()
        {
            bossStageUIs[bossStage].EndStage();
            bossStage++;
            bossStageUIs[bossStage].StartStage();
        }
    }
}