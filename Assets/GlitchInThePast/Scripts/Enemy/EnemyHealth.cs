using TMPro;
using UnityEngine;

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
        public int HealthMax { get => healthMax; }
        public EnemyTypes EnemyType = EnemyTypes.Melee;
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
                        spawner.MeleeEnemyCount--;
                    }
                    else if (EnemyType == EnemyTypes.Ranged)
                    {
                        spawner.RangedEnemyCount--;
                    }
                }
                
                // Update health UI
                healthUI.localScale = new Vector3(healthCurrent/healthMax, 1, 1);
            }
        }
        public EnemySpawner spawner; // We hold a reference to our spawner so we can keep track of how many of each enemy is currently alive
        
        [Header("UI")]
        [SerializeField] private Transform healthUI;
    }
}