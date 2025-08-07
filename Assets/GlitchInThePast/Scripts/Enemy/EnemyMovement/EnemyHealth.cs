using System.Collections;
using GlitchInThePast.Scripts.Player;
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

        // Flash parameters
        [Tooltip("How long the sprite stays red when hit.")]
        [SerializeField] private float flashDuration = 0.1f;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine flashCoroutine;

        // Events
        [SerializeField] public UnityEvent OnDamageTaken;
        [SerializeField] public UnityEvent OnDeath;

        public EnemySpawner spawner;

        public int Health
        {
            get { return healthCurrent; }
            set
            {
                healthCurrent = value;

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
            }
        }
        public int HealthMax => healthMax;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }

        public void TestMessage(int num)
        {
            TakeDamage(num);
        }

        public void TakeDamage(int damage, PlayerWeaponSystem.WeaponType weaponType = PlayerWeaponSystem.WeaponType.None)
        {
            if (EnemyType == EnemyTypes.Boss)
            {
                OnDamageTaken?.Invoke();
                StartFlash();
            }
            else if (EnemyType == EnemyTypes.Melee && weaponType == PlayerWeaponSystem.WeaponType.Melee)
            {
                Health -= damage;
                OnDamageTaken?.Invoke();
                StartFlash();
            }
            else if (EnemyType == EnemyTypes.Ranged && weaponType == PlayerWeaponSystem.WeaponType.Ranged)
            {
                Health -= damage;
                OnDamageTaken?.Invoke();
                StartFlash();
            }
        }

        private void StartFlash()
        {
            if (spriteRenderer == null) return;
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            if (this.gameObject.activeSelf == true)
            {
                flashCoroutine = StartCoroutine(FlashRed());
            }
        }

        private IEnumerator FlashRed()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            flashCoroutine = null;
        }
    }
}