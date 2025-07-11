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
        
        [Header("Stats")] 
        [SerializeField] private int healthMax = 3;
        [SerializeField] private int healthCurrent = 3;
        public EnemyTypes EnemyType = EnemyTypes.Melee;
        
        [Header("Events")]
        [SerializeField] public UnityEvent OnDamageTaken;
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
                        if (spawner != null) spawner.MeleeEnemyCount--;
                        OnDeath?.Invoke();
                    }
                    else if (EnemyType == EnemyTypes.Ranged)
                    {
                        if (spawner != null) spawner.RangedEnemyCount--;
                        OnDeath?.Invoke();
                    }
                }
                
                // Update health UI
                if (healthUI != null)
                {
                    healthUI.localScale = new Vector3((float)healthCurrent/healthMax, healthUI.localScale.y, healthUI.localScale.z);
                }
            }
        }
        public int HealthMax => healthMax;
        public EnemySpawner spawner; // We hold a reference to our spawner so we can keep track of how many of each enemy is currently alive

        [Header("UI")]
        [SerializeField] private Transform healthUI;

        public void TakeDamage(int damage, PlayerWeaponSystem.WeaponType weaponType)
        {
            if (EnemyType == EnemyTypes.Melee && weaponType == PlayerWeaponSystem.WeaponType.Melee)
            {
                Health -= damage;
                OnDamageTaken?.Invoke();
            }
            else if (EnemyType == EnemyTypes.Ranged && weaponType == PlayerWeaponSystem.WeaponType.Ranged)
            {
                Health -= damage;
                OnDamageTaken?.Invoke();
            }
        }
    }
}