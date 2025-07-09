using System.Collections.Generic;
using UnityEngine;

public class GamePauser : MonoBehaviour
{
    #region Variables
    public static GamePauser Instance;
    private readonly List<IPauseable> pauseables = new();
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    #region Public Functions
    public void RegisterPauseable(IPauseable pauseable)
    {
        if (!pauseables.Contains(pauseable))
            pauseables.Add(pauseable);
    }

    public void UnregisterPauseable(IPauseable pauseable)
    {
        if (pauseables.Contains(pauseable))
            pauseables.Remove(pauseable);
    }

    public void PauseGame()
    {
        foreach (IPauseable pauseable in pauseables)
            pauseable.OnPause();
    }

    public void UnpauseGame()
    {
        foreach (IPauseable pauseable in pauseables)
            pauseable.OnUnpause();
    }
    #endregion
}