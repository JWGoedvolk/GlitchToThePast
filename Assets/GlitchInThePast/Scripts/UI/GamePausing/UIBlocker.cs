using UnityEngine;
using UnityEngine.UI;

public class UIBlocker : MonoBehaviour
{
    [SerializeField] public Button[] mainMenuButtons;
    private bool isButtonLocked;

    private void Update()
    {
        if (isButtonLocked && Input.GetKeyDown(KeyCode.KeypadEnter))
            Debug.Log("Buttons Are Locked!!");
    }

    //lock the buttons reagrdign the ones int he serlized field
    public void LockButtons()
    {
        foreach (var button in mainMenuButtons)
        {
            button.interactable = false;
            isButtonLocked = true;
        }

        Debug.Log("locking the buttons.");
    }

    //unlock them
    public void UnlockButton()
    {
        foreach (var button in mainMenuButtons)
        {
            button.interactable = true;
            isButtonLocked = false;
        }

        Debug.Log("Unlocking the buttons.");
    }
}