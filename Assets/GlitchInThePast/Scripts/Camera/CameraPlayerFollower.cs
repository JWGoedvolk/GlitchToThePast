using UnityEngine;

namespace CameraScripts
{
    public class CameraPlayerFollower : MonoBehaviour
    {
        [SerializeField] private string player1Tag = "Player1";
        [SerializeField] private string player2Tag = "Player2";

        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private float minX = -400f;
        [SerializeField] private float maxX = 400f;

        [SerializeField] private Transform player1;
        [SerializeField] private Transform player2;
        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            if (player1 == null)
            {
                GameObject foundPlayer1 = GameObject.FindGameObjectWithTag(player1Tag);
                if (foundPlayer1 != null)
                    player1 = foundPlayer1.transform;
            }

            if (player2 == null)
            {
                GameObject foundPlayer2 = GameObject.FindGameObjectWithTag(player2Tag);
                if (foundPlayer2 != null)
                    player2 = foundPlayer2.transform;
            }

            if (player1 == null || player2 == null)
                return;

            float midpointX = (player1.position.x + player2.position.x) / 2f;
            midpointX = Mathf.Clamp(midpointX, minX, maxX);

            Vector3 targetPosition = new Vector3(midpointX, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
