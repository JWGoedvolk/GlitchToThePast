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
                for (int i = TriggeringObjects.Count - 1; i >= 0; i--)
                {
                    GameObject go = TriggeringObjects[i];

                    if (go != null)
                    {
                        if (!go.activeInHierarchy)
                        {
                            TriggeringObjects.Remove(go);
                        }
                        else
                        {
                            EnemyHealth health = go.GetComponent<EnemyHealth>();

                            if (health != null)
                            {
                                health.TakeDamage(Damage, PlayerWeaponSystem.WeaponType.Melee);
                                health.TakeDamage(Damage, PlayerWeaponSystem.WeaponType.Ranged);
                            }
                        }
                    }
                }
            }
        }
    }
}