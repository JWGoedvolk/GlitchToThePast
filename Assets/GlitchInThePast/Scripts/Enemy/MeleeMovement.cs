using System;
using UnityEngine;

namespace Systems.Enemies
{
    public class MeleeMovement : EnemyMovement
    {
        private void Update()
        {
            RB.velocity = new Vector2(MoveSpeed, RB.velocity.y);
        }
    }
}