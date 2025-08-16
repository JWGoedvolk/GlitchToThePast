using System.Collections;
using System.Collections.Generic;
using GlitchInThePast.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player.Health
{
    public class PlayerHealthSystem : MonoBehaviour
    {
        #region Variables
        public static System.Action<int, PlayerHealthSystem> OnPlayerSpawned;
        [SerializeField] private PlayerWeaponSystem weaponSystem;

        [Header("Player Information")]
        public int currentHealth;
        public int maxHealth;
        [SerializeField] private float respawnInvuln = 1.0f;
        private Coroutine invulnRoutine;
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

        [Header("SFX")]
        [Tooltip("Played when the player takes damage")]
        public AudioClip hitClip;
        [Tooltip("Played when this player dies")]
        public AudioClip deathClip;
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
            if (weaponSystem is null)
            {
                weaponSystem = GetComponentInChildren<PlayerWeaponSystem>();
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
                if (currentHealth != maxHealth)
                {
                    TakeDamage(-1); // Heal by 1
                    // disable collided w object.
                }
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
                    // --- Added: prefer central AudioManager for clean routing; fallback to old SFXManager ---
                    if (Audio.AudioManager.Instance != null && hitClip != null)
                    {
                        Audio.AudioManager.Instance.PlaySFXOneShot(hitClip, 1f);
                    }
                    else
                    {
                        if (sfxManager != null)
                        {
                            //Debug.Log("PlayerHealthSystem: Calling PlayHitSFX()");
                            sfxManager.PlayHitSFX();
                        }
                    }

                    if (animator != null)
                    {
                        animator.SetBool("isGettingHit", true);
                        animator.SetBool("isDashing", false);
                        Invoke("StopHitAnimation", 0.3f);
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

                    if (Audio.AudioManager.Instance != null && deathClip != null)
                    {
                        Audio.AudioManager.Instance.PlaySFXOneShot(deathClip, 1f);
                    }
                    else
                    {
                        sfxManager?.PlayDeathSFX();
                    }

                    animator.SetBool("isGettingHit", true);
                    Invoke("StopHitAnimation", 0.5f);

                    Invoke("Die", 0.5f);
                    return;
                }
            }
        }

        void Die()
        {
            if (spawningManager != null)
            {
                weaponSystem.DisableWeapon();

                // Respawn the player
                spawningManager.SaveDeathLocation(playerInput.playerIndex, transform.position);
                spawningManager.HandleRespawning(playerInput);
            }

            onDeath?.Invoke();
        }
        public void BeginRespawnInvulnerability(float seconds = -1f)
        {
            weaponSystem.EnableWeapon();

            if (seconds <= 0f) seconds = respawnInvuln;

            if (invulnRoutine != null) StopCoroutine(invulnRoutine);
            invulnRoutine = StartCoroutine(RespawnInvuln(seconds));
        }

        private IEnumerator RespawnInvuln(float seconds)
        {
            isInvincible = true;
            lastDamageTime = Time.time + damageCooldown;

            yield return new WaitForSeconds(seconds);

            isInvincible = false;
            invulnRoutine = null;
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