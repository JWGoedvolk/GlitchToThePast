using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersDistanceLimiter : MonoBehaviour
{
    [Tooltip("Max distance allowed between the players on the X axis")]
    public float maxXDistance = 10f;

    [Tooltip("Max distance allowed between the players on the Z axis")]
    public float maxZDistance = 5f;

    private GameObject playerOne;
    private GameObject playerTwo;

    void Update()
    {
        if (playerOne == null || playerTwo == null)
        {
            TryAssignPlayers();
            return;
        }

        Vector3 positionOne = playerOne.transform.position;
        Vector3 positionTwo = playerTwo.transform.position;
        Vector3 delta = positionTwo - positionOne;

        if (Mathf.Abs(delta.x) > maxXDistance)
        {
            float clampedX = Mathf.Sign(delta.x) * maxXDistance;
            positionTwo.x = positionOne.x + clampedX;
        }

        if (Mathf.Abs(delta.z) > maxZDistance)
        {
            float clampedZ = Mathf.Sign(delta.z) * maxZDistance;
            positionTwo.z = positionOne.z + clampedZ;
        }
        playerTwo.transform.position = positionTwo;
    }

    private void TryAssignPlayers()
    {
        var allPlayers = PlayerInput.all;
        if (allPlayers.Count >= 2)
        {
            foreach (var player in allPlayers)
            {
                if (player.playerIndex == 0) playerOne = player.gameObject;
                else if (player.playerIndex == 1) playerTwo = player.gameObject;
            }
        }
    }
}
