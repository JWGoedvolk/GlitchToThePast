using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player.Health
{
    public class PlayerHealthSystem : MonoBehaviour
    {
        #region Variables
        public static System.Action<int, PlayerHealthSystem> OnPlayerSpawned;

        [Header("Player Information")]
        public int currentHealth;
        public int maxHealth;
        public SpawningManager spawningManager;
        [HideInInspector] public bool isInvincible = false;

        [Header("Unity Events")]
        public UnityEvent onDamageTaken;
        public UnityEvent onDeath;

        [Header("Collision Tags")]
        [SerializeField] private List<string> damageableTags = new List<string>();
        [SerializeField] private List<string> healableTags = new List<string>();

        [HideInInspector] private Animator animator;

        [SerializeField] private float damageCooldown = 0.3f;
        [HideInInspector] public float lastDamageTime = -Mathf.Infinity;

        private PlayerInput playerInput;
        private HealthDisplayUI healthUI;

        //sfx
        private SFXManager sfxManager;
        #endregion

        void Awake()
        {
            if (playerInput is null) playerInput = GetComponent<PlayerInput>();
            if (sfxManager is null) sfxManager = FindObjectOfType<SFXManager>();
        }

        void Start()
        {
            #region Make sure these tags exist in DamageableTag List
            AddToDamageableTag("Enemy");
            AddToDamageableTag("Hazard");
            AddToDamageableTag("Laser");
            #endregion

            currentHealth = maxHealth;
            OnPlayerSpawned?.Invoke(playerInput.playerIndex, this);

            if (animator is null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            HealthDisplayUI[] allDisplays = FindObjectsOfType<HealthDisplayUI>();
            foreach (var display in allDisplays)
            {
                if ((int)display.playerID == playerInput.playerIndex)
                {
                    healthUI = display;
                    break;
                }
            }

            UpdateUI();
        }

        private void OnTriggerEnter(Collider collision)
        {
            // Debug.Log($"Triggered by: {collision.tag}");

            if (damageableTags.Contains(collision.tag))
            {
                if (Time.time - lastDamageTime >= damageCooldown)
                {
                    lastDamageTime = Time.time;
                    TakeDamage(1);
                }
            }
            else if (healableTags.Contains(collision.tag))
            {
                TakeDamage(-1); // Heal by 1
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (damageableTags.Contains(collision.tag))
            {
                Invoke("StopHitAnimation", 0.3f);
            }
        }

        private void UpdateUI()
        {
            if (healthUI != null)
                healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        public void TakeDamage(int ammount)
        {
            if (isInvincible) return;
            if (currentHealth > 0)
            {
                // Debug.Log($"PlayerHealthSystem: TakeDamage called with amount: {ammount}");
                if (ammount > 0)
                {
                    if (sfxManager != null)
                    {
                        //Debug.Log("PlayerHealthSystem: Calling PlayHitSFX()");
                        sfxManager.PlayHitSFX();
                        if (animator != null)
                        {
                            animator.SetBool("isGettingHit", true);
                        }
                    }
                    else
                    {
                        //Debug.LogWarning("PlayerHealthSystem: sfxManager reference is null!");
                    }
                }

                currentHealth -= ammount;
                UpdateUI();

                onDamageTaken?.Invoke();
                // Debug.Log("player is hit");

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    UpdateUI();

                    sfxManager?.PlayDeathSFX();
                    animator.SetBool("isGettingHit", true);

                    Invoke("Die", 0.5f);
                    return;
                }
            }
            //play sfx when damged
        }

        void Die()
        {
            if (spawningManager != null)
            {
                spawningManager.SaveDeathLocation(playerInput.playerIndex, transform.position);
                spawningManager.HandleRespawning(playerInput);
            }

            onDeath?.Invoke();
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
            UpdateUI();
            // if (flashingEffect != null) flashingEffect.enabled = true;
        }

        public void AssignUI(HealthDisplayUI ui)
        {
            healthUI = ui;
            // SpriteRenderer sr = ui.GetComponentInChildren<SpriteRenderer>();
            // healthUI.SetUp(sr.sprite);

            // Debug.Log($"{healthUI} was assigned to {transform.name}");

            UpdateUI();
        }
        #region Private Functions
        private void AddToDamageableTag(string tagName)
        {
            if (damageableTags.Contains(tagName))
                return;
            else
                damageableTags.Add(tagName);
        }

        private void StopHitAnimation()
        {
            if (animator != null)
            {
                animator.SetBool("isGettingHit", false);
            }
        }
        #endregion
    }
}