using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.Enemies;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GlitchInThePast.Scripts.Player
{
    /// <summary>
    /// Author: JW developed core system, Dana implemented pausing, Youssef implemented sound effects
    /// Handles the attacking of the melee and ranged player including combos
    /// </summary>
    public class PlayerWeaponSystem : MonoBehaviour, IPauseable
    {
        public enum WeaponType
        {
            Melee,
            Ranged,
            None
        }

        [Header("General")]
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private bool isRecharging = false;
        [SerializeField] private bool isWeaponEnabled = true;
        [SerializeField] private Transform meleeFlipperTransform;
        [SerializeField] private SpriteRenderer weaponRenderer;

        [Header("Combo")]
        [SerializeField] private int comboCount = 0;
        [SerializeField] private float timeSinceLastAttack = 0f;
        [SerializeField] private float comboDuration = 0f;
        [ReadOnly][SerializeField] float currentComboDuration = 0f;

        [Header("Events")]
        [Header("Recharge")]
        [SerializeField] private UnityEvent onStartRecharge;
        [SerializeField] private UnityEvent onRecharging;
        [SerializeField] private UnityEvent onEndRecharge;
        [Header("Melee")]
        [SerializeField] private UnityEvent onMeleeAttack;
        [Header("Ranged")]
        [SerializeField] private UnityEvent onRangedAttack;
        [Header("Combo")]
        public UnityEvent OnComboIncrease;
        public UnityEvent OnComboReset;

        [Header("Melee Attack")]
        [SerializeField] private Animator meleeAnimator;
        [SerializeField] private Transform meleeAttackTransform;
        [SerializeField] private Bounds meleeBounds;
        // [SerializeField] private float meleeAttackRange = 3f;
        [SerializeField] private float meleeRechargeTime = 3f;
        // [SerializeField] private int meleeDamage = 1;
        [Header("Melee Combo")]
        [SerializeField] private List<int> comboMeleeDamage = new List<int>();

        [Header("Ranged Attack")]
        [SerializeField] private Animator rangedAnimator;
        [SerializeField] private Transform rangedAttackSpawnPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float rangedRechargeTime = 3f;
        // [SerializeField] private int rangedDamage = 1;
        // [SerializeField] private float projectileSpeed = 5f;
        [Header("Ranged Combo")]
        public bool IsCharging = false;
        public List<float> ChargeThresholds;
        public float CurrentChargeTime = 0f;
        [SerializeField] private List<int> comboRangedDamage = new List<int>();
        [SerializeField] private List<int> comboRangedSpeed = new List<int>();

        public WeaponType Weapon { get; }

        //SFX
        private SFXManager sFXManager;
        void Start()
        {
            if (sFXManager == null)
            {
                sFXManager = FindAnyObjectByType<SFXManager>();
            }

            InGameButtons.Instance?.RegisterPauseable(this);
        }

        void OnDestroy()
        {
            InGameButtons.Instance?.UnregisterPauseable(this);
        }

        void Update()
        {
            if (!isWeaponEnabled) // We aren't usable so don't bother continuing
            {
                return;
            }

            switch (weaponType) // Weapon behaviours
            {
                case WeaponType.Melee:
                    timeSinceLastAttack += Time.deltaTime;
                    if (comboCount != 0 && timeSinceLastAttack > currentComboDuration && weaponType != WeaponType.Ranged) // Ranged weapons have a charging combo which works differently
                    {
                        // we are outside the combo window so reset the counter
                        ResetComboCounter();
                        UpdateAnimatorCounter();
                    }
                    break;
                case WeaponType.Ranged:
                    if (IsCharging)
                    {
                        CurrentChargeTime += Time.deltaTime; // Charge up our shot this frame

                        if (CurrentChargeTime >= ChargeThresholds[comboCount]) // Check if we should go to next combo stage
                        {
                            CurrentChargeTime = 0f;
                            comboCount++;
                            OnComboIncrease?.Invoke();

                            if (comboCount == 4) // Loop back after the final stage to the first stage
                            {
                                comboCount = 1;
                            }

                            UpdateAnimatorCounter();
                        }
                    }
                    else
                    {
                        // Keep things in check in the animator state machine
                        rangedAnimator.SetBool("IsCharging", false);
                        rangedAnimator.SetInteger("ComboCount", 0);
                    }
                    break;
            }
        }

        /// <summary>
        /// Updates the animator of the respective weapon being held with the new combo count
        /// </summary>
        private void UpdateAnimatorCounter()
        {
            if (weaponType == WeaponType.Melee)
            {
                meleeAnimator.SetInteger("ComboCount", comboCount);
            }
            else if (weaponType == WeaponType.Ranged)
            {
                rangedAnimator.SetInteger("ComboCount", comboCount);
            }
        }

        /// <summary>
        /// Resets the combo counter to 0, calls the neccesary events, and updates the animator parameters
        /// </summary>
        private void ResetComboCounter()
        {
            comboCount = 0;
            OnComboReset?.Invoke();
            UpdateAnimatorCounter();
        }

        /// <summary>
        /// Called when the attack action is performed. Subscribed and unsubscribed from in PlayerMovement
        /// This is really only used for the melee attack as it is an instantaneous event while the ranged combo is a charged event
        /// </summary>
        public void OnAttack()
        {
            // Only allow attacks if our weapon is enabled and not recharging
            if (isRecharging || !isWeaponEnabled || weaponType == WeaponType.Ranged)
            {
                return;
            }

            if (comboCount > 0) // We are in a combo so see if it was within the combo window
            {
                // Check if we have attacked within the combo window
                if (timeSinceLastAttack <= currentComboDuration)
                {
                    timeSinceLastAttack = 0f; // update attack time

                    // Increase combo counter and trigger attack
                    comboCount++;
                    OnComboIncrease?.Invoke();

                    UpdateAnimatorCounter();
                }
            }
            else // We have no combo count so this starts the combo
            {
                timeSinceLastAttack = 0f;
                comboCount++;
                OnComboIncrease?.Invoke();

                UpdateAnimatorCounter();
            }

            if (weaponType == WeaponType.Melee)
            {
                // Start recharging
                StartCoroutine(Recharge(meleeRechargeTime));

                // Trigger the attack animation
                meleeAnimator.SetTrigger("Attack");

                // Get an adjusted combo duration window
                currentComboDuration = comboDuration + meleeRechargeTime; // The combo window is the time after recharging that the player needs to attack in

                // Get all the enemies hit by the attack and deal damage to them based on combo progression
                var hits = Physics.BoxCastAll(meleeAttackTransform.position + meleeBounds.center, meleeBounds.extents, meleeAttackTransform.localEulerAngles);
                Debug.Log(hits.Length);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        EnemyHealth eh = hit.transform.gameObject.GetComponent<EnemyHealth>();
                        if (eh != null)
                        {
                            eh.TakeDamage(CurrentComboDamage(), WeaponType.Melee);
                        }
                    }
                }

                // Invoke weapon events
                onMeleeAttack?.Invoke();

                //sfx
                sFXManager?.PlayWeaponSFX();

                // If we have reached the max combo count then reset it
                if (comboCount == 3)
                {
                    ResetComboCounter();
                }
            }
        }

        /// <summary>
        /// Called when we begin holding down the attack button for ranged players
        /// </summary>
        public void StartCharging()
        {
            // Only continue if we have the right weapon, it is enabled, and not on cooldown
            if (weaponType != WeaponType.Ranged) return;
            if (isRecharging || !isWeaponEnabled)
            {
                return;
            }

            IsCharging = true;
            CurrentChargeTime = 0f;
            comboCount = 0;
            UpdateAnimatorCounter();
            rangedAnimator.SetBool("IsCharging", true);
        }

        /// <summary>
        /// Called when we release the attack button as the ranged player
        /// </summary>
        public void StopCharging()
        {
            if (weaponType != WeaponType.Ranged) return; // Disregard if we are the melee character

            // Only continue if we have actually charged something
            if (comboCount > 0)
            {
                // Spawn in the projectile
                var projectile = Instantiate(projectilePrefab, rangedAttackSpawnPoint.position, rangedAttackSpawnPoint.rotation);
                PlayerRangedProjectile proj = projectile.GetComponent<PlayerRangedProjectile>();
                proj.Init(comboRangedDamage[comboCount - 1], comboRangedSpeed[comboCount - 1]);

                // Begin weapon recharging
                StartCoroutine(Recharge(rangedRechargeTime));

                // Set animator to play the firing animation
                rangedAnimator.SetBool("IsCharging", false);
                rangedAnimator.SetTrigger("Attack");

                // Weapon events
                onRangedAttack?.Invoke();
            }

            // Reset charging and combo everytime regardless of combo level
            IsCharging = false;
            CurrentChargeTime = 0f;
            ResetComboCounter();
        }

        /// <summary>
        /// Handles weapon cooldowns
        /// </summary>
        /// <param name="time">How long to stay on cooldown</param>
        /// <returns></returns>
        private IEnumerator Recharge(float time)
        {
            timeSinceLastAttack = 0f;

            // Start recharging
            isRecharging = true;
            onStartRecharge?.Invoke();
            Debug.Log("Starting recharge");
            yield return new WaitForSeconds(time);
            onEndRecharge?.Invoke();
            Debug.Log("Done recharging");
            isRecharging = false;
            yield break;
        }

        /// <summary>
        /// Function to easily keep the melee player's attack transform where it should be with moving back and forth
        /// </summary>
        /// <param name="isFlipped">the newly set state of the SpriteRenderer</param>
        public void FlipMeleeTransform(bool isFlipped)
        {
            if (meleeFlipperTransform != null)
            {
                meleeFlipperTransform.rotation = Quaternion.Euler(meleeFlipperTransform.rotation.x, isFlipped ? 180f : 0f, meleeFlipperTransform.rotation.z);
            }
        }

        /// <summary>
        /// Gives the melee damage in the current combo phase
        /// </summary>
        /// <returns>int: damage to be dealt</returns>
        public int CurrentComboDamage()
        {
            return comboMeleeDamage[comboCount - 1];
        }

        /// <summary>
        /// Allows the weapon to be used
        /// </summary>
        public void EnableWeapon()
        {
            isWeaponEnabled = true;
            weaponRenderer.enabled = true;
        }

        /// <summary>
        /// Stops the weapon from being used
        /// </summary>
        public void DisableWeapon()
        {
            isWeaponEnabled = false;
            weaponRenderer.enabled = false;
        }

        private void OnDrawGizmos()
        {
            if (weaponType == WeaponType.Melee)
            {
                // Debug.DrawLine(meleeAttackTransform.position, meleeAttackTransform.position + meleeAttackTransform.forward * meleeAttackRange);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(meleeAttackTransform.position + meleeBounds.center, meleeBounds.size);
            }
        }


        #region IPauseable functions
        public void OnPause()
        {
            enabled = false;
        }

        public void OnUnpause()
        {
            enabled = true;
        }
        #endregion
    }
}