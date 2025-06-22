using System;
using JW.Movement;
using UnityEngine;

namespace JW.Roguelike.Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AnvilPlayerFollower : MonoBehaviour
    {
        // Find the closest player
        // move towards closest player
        // countdown grace period if in a certain distance
        // drop the anvil
        
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D rb;

        [Header("Players")]
        [SerializeField] private Transform[] players;
        private Transform closestPlayer;

        public Transform ClosestPlayer
        {
            get { return closestPlayer; }
            set
            {
                if (closestPlayer != value)
                {
                    fallSensor.SetTarget(value);
                }
                closestPlayer = value;
            }
        }

        [Header("Sensors")] 
        [SerializeField] private Sensor fallSensor;

        private void OnEnable()
        {
            // Get our rigid body
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (closestPlayer == null)
            {
                ClosestPlayer = players[0];
            }
            foreach (var player in players)
            {
                float distance = Vector2.Distance(player.position, transform.position);
                if (distance <= Vector2.Distance(transform.position, closestPlayer.position))
                {
                    ClosestPlayer = player;
                }
            }
        }

        void FixedUpdate()
        {
            Vector2 movDir = Vector2.MoveTowards(transform.position, closestPlayer.position, moveSpeed * Time.deltaTime);
            rb.velocity = movDir.normalized * moveSpeed * Time.deltaTime;
        }
    }
}