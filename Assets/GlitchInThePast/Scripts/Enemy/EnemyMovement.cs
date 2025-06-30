using UnityEngine;
using UnityEngine.InputSystem;

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

            foreach (var pi in PlayerInput.all)
            {
                if (pi.playerIndex == 0) Player1 = pi.gameObject;
                else if (pi.playerIndex == 1) Player2 = pi.gameObject;
            }
        }

        protected virtual void Update()
        {
            GameObject target = null;
            if (Player1 != null && Player2 != null)
            {
                float d1 = Vector2.Distance(transform.position, Player1.transform.position);
                float d2 = Vector2.Distance(transform.position, Player2.transform.position);
                target = d1 < d2 ? Player1 : Player2;
            }
            else if (Player1 != null) target = Player1;
            else if (Player2 != null) target = Player2;

            if (target != null)
            {
                Vector2 dir = (target.transform.position - transform.position).normalized;
                RB.velocity = dir * MoveSpeed;
            }
        }
    }
}
