using System;
using UnityEngine;

namespace Systems.Enemies
{
    public class MeleeMovement : EnemyMovement
    {
        [Header("Melee Movement")]
        [SerializeField] private float standOffDistance;
        protected override void Update()
        {
            base.Update();
            
            float distance = Vector2.Distance(transform.position, ClosestPlayer.transform.position);
            
            if (distance > standOffDistance)
            {
                float direction = 0f;
                if (ClosestPlayer.transform.position.x < transform.position.x) // Player is to the left
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
                RB.velocity = new Vector2(direction * MoveSpeed, RB.velocity.y);
            }
            else
            {
                RB.velocity = new Vector2(0f, RB.velocity.y);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, standOffDistance);
        }
    }
}