
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Starts NarrativeLevel
    #region New Game Button
    public void StartNewGame()
    {
        // CharacterSelection.SetActive(true); // TODO: Let there be a way to exit the selection screen which also results in deleting the new game file.
        SceneManager.LoadScene(1);
    }
    #endregion

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
