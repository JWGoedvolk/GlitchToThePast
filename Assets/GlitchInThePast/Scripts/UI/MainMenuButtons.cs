
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmationPanel;
    [SerializeField] UIBlocker buttonLocker;
    #endregion

    private void Start()
    {
        if (buttonLocker == null) buttonLocker = GetComponent<UIBlocker>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && quitConfirmationPanel.activeSelf != true)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);

            UpdateButtonState();
        }
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
        UpdateButtonState();
    }
    #endregion

    // Quit button
    #region Quitting
    public void QuitGame()
    {
        quitConfirmationPanel.SetActive(true);
        UpdateButtonState();
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void DismissQuit()
    {
        quitConfirmationPanel.SetActive(false);
        UpdateButtonState();
    }
    #endregion

    #region UIBlocking
    private void UpdateButtonState()
    {
        //checks if any pannels are active
        bool anyPannelActive = settingsPanel.activeSelf || quitConfirmationPanel.activeSelf;
        if (anyPannelActive)
        {
            //if they are then call the fucntion from UIBlocker.cs
            buttonLocker?.LockButton();
        }
        else
        {
            //if they are not then call........
            buttonLocker?.UnlockButton();
        }
    }
    #endregion
}
