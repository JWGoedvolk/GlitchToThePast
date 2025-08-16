using UnityEngine;
using JW.BeatEmUp.Objects;
using Systems.Enemies;
using UnityEngine.Events;

namespace GlitchInThePast.Scripts.Player
{
    /// <summary>
    /// The bullet for Tobby which handles movement and damage dealing
    /// </summary>
    public class PlayerRangedProjectile : CustomTriggerer
    {
        #region Variables
        private Rigidbody rb;
        [HideInInspector] public int Damage;
        public UnityEvent OnEnemyHit;

        [SerializeField] private float maxLife = 3f;
        private float lifeTimer = 0f;
        private ProjectilePool pool;

        public void SetPool(ProjectilePool projectiles) { pool = projectiles; }
        #endregion

        private void OnEnable()
        {
            lifeTimer = 0f;
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (rb != null) rb.velocity = Vector3.zero; 
        }

        private void Update()
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= maxLife)
            {
                Despawn();
            }
        }

        /// <summary>
        /// Initialises the bullet with the necesary parameters
        /// </summary>
        /// <param name="damage">How much damage it will deal</param>
        /// <param name="speed">How fast it should move</param>
        public void Init(int damage, float speed)
        {
            Damage = damage;
            rb = rb == null ? GetComponent<Rigidbody>() : rb;
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

            Despawn();
        }

        private void Despawn()
        {
            if (pool != null) pool.Despawn(this);
            else gameObject.SetActive(false); 
        }
    }
}