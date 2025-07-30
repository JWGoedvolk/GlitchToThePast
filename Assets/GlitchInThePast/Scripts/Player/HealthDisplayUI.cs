using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthDisplayUI : MonoBehaviour
{
    #region Variables
    public enum PlayerID { P1 = 0, P2 = 1 }
    public PlayerID playerID;
    public PlayerHealthSystem playerHealthSystem;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image playerIcon;
    #endregion

    private void Update()
    {
        if (!playerHealthSystem)
        {
            return;
        }
        
        string newHealthText = $"{playerHealthSystem.currentHealth}/{playerHealthSystem.maxHealth}";
        healthText.text = newHealthText;
    }

    public void UpdateHealth(int current, int max)
    {
        // if (healthText != null)
        // {
        //     string currentPlayer = playerID == PlayerID.P1 ? "P1" : "P2";
        //     Debug.Log(currentPlayer + ": " + current);
        //     healthText.text = $"{current}/{max}";
        // }
    }
}