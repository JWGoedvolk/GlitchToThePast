using System;
using UnityEngine;

namespace Systems.Enemies
{
    public class MeleeMovement : EnemyMovement
    {
        private void Update()
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }
}