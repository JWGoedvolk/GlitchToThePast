using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmationPanel;
    [SerializeField] private GameObject firstSettingsButton;
    [SerializeField] private GameObject firstQuitPanelButton;
    [SerializeField] private GameObject firstMainMenuButton;
    [SerializeField] private GameObject charactersSelectionpanel;

    [SerializeField] private Image substitlesToggleImage;
    [SerializeField] private Sprite subtitlesOnImage;
    [SerializeField] private Sprite SpritesubtitlesOffImage;

    [SerializeField] private TMP_Text sizeLabel;

    [SerializeField] private bool isSelectingCharacters;

    //button locker
    [SerializeField] UIBlocker buttonLocker;

    private bool subtitlesOn;
    private string[] sizes = new string[3];
    private int selectedSize = 0;
    #endregion

    private void Start()
    {
        if (isSelectingCharacters)
        {
            charactersSelectionpanel.SetActive(true);
            return;
        }

        if (sizes is null) return;
        sizes[0] = "SMALL";
        sizes[1] = "MEDIUM";
        sizes[2] = "LARGE";

        //auto assigning the buttons since they are seralized
        if (buttonLocker == null)
        {
            buttonLocker = GetComponent<UIBlocker>();
        }
    }

    private void Update()
    {
        if (settingsPanel == null || quitConfirmationPanel == null) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !quitConfirmationPanel.activeSelf || Input.GetButtonDown("Submit") && !quitConfirmationPanel.activeSelf)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);

            EventSystem.current.SetSelectedGameObject(null);
            if (settingsPanel.activeSelf)
                EventSystem.current.SetSelectedGameObject(firstSettingsButton);
            else
                EventSystem.current.SetSelectedGameObject(firstMainMenuButton);

            UpdateButtonState();
        }

    }

    #region Start Button
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    #endregion

    // Options toggler with esc
    #region Settings
    public void SettingsToggler()
    {
        bool settingsActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(settingsActive);

        EventSystem.current.SetSelectedGameObject(null);

        if (settingsActive)
        {
            EventSystem.current.SetSelectedGameObject(firstSettingsButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(firstMainMenuButton);
        }

        // Calling the function.
        UpdateButtonState();
    }


    public void GoToPreviousSize()
    {
        selectedSize = (selectedSize - 1 + sizes.Length) % sizes.Length;
        UpdateSizeLabel();
    }

    public void GoToNextSize()
    {
        selectedSize = (selectedSize + 1) % sizes.Length;
        UpdateSizeLabel();
    }

    private void UpdateSizeLabel()
    {
        if (sizeLabel is not null)
        {
            sizeLabel.text = sizes[selectedSize];
        }
    }
    #endregion

    #region Subtitles Toggler
    // Just a visual changer
    public void OnSubtitlesToggle()
    {
        subtitlesOn = !subtitlesOn;
        if (substitlesToggleImage is not null)
        {
            substitlesToggleImage.sprite = subtitlesOn ? subtitlesOnImage : SpritesubtitlesOffImage;
        }
    }
    #endregion
    // Quit button
    #region Quitting
    public void QuitGame()
    {
        quitConfirmationPanel.SetActive(true);

        // Set focus to first button in quit confirmation panel
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstQuitPanelButton);

        UpdateButtonState();
    }


    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void DismissQuit()
    {
        quitConfirmationPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMainMenuButton);

        // calling the fucntion
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