using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

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

    //button locker
    [SerializeField] UIBlocker buttonLocker;

    private bool subtitlesOn;
    private string[] sizes = new string[3];
    private int selectedSize = 0;

    //Music BG
    [SerializeField] public AudioSource backgroundMusic;
    [SerializeField] [Range(0f, 1f)] private float fadeOutRate = 0.2f;
    #endregion

    //SFX integration
    [SerializeField] public UISfxManager sfxManager;

    private void Start()
    {
        if (isSelectingCharacters)
        {
            return;
        }

        //music only plays once and doesnt restart when somehting else activates
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
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

        bool escapePressed = Keyboard.current?.escapeKey.wasPressedThisFrame == true;
        bool controllerMenuPressed = Gamepad.current?.startButton.wasPressedThisFrame == true;

        if ((escapePressed || controllerMenuPressed) && !quitConfirmationPanel.activeSelf)
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
        StartCoroutine(MusicAndStart());

        //call the sfx on lcick
        sfxManager?.PlayButtonClickSFX();
    }
    #endregion

    #region Settings
    public void SettingsToggler()
    {
        bool settingsActive = !settingsPanel.activeSelf;
        settingsPanel.SetActive(settingsActive);

        //sfx managing
        sfxManager?.PlayButtonClickSFX();

        //asumption that the toogle is always open
        sfxManager?.PlayPannelOpeningSFX();


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

        sfxManager?.PlayButtonClickSFX();

        selectedSize = (selectedSize - 1 + sizes.Length) % sizes.Length;
        UpdateSizeLabel();

    }

    public void GoToNextSize()
    {
        sfxManager?.PlayButtonClickSFX();

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

        sfxManager?.PlayButtonClickSFX();

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
        sfxManager?.PlayButtonClickSFX();
        sfxManager?.PlayPannelOpeningSFX();

        quitConfirmationPanel.SetActive(true);

        // Set focus to first button in quit confirmation panel
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstQuitPanelButton);

        UpdateButtonState();
    }


    public void ConfirmQuit()
    {

        sfxManager?.PlayButtonClickSFX();

        Application.Quit();

    }

    public void DismissQuit()
    {
        sfxManager?.PlayButtonClickSFX();
        sfxManager?.PlayPannelClosingSFX();

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

    #region TEMP
    public void TempFeaturesLoadToggler()
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

    #region Music
    IEnumerator MusicFadingOut()
    {
        //saves og volume so i can rstror later
        float origonalVolume = backgroundMusic.volume;

        //as long as the volume is 0 , keep the bg music on
        while (backgroundMusic.volume > 0)
        {
            backgroundMusic.volume -= fadeOutRate * Time.deltaTime;
            yield return null;
        }

        //once its zero stop THEN reset it next time
        backgroundMusic.Stop();
        backgroundMusic.volume = origonalVolume;

    }

    private IEnumerator MusicAndStart()
    {

        yield return StartCoroutine(MusicFadingOut());

        SceneManager.LoadScene(1);

    }
    #endregion
}