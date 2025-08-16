using System;
using System.Collections.Generic;
using GlitchInThePast.Scripts.Utility;
using UnityEngine;

namespace Systems.Enemies
{
    [RequireComponent(typeof(SphereCollider))]
    public class RangedMovement : EnemyMovement
    {
        [SerializeField] private SpriteRenderer enemySpriteRenderer;

        [SerializeField] private Vector3 movDir;
        [Header("Cruising Altitude")]
        public float CruisingAltitude; // This is the y position the enemies will try to fly at
        public float CruisingAltitudeError = 0.1f;
        [Header("Attack Altitude")]
        [SerializeField] public float AttackAltitude = 1f;
        [Header("Player Detection")]
        private PlayerDetector playerDetection;
        [SerializeField] private Vector3 AttackSize = Vector3.one;

        [Header("Strafing")]
        [SerializeField] private Transform strafePoint;
        [Tooltip("This is the total distance of the strafe centered over the closest player's x position")]
        [SerializeField] private float strafeDistance = 0.5f;
        [SerializeField] private float startStrafeDistance = 0.6f;
        private int strafeDirection = 1;

        public Transform TargetPlayer { get { return ClosestPlayer; } }

        // States
        public enum State
        {
            Chase,              // At cruising altitude, player not in attack range
            AltitudeAdjustment, // Trying to go back to the cruising altitude
            Attack,             // The player is in attack range
            Strafing,           // Will bounce between 2 points on the player
        }
        private State currentState;

        protected override void OnEnable()
        {
            base.OnEnable();
            SphereCollider circleCollider = GetComponent<SphereCollider>();
            circleCollider.isTrigger = true;
            if (enemySpriteRenderer is null) enemySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            playerDetection = GetComponentInChildren<PlayerDetector>();
            playerDetection.OnPlayerChanged += ClosestPlayerChanged;
        }
        private void OnDisable()
        {
            playerDetection.OnPlayerChanged -= ClosestPlayerChanged;
        }

        protected override void Update()
        {
            // Update the closest player
            base.Update();

            // Keep the player detector on the ground
            var hits = Physics.RaycastAll(transform.position, Vector3.down);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    // Skip all non ground hits
                    if (hit.collider.tag != "Ground")
                    {
                        continue;
                    }

                    // Set the player detector's position to be on the ground
                    playerDetection.transform.position = new Vector3(playerDetection.transform.position.x, hit.transform.position.y + .5f, playerDetection.transform.position.z);
                }
            }

            UpdateState(); // Update what state the enemy is in to know what behavior to follow

            // Always move towards the closest player
            movDir = DirectionToPlayer.normalized;

            switch (currentState)
            {
                case State.Chase: // Move on the same height towards the closest player
                    movDir.y = 0; // move towards the player while staying at the current height
                    break;
                case State.AltitudeAdjustment: // Move up or down to the desired cruising altitude, still moves towards the closest player horizontally
                    if (transform.position.y < CruisingAltitude - CruisingAltitudeError) // rise up if too low
                    {
                        movDir.y = 1f;
                    }
                    else if (transform.position.y > CruisingAltitude + CruisingAltitudeError) // Drop down if too high
                    {
                        movDir.y = -1f;
                    }
                    break;
                case State.Attack: // Drops down to the attacking altitude and stay there as long as the player is in attack range
                    Attack();
                    break;
                case State.Strafing:
                    Strafe();
                    break;
            }

            if (enemySpriteRenderer is not null && movDir.x != 0f)
            {
                enemySpriteRenderer.flipX = movDir.x < 0f;
            }
        }

        /// <summary>
        /// Drops straight down to attack altitude with no horizontal movement
        /// Then starts moving horizontally to the closest player
        /// </summary>
        private void Attack()
        {
            if (transform.position.y > AttackAltitude) // Drop to attack altitude
            {
                movDir.y = -1f;
            }
            else // We are at attack altitude so start moving horizontally with no vertical
            {
                movDir.y = 0f;
            }
        }

        /// <summary>
        /// Will cause the enemy to move left and right in the x plane around the closest player in a ping pong manouver
        /// </summary>
        private void Strafe()
        {
            // Calculate offset
            float distanceFromCenter = strafePoint.position.x - playerDetection.ClosestPlayerInRange.transform.position.x;

            if (strafeDirection == 1)
            {
                if (distanceFromCenter > strafeDistance)
                {
                    strafeDirection = -1;
                }
            }
            else
            {
                if (distanceFromCenter < -strafeDistance)
                {
                    strafeDirection = 1;
                }
            }

            movDir.y = 0f;
            movDir.x = strafeDirection;
        }

        public void ClosestPlayerChanged()
        {
            if (strafePoint.position.x > playerDetection.ClosestPlayerInRange.transform.position.x) // The player is to the left of us
            {
                strafeDirection = -1; // Strafe left towards the player
            }
            else
            {
                strafeDirection = 1; // Strafe right towards the player
            }
        }

        private void FixedUpdate()
        {
            // Apply the movement on the normalized move direction
            RB.velocity = movDir.normalized * MoveSpeed;
        }

        private void UpdateState()
        {
            State newState = currentState;
            // Are we in attack range of the player?
            if (playerDetection.IsPlayerInRange)
            {
                newState = State.Attack;

                // Check if the player is in strafing distance and that we are at the attack altitude
                if (Mathf.Abs(Vector3.Distance(strafePoint.position, playerDetection.ClosestPlayerInRange.transform.position)) <=
                    strafeDistance + startStrafeDistance && transform.position.y <= AttackAltitude)
                {
                    if (currentState == State.Strafing)
                    {
                        return;
                    }

                    newState = State.Strafing;
                    if (newState != currentState) // Switch to strafing if it's not already
                    {
                        currentState = newState;
                        playerDetection.SetDetectionSize(AttackSize);
                        // Debug.Log($"{name} switched to Strafing mode");
                        if (playerDetection.ClosestPlayerInRange.transform.position.x < strafePoint.position.x) // if the player is to the left of us
                        {
                            strafeDirection = -1; // Strafe left
                        }
                        else // If it is to the right of us
                        {
                            strafeDirection = 1; // Strafe right
                        }

                        return; // We don't care about continuing more logic for attack state switching so we leave
                    }
                }

                // Switch to attack state if we are outside strafing distance and not already in attack state
                if (newState != currentState)
                {
                    // Debug.Log($"{name} switched to Attack mode");
                    currentState = newState;
                    playerDetection.SetDetectionSize(AttackSize);
                }
            }
            else
            {
                // If we ar not in range, are we at cruising altitude?
                if (transform.position.y < CruisingAltitude - CruisingAltitudeError || transform.position.y > CruisingAltitude + CruisingAltitudeError)
                {
                    newState = State.AltitudeAdjustment;
                    if (newState != currentState)
                    {
                        // Debug.Log($"{name} switched to Altitude Adjustment mode");
                        currentState = newState;
                        playerDetection.ResetDetectionSize();
                    }
                }
                else // We are at cruising altitude and no player is in range
                {
                    newState = State.Chase;
                    if (newState != currentState)
                    {
                        // Debug.Log($"{name} switched to Chase mode");
                        currentState = newState;
                        playerDetection.ResetDetectionSize();
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // if (ClosestPlayer != null)
            // {
            //     Debug.DrawLine(ClosestPlayer.position + Vector3.up * CruisingAltitude, ClosestPlayer.position + Vector3.left * strafeDistance + Vector3.up * CruisingAltitude, Color.red);
            //     Debug.DrawLine(ClosestPlayer.position + Vector3.up * CruisingAltitude, ClosestPlayer.position - Vector3.left * strafeDistance + Vector3.up * CruisingAltitude, Color.red);
            // }

            // Move in this direction
            Debug.DrawRay(transform.position, movDir.normalized * MoveSpeed, Color.cyan);

            // Closest player being targeted
            if (ClosestPlayer != null) Debug.DrawLine(transform.position, ClosestPlayer.position, Color.green);

            // Cruising altitude difference
            Debug.DrawLine(Vector3.up * transform.position.y, Vector3.up * CruisingAltitude, Color.red);

            // Strafing markers
            Debug.DrawLine(strafePoint.position, strafePoint.position + Vector3.left * strafeDistance, Color.yellow);
            Debug.DrawLine(strafePoint.position, strafePoint.position + -Vector3.left * strafeDistance, Color.yellow);
        }
    }
}