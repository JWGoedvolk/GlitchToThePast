using JW.BeatEmUp.Objects;
using JW.Objects;
using UnityEngine;

namespace Systems.Enemies
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyProjectile : CustomTriggerer
    {
        public void Init(float speed, Vector3 targetPosition)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            
            Vector3 dir = targetPosition - rb.position;
            rb.velocity = dir.normalized * speed;
        }

        public override void OnTrigger(GameObject other)
        {
            Destroy(gameObject);
        }
    }
}