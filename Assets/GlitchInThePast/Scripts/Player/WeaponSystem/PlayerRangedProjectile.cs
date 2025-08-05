using UnityEngine;
using JW.Objects;
using Systems.Enemies;
using UnityEngine.Events;

namespace GlitchInThePast.Scripts.Player
{
    public class PlayerRangedProjectile : CustomTriggerer
    {
        private Rigidbody rb;
        [HideInInspector] public int Damage;
        public UnityEvent OnEnemyHit;

        public void Init(int damage, float speed)
        {
            Damage = damage;
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;
        }
        
        public override void OnTrigger(GameObject other)
        {
            Debug.Log("OnTrigger");
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Damage, PlayerWeaponSystem.WeaponType.Ranged);
                OnEnemyHit?.Invoke();
            }
        }
    }
}