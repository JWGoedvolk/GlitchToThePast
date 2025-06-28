using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        public float MoveSpeed;
        public Rigidbody2D RB;
        public GameObject Player1;
        public GameObject Player2;

        protected virtual void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
            
            SpawningManager spawningManager = FindObjectOfType<SpawningManager>();
            Player1 = spawningManager.player1;
            Player2 = spawningManager.player2;
        }
    }
}