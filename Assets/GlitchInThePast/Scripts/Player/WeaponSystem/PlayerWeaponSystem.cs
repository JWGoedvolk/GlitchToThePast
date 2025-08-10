using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Enemies;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GlitchInThePast.Scripts.Player
{
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
        [Header("Combo")] 
        [SerializeField] private int comboCount = 0;
        [SerializeField] private float timeSinceLastAttack = 0f;
        [SerializeField] private float comboDuration = 0f;
        [ReadOnly][SerializeField] float currentComboDuration = 0f;
        [SerializeField] private List<int> comboMeleeDamage = new List<int>();
        [SerializeField] private BoxCollider comboMeleeCollider = null;
        [SerializeField] private List<int> comboRangedDamage = new List<int>();
        [SerializeField] private List<int> comboRangedSpeed = new List<int>();
        [Header("Events")]
        [Header("Recharge")]
        [SerializeField] private UnityEvent onStartRecharge;
        [SerializeField] private UnityEvent onRecharging;
        [SerializeField] private UnityEvent onEndRecharge;
        [Header("Melee")]
        [SerializeField] private UnityEvent onMeleeAttack;
        public UnityEvent OnComboIncrease;
        public UnityEvent OnComboReset;
        [Header("Ranged")] 
        [SerializeField] private UnityEvent onRangedAttack;

        // Melee Attack
        [SerializeField] private Animator meleeAnimator;
        [SerializeField] private Transform meleeAttackTransform;
        [SerializeField] private Bounds meleeBounds;
        [SerializeField] private float meleeAttackRange = 3f;
        [SerializeField] private float meleeRechargeTime = 3f;
        [SerializeField] private int meleeDamage = 1;

        // Ranged Attack
        [SerializeField] private Animator rangedAnimator;
        [SerializeField] private Transform rangedAttackSpawnPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float rangedRechargeTime = 3f;
        [SerializeField] private int rangedDamage = 1;
        [SerializeField] private float projectileSpeed = 5f;
        
        public WeaponType Weapon { get; }

        //SFX
        private SFXManager sFXManager;
        void Start()
        {
            if(sFXManager == null) 
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
            if (!isWeaponEnabled)
            {
                return;
            }
            
            timeSinceLastAttack += Time.deltaTime;
            if (comboCount != 0 && timeSinceLastAttack > currentComboDuration)
            {
                ResetComboCounter();

                UpdateAnimatorCounter();
            }
        }

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

        private void ResetComboCounter()
        {

            comboCount = 0;
            OnComboReset?.Invoke();
        }

        public void OnAttack()
        {
            // Only allow attacks if our weapon is enabled and not recharging
            if (isRecharging || !isWeaponEnabled)
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

                
                var hits = Physics.BoxCastAll(meleeAttackTransform.position + meleeBounds.center, meleeBounds.extents,meleeAttackTransform.localEulerAngles);
                Debug.Log(hits.Length);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        EnemyHealth eh = hit.transform.gameObject.GetComponent<EnemyHealth>();
                        if (eh != null)
                        {
                            eh.TakeDamage(CurrentComboDamage(), WeaponType.Melee);
                        }
                    }
                }
                
                
                // Invoke events
                onMeleeAttack?.Invoke();

                //sfx
                sFXManager?.PlayWeaponSFX();
                
                // If we have reached the max combo count then reset it
                if (comboCount == 3)
                {
                    ResetComboCounter();
                }
                
                /*var hits = Physics.RaycastAll(meleeAttackTransform.position, meleeAttackTransform.forward, meleeAttackRange);
                foreach (var hit in hits)
                {
                    Debug.Log(hit.transform.name);
                    EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(comboMeleeDamage[comboCount-1], WeaponType.Melee);
                    }
                }*/
            }
            else if (weaponType == WeaponType.Ranged)
            {
                StartCoroutine(Recharge(rangedRechargeTime));
                onRangedAttack?.Invoke();

                //Sfx
                sFXManager?.PlayWeaponSFX();

                var projectile = Instantiate(projectilePrefab, rangedAttackSpawnPoint.position, rangedAttackSpawnPoint.rotation);
                PlayerRangedProjectile projectileScript = projectile.GetComponent<PlayerRangedProjectile>();
                projectileScript.Init(comboRangedDamage[comboCount-1], comboRangedSpeed[comboCount-1]);
            }
        }

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

        public void FlipMeleeTransform(bool isFlipped)
        {
            if (isFlipped)
            {
                meleeFlipperTransform.rotation = Quaternion.Euler(meleeFlipperTransform.rotation.x, 180f, meleeFlipperTransform.rotation.z);
            }
            else
            {
                meleeFlipperTransform.rotation = Quaternion.Euler(meleeFlipperTransform.rotation.x, 0f, meleeFlipperTransform.rotation.z);
            }
        }

        public int CurrentComboDamage()
        {
            return comboMeleeDamage[comboCount - 1];
        }

        public void EnableWeapon()
        {
            isWeaponEnabled = true;
        }

        public void DisableWeapon()
        {
            isWeaponEnabled = false;
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