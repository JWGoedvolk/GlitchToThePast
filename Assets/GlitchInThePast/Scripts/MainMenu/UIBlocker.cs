using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlocker : MonoBehaviour
{
    [SerializeField] public Button[] mainMenuButtons;


    //lock the buttons reagrdign the ones int he serlized field
    public void LockButton()
    {
        foreach (var button in mainMenuButtons) 
        {
            button.interactable = false;
        }

        Debug.Log("locked being called");
    }

    //unlock them
    public void UnlockButton()
    {
        foreach(var button in mainMenuButtons)
        {
            button.interactable = true;
        }

        Debug.Log("unlockign in progress");
    }
}
