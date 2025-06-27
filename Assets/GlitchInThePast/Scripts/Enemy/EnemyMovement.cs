using UnityEngine;

namespace Systems.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        public float moveSpeed;
        public Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }
}