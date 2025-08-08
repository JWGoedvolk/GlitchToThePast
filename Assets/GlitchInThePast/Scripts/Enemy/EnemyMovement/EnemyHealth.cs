using System;
using GlitchInThePast.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Systems.Enemies
{
    public class EnemyHealth : MonoBehaviour
    {
        public enum EnemyTypes
        {
            Melee,
            Ranged,
            Boss
        }
        
        // Stats
        [SerializeField] private int healthMax = 3;
        [SerializeField] private int healthCurrent = 3;
        public EnemyTypes EnemyType = EnemyTypes.Melee;
        
        // Events
        [SerializeField] public UnityEvent<int> OnDamageTaken;
        [SerializeField] public UnityEvent OnDeath;
        public int Health
        {
            get { return healthCurrent; }
            set
            {
                healthCurrent = value; 
                
                // Check if we died
                if (healthCurrent <= 0)
                {
                    if (EnemyType == EnemyTypes.Melee)
                    {
                        if (spawner != null)
                        {
                            spawner.MeleeEnemyCount--;
                            spawner.MeleeKillCount++;
                        }
                        OnDeath?.Invoke();
                        Destroy(gameObject);
                    }
                    else if (EnemyType == EnemyTypes.Ranged)
                    {
                        if (spawner != null)
                        {
                            spawner.RangedEnemyCount--;
                            spawner.RangedKillCount++;
                        }
                        OnDeath?.Invoke();
                        Destroy(gameObject);
                    }
                }
            }
        }
        public int HealthMax => healthMax;
        public EnemySpawner spawner; // We hold a reference to our spawner so we can keep track of how many of each enemy is currently alive
        
        public void TakeDamage(int damage, PlayerWeaponSystem.WeaponType weaponType = PlayerWeaponSystem.WeaponType.None)
        {
            if (EnemyType == EnemyTypes.Boss)
            {
                OnDamageTaken?.Invoke(damage);
            }
            else if (EnemyType == EnemyTypes.Melee && weaponType == PlayerWeaponSystem.WeaponType.Melee)
            {
                Health -= damage;
                OnDamageTaken?.Invoke(damage);
            }
            else if (EnemyType == EnemyTypes.Ranged && weaponType == PlayerWeaponSystem.WeaponType.Ranged)
            {
                Health -= damage;
                OnDamageTaken?.Invoke(damage);
            }
        }
    }
}