using System;
using UnityEngine;

namespace Systems.Enemies
{
    public class RangedEnemyWeapon : MonoBehaviour
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform projectileSpawn;
        [SerializeField] private float fireRate;
        [SerializeField] private float reloadTime;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float attackRange;
        [SerializeField] private RangedMovement movement;
        
        [Header("Tools")]
        [SerializeField] private Color spawnPreviewColor;
        [SerializeField] private Color rangePreviewColor;
        private void Awake()
        {
            movement = GetComponent<RangedMovement>();
        }

        private void Update()
        {
            if (isPlayerInRange() && reloadTime >= fireRate)
            {
                reloadTime = 0;
                var spawnedProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
                spawnedProjectile.GetComponent<EnemyProjectile>().Init(projectileSpeed, movement.closestPlayer.position);
            }
            else
            {
                reloadTime += Time.deltaTime;
            }
        }

        bool isPlayerInRange()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, movement.closestPlayer.position);
            return distanceToPlayer <= attackRange;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = spawnPreviewColor;
            Gizmos.DrawSphere(projectileSpawn.position, 0.1f);
            
            Gizmos.color = rangePreviewColor;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}