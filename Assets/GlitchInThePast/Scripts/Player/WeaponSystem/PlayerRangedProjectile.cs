using UnityEngine;
using JW.BeatEmUp.Objects;
using Systems.Enemies;
using UnityEngine.Events;

namespace GlitchInThePast.Scripts.Player
{
    /// <summary>
    /// Author: JW
    /// The bullet for Tobby which handles movement and damage dealing
    /// </summary>
    public class PlayerRangedProjectile : CustomTriggerer
    {
        private Rigidbody rb;
        [HideInInspector] public int Damage;
        public UnityEvent OnEnemyHit;

        /// <summary>
        /// Initialises the bullet with the necesary parameters
        /// </summary>
        /// <param name="damage">How much damage it will deal</param>
        /// <param name="speed">How fast it should move</param>
        public void Init(int damage, float speed)
        {
            Damage = damage;
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.right * speed;
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