using System;
using Systems.Enemies.Boss;
using TMPro;
using UnityEngine;

namespace UI.FadingEffect.Boss
{
    public class BossStateShower : MonoBehaviour
    {
        [Header("UI")]
        public TMP_Text BossStateText;

        private void Update()
        {
            switch (BossStateManager.Instance.currentState)
            {
                case BossStateManager.State.Idle:
                    BossStateText.text = "Idle";
                    break;
                case BossStateManager.State.Transition:
                    BossStateText.text = "Transition";
                    break;
                case BossStateManager.State.Stunned:
                    BossStateText.text = "Stunned";
                    break;
                case BossStateManager.State.AttackArmRaise:
                    BossStateText.text = "Raising arms";
                    break;
                case BossStateManager.State.Attacking:
                    BossStateText.text = "Lowering arms and attacking";
                    break;
                case BossStateManager.State.SpawningEnemies:
                    BossStateText.text = "Spawning enemies";
                    break;
                case BossStateManager.State.Dead:
                    BossStateText.text = "Dead";
                    break;
            }
        }
    }
}