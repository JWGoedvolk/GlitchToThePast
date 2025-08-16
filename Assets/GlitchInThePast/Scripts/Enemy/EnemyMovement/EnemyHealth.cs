using System.Collections;
using GlitchInThePast.Scripts.Player;
using GlitchInThePast.Scripts.Utility;
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
        #region Variables
        // Stats
        [SerializeField] private int healthMax = 3;
        [SerializeField] private int healthCurrent = 3;
        public EnemyTypes EnemyType = EnemyTypes.Melee;

        // Flash parameters
        [Tooltip("How long the sprite stays red when hit.")]
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine flashCoroutine;

        // Events
        [SerializeField] public UnityEvent<int> OnDamageTaken;
        [SerializeField] public UnityEvent OnDeath;

        // Spawner counters
        public PooledEnemySpawner spawner;
        // TODO: Make and implement spawner for ranged enemies
        
        [Header("DEBUGGING")]
        public DamgeDebugger damgeDebugger;
        #endregion

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
                            spawner.KillCount++;
                            Debug.LogWarning($"Enemy {gameObject.name} is destroyed, increasing kill count for {spawner.name}");
                        }
                        OnDeath?.Invoke();
                        spawner.Pool.Release(gameObject);
                    }
                    else if (EnemyType == EnemyTypes.Ranged)
                    {
                        if (spawner != null)
                        {
                            spawner.KillCount++;
                        }
                        OnDeath?.Invoke();
                        spawner.Pool.Release(gameObject);
                    }
                    else
                    {
                        OnDeath?.Invoke();
                        Destroy(gameObject);
                    }
                }
            }
        }

        public int HealthMax => healthMax;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        #region Public Functions
        public void TakeDamage(int damage, PlayerWeaponSystem.WeaponType weaponType = PlayerWeaponSystem.WeaponType.None)
        {
            bool apply = false;

            if (EnemyType == EnemyTypes.Boss)
            {
                // Boss reacts
                OnDamageTaken?.Invoke(damage);
                StartFlash();
                
                if (damgeDebugger != null)
                {
                    damgeDebugger.DipslayDamageDealt(damage);
                }
                
                return;
            }
            else if (EnemyType == EnemyTypes.Melee && weaponType == PlayerWeaponSystem.WeaponType.Melee)
            {
                apply = true;
            }
            else if (EnemyType == EnemyTypes.Ranged && weaponType == PlayerWeaponSystem.WeaponType.Ranged)
            {
                apply = true;
            }

            if (apply)
            {
                Health -= damage;
                Debug.LogWarning($"Enemy: {gameObject.name} has been dealt {damage} damage with {weaponType}");

                OnDamageTaken?.Invoke(damage);
                StartFlash();

                if (damgeDebugger != null)
                {
                    damgeDebugger.DipslayDamageDealt(damage);
                }
            }
        }
        #endregion

        #region Private Functions
        private void StartFlash()
        {
            if (spriteRenderer == null) return;

            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            if (gameObject.activeInHierarchy)
                flashCoroutine = StartCoroutine(FlashRed());
        }

        private IEnumerator FlashRed()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            flashCoroutine = null;
        }
        #endregion
    }
}