using UnityEngine;
using TMPro;

public class HealthDisplayUI : MonoBehaviour
{
    #region Variables
    public enum PlayerID { P1 = 0, P2 = 1 }
    public PlayerID playerID;

    [SerializeField] private TMP_Text healthText;
    #endregion

    public void UpdateHealth(int current, int max)
    {
        if (healthText != null)
        {
            string currentPlayer = playerID == PlayerID.P1 ? "P1" : "P2";
            healthText.text = $"{currentPlayer}: {current} / {max}";
        }
    }
}