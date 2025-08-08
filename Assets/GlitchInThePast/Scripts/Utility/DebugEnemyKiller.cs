using System;
using GlitchInThePast.Scripts.Player;
using JW.BeatEmUp.Objects;
using Systems.Enemies;
using UnityEngine;

namespace GlitchInThePast.Scripts.Utility
{
    public class DebugEnemyKiller : CustomTriggerer
    {
        public int Damage = 3;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                foreach (var enemy in TriggeringObjects)
                {
                    if (enemy == null)
                    {
                        continue;
                    }
                    
                    EnemyHealth healh = enemy.GetComponent<EnemyHealth>();
                    healh.TakeDamage(Damage, PlayerWeaponSystem.WeaponType.Melee);
                    healh.TakeDamage(Damage, PlayerWeaponSystem.WeaponType.Ranged);
                }
            }
        }
    }
}