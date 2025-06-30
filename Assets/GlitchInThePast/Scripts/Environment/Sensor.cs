using System;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Roguelike.Objects
{
    public class Sensor : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private float sensorDistance;

        [Header("Conditions")]
        [SerializeField] private bool isSensing = false;
        // [SerializeField] private bool requiresMinTimeSensed = false;
        [Header("Time Based")]
        [SerializeField] private float currentTimeSensed = 0f;
        [SerializeField] private float minTimeSensed = 0.5f;
        // [SerializeField] private float sensedCooldown = -1f;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onSensed;
        [SerializeField] private UnityEvent onTargetChanged;
        
        private void Update()
        {
            if (target == null)
            {
                return;
            }

            if (Vector2.Distance(transform.position, target.position) <= sensorDistance)
            {
                onSensed?.Invoke();
                isSensing = true;
            }
            else
            {
                isSensing = false;
            }

            if (isSensing)
            {
                currentTimeSensed += Time.deltaTime;
                if (currentTimeSensed >= minTimeSensed)
                {
                    
                }
            }
        }

        /// <summary>
        /// Sets the target transform to sense for
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetTarget(Transform newTarget)
        {
            this.target = newTarget;
            onTargetChanged?.Invoke();
        }

        private void OnDrawGizmos()
        {
            // Sensor range. Red when not in range, green when in range
            if (target != null)
            {
                Gizmos.color = Vector2.Distance(transform.position, target.position) <= sensorDistance ? Color.red : Color.green;
            }
            Gizmos.DrawWireSphere(transform.position, sensorDistance);
        }
    }
}