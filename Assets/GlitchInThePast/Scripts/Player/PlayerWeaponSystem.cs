using System;
using System.Collections;
using Systems.Enemies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GlitchInThePast.Scripts.Player
{
    public class PlayerWeaponSystem : MonoBehaviour
    {
        public enum WeaponType
        {
            Melee,
            Ranged
        }
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float rechargeTime = 3f;
        [SerializeField] private bool isRecharging = false;
        [SerializeField] private UnityEvent onStartRecharge;
        [SerializeField] private UnityAction onRecharging;
        [SerializeField] private UnityEvent onEndRecharge;

        [Header("Melee Attack")]
        [SerializeField] private float meleeAttackRange = 3f;
        [SerializeField] private Transform meleeAttackTransform;

        public void OnAttack()
        {
            if (isRecharging)
            {
                Debug.Log($"Recharging...");
                return;
            }
            StartCoroutine(Recharge());
            
            Debug.Log("Player attacking from weapon system script");
            if (weaponType == WeaponType.Melee)
            {
                var hits = Physics.RaycastAll(meleeAttackTransform.position, meleeAttackTransform.forward, meleeAttackRange, LayerMask.GetMask("Enemy"));
                foreach (var hit in hits)
                {
                    Debug.Log(hit.transform.name);
                    EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(1, WeaponType.Melee);
                    }
                }
            }
        }

        private IEnumerator Recharge()
        {
            // Start recharging
            isRecharging = true;
            onStartRecharge?.Invoke();
            Debug.Log("Starting recharge");
            yield return new WaitForSeconds(rechargeTime);
            onEndRecharge?.Invoke();
            Debug.Log("Done recharging");
            isRecharging = false;
            yield break;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(meleeAttackTransform.position, meleeAttackTransform.position + meleeAttackTransform.forward * meleeAttackRange);
        }
    }
}