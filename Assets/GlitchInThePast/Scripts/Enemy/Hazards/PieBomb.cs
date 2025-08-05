using System;
using JW.Objects;
using Player.Health;
using UnityEngine;

namespace Systems.Enemies
{
    public class PieBomb : CustomTriggerer
    {
        // Explosion
        [SerializeField] [Range(0.1f, 10f)] private float explosionRange;
        [SerializeField] [Range(1, 3)] private int damage;
        [Tooltip("This is the time from activating it to the time it blows up")]
        [SerializeField] [Range(0f, 10f)] private float cookTime;
        private bool isCooking = false;
        private float timer;

        private void Update()
        {
            if (!isCooking)
            {
                return;
            }
            
            timer += Time.deltaTime;
            if (timer >= cookTime)
            {
                var hits = Physics.SphereCastAll(transform.position, explosionRange, Vector3.up);
                foreach (var hit in hits)
                {
                    if (Whitelist.Contains(hit.collider.tag))
                    {
                        PlayerHealthSystem playerHealthSystem = hit.collider.GetComponent<PlayerHealthSystem>();
                        playerHealthSystem.TakeDamage(damage);
                    }
                }
                gameObject.SetActive(false);
            }
        }

        public override void OnTrigger(GameObject other)
        {
            if (!isCooking) // If we aren't cooking, we haven't been triggered yet
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();
                renderer.material.color = Color.red;
            }
            
            // Start the cooking time
            isCooking = true;
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
    }
}