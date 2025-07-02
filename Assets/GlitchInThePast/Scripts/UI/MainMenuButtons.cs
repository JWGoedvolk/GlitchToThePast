
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmationPanel;

    [SerializeField] private Image substitlesToggleImage;
    [SerializeField] private Sprite subtitlesOnImage;
    [SerializeField] private Sprite SpritesubtitlesOffImage;

    [SerializeField] private TMP_Text sizeLabel;

    //button locker
    [SerializeField] UIBlocker buttonLocker;

    private bool subtitlesOn;
    private string[] sizes = new string[3];
    private int selectedSize = 0;
    #endregion

    private void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Escape) && quitConfirmationPanel.activeSelf != true)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf); // TODO: Use new InputSytem.

            //reagarding ui blockers
            UpdateButtonState();

        }
    }

    // Options toggler with esc
    #region Settings
    public void SettingsToggler()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);


        // calling the fucntion
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

        // calling the fucntion
        UpdateButtonState();
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void DismissQuit()
    {
        quitConfirmationPanel.SetActive(false);

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
