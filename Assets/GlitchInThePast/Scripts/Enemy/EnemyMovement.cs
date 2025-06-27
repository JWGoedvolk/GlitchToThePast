using UnityEngine;

namespace Systems.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        public float moveSpeed;
        public Rigidbody2D rb;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }
}