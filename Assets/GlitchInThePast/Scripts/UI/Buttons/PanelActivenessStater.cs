using UnityEngine;

[DisallowMultipleComponent]
public class PanelActivenessStater : MonoBehaviour
{
    [SerializeField] private UIBlocker blocker;

    private void Awake()
    {
        if (blocker == null) blocker = FindObjectOfType<UIBlocker>(true);
    }

    private void OnEnable()
    {
        if (blocker != null) blocker.ReportPanelOpened(gameObject);
    }

    private void OnDisable()
    {
        if (blocker != null) blocker.ReportPanelClosed(gameObject);
    }
}