using System;
using Player.Health;
using UnityEngine;

namespace Systems.Enemies
{
    public class MeleeMovement : EnemyMovement
    {
        [SerializeField] private SpriteRenderer enemySpriteRenderer;

        [Header("Melee Movement")]
        [SerializeField] private Vector3 standOffDistance;
        private Vector3 direction = Vector3.zero;

        [Header("Melee Damage")]
        [SerializeField] private float damageInterval = 3f;
        private float damageTimer;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (enemySpriteRenderer is null) enemySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();
            direction = DirectionToPlayer;

            bool isInStandoff = false;
            var hits = Physics.OverlapBox(transform.position, standOffDistance/2f);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.tag.Contains("Player"))
                    {
                        isInStandoff = true;
                        break;
                    }
                }
            }

            // Standoff from the player
            if (!isInStandoff) // We are outside the standoff distance
            {
                direction = new Vector3(Mathf.Sign(DirectionToPlayer.x), RB.velocity.y, DirectionToPlayer.z);
                damageTimer = 0f;
            }
            else // We are inside the standoff distance
            {
                direction.x = 0;
                direction.y = RB.velocity.y;
                direction.z = 0;

                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    ClosestPlayer.GetComponent<PlayerHealthSystem>().TakeDamage(1);
                    damageTimer = 0f;
                }
            }

            if (enemySpriteRenderer is not null && direction.x != 0)
            {
                enemySpriteRenderer.flipX = direction.x < 0;
            }
        }

        private void FixedUpdate()
        {
            direction = direction.normalized * MoveSpeed;
            RB.velocity = direction;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, standOffDistance);
        }
    }
}