using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using UI.FadingEffect;
using UnityEngine.SceneManagement;
using Audio;

public class MainMenuButtons : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmationPanel;
    [SerializeField] private GameObject loadFeaturePanel;
    [SerializeField] private GameObject firstSettingsButton;
    [SerializeField] private GameObject firstQuitPanelButton;
    [SerializeField] private GameObject firstMainMenuButton;
    [SerializeField] private GameObject firstSceneButton;
    [SerializeField] private GameObject charactersSelectionpanel;

    [SerializeField] private Image substitlesToggleImage;
    [SerializeField] private Sprite subtitlesOnImage;
    [SerializeField] private Sprite SpritesubtitlesOffImage;

    [SerializeField] private TMP_Text sizeLabel;

    [SerializeField] private bool isSelectingCharacters;

    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private PauseMenu pauseMenuScript;
    //button locker
    [SerializeField] UIBlocker buttonLocker;

    //SFX integration
    [SerializeField] public UISfxManager sfxManager;

    private bool startingTransition;

    private bool subtitlesOn;
    private string[] sizes = new string[3];
    private int selectedSize = 0;
    #endregion

    private void Start()
    {
        Time.timeScale = 1f;
        if (isSelectingCharacters)
        {
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

        if (screenFader == null)
        {
            screenFader = FindObjectOfType<ScreenFader>();
        }

        if (pauseMenuScript != null)
            settingsPanel = pauseMenuScript.pauseMenu;
    }

    private void Update()
    {
        if (settingsPanel == null || quitConfirmationPanel == null) return;

        bool escapePressed = Keyboard.current?.escapeKey.wasPressedThisFrame == true;
        bool controllerMenuPressed = Gamepad.current?.startButton.wasPressedThisFrame == true;

        if ((escapePressed || controllerMenuPressed) && !quitConfirmationPanel.activeSelf)
        {
            if (settingsPanel.activeSelf == true) // Panel is now active (was inactive)
            {
                sfxManager?.PlayPannelOpeningSFX();
            }
            else // Panel is now inactive (was active)
            {
                sfxManager?.PlayPannelClosingSFX();
            }

            EventSystem.current.SetSelectedGameObject(null);
            if (settingsPanel.activeSelf)
                EventSystem.current.SetSelectedGameObject(firstSettingsButton);
            else
                EventSystem.current.SetSelectedGameObject(firstMainMenuButton);

            UpdateButtonState();
        }
    }

    #region Set Selected GameObject
    public void SetSelectedGameObject(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(button);
    }
    #endregion

    #region Start Button
    public void StartGame()
    {
        if (startingTransition) return;
        startingTransition = true;

        if (screenFader == null) screenFader = FindObjectOfType<ScreenFader>();

        if (screenFader != null)
        {
            buttonLocker?.LockButtons();
            screenFader.OnButtonClickFadeTransition(1.5f);
            Invoke("LoadCharacterSelectionScene", 1f);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private void LoadCharacterSelectionScene()
    {
        SceneManager.LoadScene(1);
    }
    #endregion

    #region Settings
    public void SettingsToggler()
    {

        if (pauseMenuScript == null)
            pauseMenuScript = PauseMenu.Instance;

        if (pauseMenuScript != null)
            settingsPanel = pauseMenuScript.pauseMenu;

        if (settingsPanel == null)
        {
            Debug.Log("Settings panel is missing");
            return;
        }

        settingsPanel.SetActive(!settingsPanel.activeSelf);

        if (settingsPanel.activeSelf == true) // Panel is now active (was inactive)
        {
            sfxManager?.PlayPannelOpeningSFX();
        }
        else // Panel is now inactive (was active)
        {
            sfxManager?.PlayPannelClosingSFX();
            Time.timeScale = 1f;
        }

        StartCoroutine(SetSelectedNextFrame(settingsPanel.activeSelf ? firstSettingsButton : firstMainMenuButton));

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

    #region Characters Selection
    public void EnableCharacterSelectionPanel()
    {
        sfxManager?.PlayButtonClickSFX();
        sfxManager?.PlayPannelOpeningSFX();

        charactersSelectionpanel.SetActive(true);
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

    #region Quitting
    public void QuitGame()
    {
        sfxManager?.PlayPannelOpeningSFX();

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
        sfxManager?.PlayPannelClosingSFX();

        quitConfirmationPanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMainMenuButton);

        UpdateButtonState();
    }
    #endregion

    #region Selected Button Setter
    private IEnumerator SetSelectedNextFrame(GameObject target)
    {
        yield return null;
        if (EventSystem.current == null) yield break;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
    #endregion

    #region Load Game Panel
    public void LoadGameSection()
    {
        bool isLoadFeaturePanel = !loadFeaturePanel.activeSelf;
        loadFeaturePanel.SetActive(isLoadFeaturePanel);

        sfxManager?.PlayButtonClickSFX();
        if (isLoadFeaturePanel)
        {
            sfxManager?.PlayPannelOpeningSFX();
        }
        else
        {
            sfxManager?.PlayPannelClosingSFX();
        }


        EventSystem.current.SetSelectedGameObject(null);

        if (isLoadFeaturePanel)
        {
            EventSystem.current.SetSelectedGameObject(firstSceneButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(firstMainMenuButton);
        }

        UpdateButtonState();
    }
    #endregion

    #region UIBlocking
    private void UpdateButtonState()
    {
        bool anyPanelActive = settingsPanel.activeSelf || quitConfirmationPanel.activeSelf;
        if (anyPanelActive)
        {
            buttonLocker?.LockButtons();
        }
        else
        {
            buttonLocker?.UnlockButton();
        }
    }
    #endregion
}