using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    #region Variables
    public static PauseMenu Instance;

    public GameObject pauseMenu;

    [SerializeField] private GameObject firstSelectedButton;
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

        if (pauseMenu == null)
        {
            pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        }
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
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
        if (pauseMenu != null) pauseMenu.SetActive(paused);

        if (paused) ForceSelect(firstSelectedButton);
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

        ForceSelect(firstSelectedButton);
    }

    public void Resume()
    {
        if (!paused) return;
        paused = false;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (Mathf.Approximately(Time.timeScale, 0f)) Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = false;

        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    private void ForceSelect(GameObject gameObject)
    {
        if (gameObject == null || EventSystem.current == null) return;
        StartCoroutine(SelectNextFrame(gameObject));
    }

    private System.Collections.IEnumerator SelectNextFrame(GameObject gameObject)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}