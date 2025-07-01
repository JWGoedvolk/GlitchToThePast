using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Enemies
{
    [RequireComponent(typeof(SphereCollider))]
    public class RangedMovement : EnemyMovement
    {
        [Header("Movement")]
        public float cruisingAltitude; // This is the y position the enemies will try to fly at
        public float cruisingAltitudeError = 0.1f;
        [SerializeField] private Vector2 movDir;
        [Tooltip("This is the total distance of the strafe centered over the closest player's x position")]
        [SerializeField] private float strafeDistance = 0.5f;
        private bool isStrafingLeft = true;
        [SerializeField] private Transform closestPlayer;

        protected override void Awake()
        {
            base.Awake();
            SphereCollider circleCollider = GetComponent<SphereCollider>();
            circleCollider.isTrigger = true;
        }

        protected override void Update()
        {
            // Update the closest player
            base.Update();
            closestPlayer = ClosestPlayer;
            
            // Strafing thresholds
            float distanceFromCenter = transform.position.x - closestPlayer.position.x;
            Debug.Log("distanceFromCenter: " + distanceFromCenter);

            // Custom PingPong based off distance and strafe direction
            if (isStrafingLeft)
            {
                if (distanceFromCenter < -strafeDistance)
                {
                    isStrafingLeft = false;
                }
            }
            else
            {
                if (distanceFromCenter > strafeDistance)
                {
                    isStrafingLeft = true;
                }
            }

            // Moving in the strafe direction
            if (isStrafingLeft)
            {
                movDir.x = -1f;
            }
            else
            {
                movDir.x = 1f;
            }
            
            // Stay at cruising altitude
            if (transform.position.y > cruisingAltitude - 0.1f && transform.position.y < cruisingAltitude + 0.1f) // Check if we are close enough to cruising
            {
                Debug.Log("Close enough to cruising altitude");
                movDir.y = 0f;
            }
            else if (transform.position.y < cruisingAltitude) // Too low so rise up
            {
                movDir.y = 1f;
                Debug.Log("Rising up");
            }
            else if (transform.position.y > cruisingAltitude) // Too high so drop down
            {
                movDir.y = -1f;
                Debug.Log("Dropping down");
            }
            
            // Apply the movement on the normalized move direction
            RB.velocity = movDir.normalized * MoveSpeed;
        }

        private void OnDrawGizmosSelected()
        {
            if (closestPlayer != null)
            {
                Debug.DrawLine(closestPlayer.position + Vector3.up * cruisingAltitude, closestPlayer.position + Vector3.left * strafeDistance + Vector3.up * cruisingAltitude, Color.red);
                Debug.DrawLine(closestPlayer.position + Vector3.up * cruisingAltitude, closestPlayer.position - Vector3.left * strafeDistance + Vector3.up * cruisingAltitude, Color.red);
            }

            Debug.DrawRay(transform.position, movDir.normalized * MoveSpeed, Color.cyan);
            if (closestPlayer != null) Debug.DrawLine(transform.position, closestPlayer.position, Color.green);
        }
    }
}