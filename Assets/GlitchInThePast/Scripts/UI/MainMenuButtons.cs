
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmationPanel;

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && quitConfirmationPanel.activeSelf != true) settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // Options toggler with esc
    #region Settings
    public void SettingsToggler()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    #endregion

    // Quit button
    #region Quitting
    public void QuitGame()
    {
        quitConfirmationPanel.SetActive(true);
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void DismissQuit()
    {
        quitConfirmationPanel.SetActive(false);
    }
    #endregion
}
