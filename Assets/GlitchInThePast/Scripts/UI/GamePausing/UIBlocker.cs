using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBlocker : MonoBehaviour
{
    #region Variables
    [SerializeField] public Button[] mainMenuButtons;
    private bool isButtonLocked;

    [SerializeField] private GameObject firstSelectedOnUnlock;
    [SerializeField] private GameObject fallbackSelectedWhenLocked;

    private readonly HashSet<GameObject> openPanels = new HashSet<GameObject>();
    #endregion

    public void ReportPanelOpened(GameObject panel)
    {
        if (panel == null) return;
        if (openPanels.Add(panel))
            RecomputeLock();
    }

    public void ReportPanelClosed(GameObject panel)
    {
        if (panel == null) return;
        if (openPanels.Remove(panel))
        {
            RecomputeLock();
        }
    }

    private void RecomputeLock()
    {
        bool shouldLock = openPanels.Count > 0;
        if (shouldLock && !isButtonLocked) LockButtons();
        else if (!shouldLock && isButtonLocked) UnlockButton();
    }

    //lock the buttons reagrdign the ones int he serlized field
    public void LockButtons()
    {
        isButtonLocked = true;

        if (mainMenuButtons != null)
        {
            foreach (Button button in mainMenuButtons)
            {
                if (button == null) continue;
                button.interactable = false;
            }
        }

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            GameObject selectedButton = eventSystem.currentSelectedGameObject;
            bool needsClear = selectedButton == null;
            if (!needsClear)
            {
                Button selectedBtn = selectedButton.GetComponent<Button>();
                if (selectedBtn == null || selectedBtn.interactable == false) needsClear = true;
            }

            if (needsClear)
            {
                eventSystem.SetSelectedGameObject(null);
                if (fallbackSelectedWhenLocked != null && fallbackSelectedWhenLocked.activeInHierarchy)
                    eventSystem.SetSelectedGameObject(fallbackSelectedWhenLocked);
            }
        }

        Debug.Log("locking the buttons.");
    }

    //unlock them
    public void UnlockButton()
    {
        if (mainMenuButtons != null)
        {
            foreach (var button in mainMenuButtons)
            {
                if (button == null) continue;
                button.interactable = true;
            }
        }

        isButtonLocked = false;

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);

            GameObject target = firstSelectedOnUnlock;
            if (target == null && mainMenuButtons != null)
            {
                foreach (Button button in mainMenuButtons)
                {
                    if (button != null && button.gameObject.activeInHierarchy)
                    {
                        target = button.gameObject;
                        break;
                    }
                }
            }

            if (target != null && target.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(target);
            }
        }

        Debug.Log("Unlocking the buttons.");
    }
}