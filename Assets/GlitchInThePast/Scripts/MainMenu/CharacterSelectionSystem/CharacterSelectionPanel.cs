using UnityEngine;

public class CharacterSelectionPanel : MonoBehaviour
{
    [SerializeField] private InputConnectionManager inputConnectionManager;

    void OnEnable()
    {
        // inputConnectionManager.ResetConfirmations();
        inputConnectionManager.AssignInputs();
    }

    public void OnPlayer1Confirmed()
    {
        // inputConnectionManager.Player1Confirmed = true;
        Debug.Log("Player 1 locked in!");
    }

    public void OnPlayer2Confirmed()
    {
        // inputConnectionManager.Player2Confirmed = true;
        Debug.Log("Player 2 locked in!");
    }
}
