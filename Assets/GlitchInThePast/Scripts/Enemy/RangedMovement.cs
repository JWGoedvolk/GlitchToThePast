using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Enemies
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class RangedMovement : EnemyMovement
    {
        [Header("Movement")]
        public float cruisingAltitude; // This is the y position the enemies will try to fly at
        [SerializeField] private Vector2 movDir;
        
        [Header("Player detection")]
        [SerializeField] private bool playerIsInRange = false;
        [SerializeField] private float playerDetectionRadius = 3f;
        [SerializeField] private List<Transform> playersInRange;
        [SerializeField] private Transform playerTransform;

        protected override void Awake()
        {
            base.Awake();
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = playerDetectionRadius;
        }

        void Update()
        {
            // Check if a player is close enough to start divebombing
            if (playerIsInRange)
            {
                Debug.Log("player is in range");
                // Get the closest player in range
                Transform closestPlayer = playerTransform;
                foreach (Transform player in playersInRange)
                {
                    float distanceToClosestPlayer = Vector2.Distance(playerTransform.position, transform.position);
                    float distanceToPlayer = Vector2.Distance(player.position, transform.position);
                    if (distanceToPlayer < distanceToClosestPlayer)
                    {
                        closestPlayer = player;
                        playerTransform = closestPlayer;
                    }
                }
                movDir = playerTransform.position - transform.position;
                movDir = movDir.normalized;
            }
            else // If no player is in range
            {
                Debug.Log("No player in range");
                if (transform.position.y > cruisingAltitude - Single.Epsilon*2 && transform.position.y < cruisingAltitude + Single.Epsilon*2) // Check if we are close enough to cruising
                {
                    Debug.Log("Close enough to cruising altitude");
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

                movDir.x = -1f;
            }
            
            // Apply the movement on the normalized move direction
            rb.velocity = movDir.normalized * moveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                playerIsInRange = true;
                if (!playersInRange.Contains(other.transform))
                {
                    playersInRange.Add(other.transform);
                    if (playerTransform == null) // Assign the triggering transform as the closest player if it isn't already set
                    {
                        playerTransform = other.transform;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                playersInRange.Remove(other.transform);
                if (playersInRange.Count == 0)
                {
                    playerIsInRange = false;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, movDir.normalized * moveSpeed, Color.red);
            if (playerTransform != null) Debug.DrawLine(transform.position, playerTransform.position, Color.green);
        }
    }
}