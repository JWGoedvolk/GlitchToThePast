using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonsFontStyleChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text tmpText;

    private void Awake()
    {
        if (tmpText != null)
        {
            tmpText.fontStyle = FontStyles.Italic;
        }
    }

    #region Public Functions
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetBold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetItalic();
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetBold();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetItalic();
    }
    #endregion

    #region Private Functions
    private void SetBold()
    {
        if (tmpText != null)
        {
            tmpText.fontStyle = FontStyles.Bold;
        }
    }

    private void SetItalic()
    {
        if (tmpText != null)
            tmpText.fontStyle = FontStyles.Italic;
    }
    #endregion
}