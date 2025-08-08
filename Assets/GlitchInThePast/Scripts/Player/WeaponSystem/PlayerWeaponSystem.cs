using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Enemies;
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
        [Header("Combo")] 
        [SerializeField] private int comboCount = 0;
        [SerializeField] private float timeSinceLastAttack = 0f;
        [SerializeField] private float comboDuration = 0f;
        [SerializeField] private List<int> comboMeleeDamage = new List<int>();
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
        public List<UnityEvent> OnComboAttacks;

        // Melee Attack
        [SerializeField] private Animator meleeAnimator;
        [SerializeField] private Transform meleeAttackTransform;
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

        [Header("Combos")] 
        public int ComboCount = 0;
        [Tooltip("This is how long after the rechagre the player has to continue the combo")]
        public float ComboWindow = 1f;
        public float CurrentComboTime = 0f;
        public bool IsComboing = false;
        
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
            if (comboCount != 0 && timeSinceLastAttack > comboDuration)
            {
                OnComboReset?.Invoke();
            }
        }

        public void OnAttack()
        {
            if (isRecharging || !isWeaponEnabled)
            {
                // Debug.Log($"Recharging or weapon is disabled");
                return;
            }
            
            if (timeSinceLastAttack <= comboDuration)
            {
                comboCount++;
                OnComboIncrease?.Invoke();
                OnComboAttacks[comboCount-1]?.Invoke();
            }
            
            if (weaponType == WeaponType.Melee)
            {
                /* COMBO IDEATION
                 * Check if this player is in a combo "state"
                 * If they are, are they still within the combo window. if so do the combo attack at the current counter and increment the counter
                 * If they aren't, start the comboing from this attack on
                 */
                if (IsComboing)
                {
                    
                }
                
                StartCoroutine(Recharge(meleeRechargeTime));
                onMeleeAttack?.Invoke();

                //sfx
                sFXManager?.PlayWeaponSFX();

                var hits = Physics.RaycastAll(meleeAttackTransform.position, meleeAttackTransform.forward, meleeAttackRange);
                foreach (var hit in hits)
                {
                    Debug.Log(hit.transform.name);
                    EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(comboMeleeDamage[comboCount-1], WeaponType.Melee);
                    }
                }
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
            if (weaponType == WeaponType.Melee) Debug.DrawLine(meleeAttackTransform.position, meleeAttackTransform.position + meleeAttackTransform.forward * meleeAttackRange);
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