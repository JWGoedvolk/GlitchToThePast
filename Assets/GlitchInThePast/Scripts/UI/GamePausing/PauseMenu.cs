using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region Variables
    public static PauseMenu Instance;
    [SerializeField] private GameObject pauseMenu;
    private bool paused;
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void Update()
    {
        bool escapePressed = Keyboard.current?.escapeKey.wasPressedThisFrame == true;
        bool controllerMenuPressed = Gamepad.current?.startButton.wasPressedThisFrame == true;

        if (escapePressed || controllerMenuPressed)
            Toggle();
    }

    private void OnSceneChanged(Scene from, Scene to)
    {
        if (pauseMenu == null)
        {
            var foundCanvas = GameObject.Find("PauseMenuCanvas") ?? FindObjectOfType<Canvas>(true)?.gameObject;
            if (foundCanvas != null) pauseMenu = foundCanvas;
        }

        if (pauseMenu != null) pauseMenu.SetActive(paused);
    }

    public void Toggle()
    {
        if (paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (paused) return;
        paused = true;

        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (!paused) return;
        paused = false;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (Mathf.Approximately(Time.timeScale, 0f)) Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = false;
    }
}