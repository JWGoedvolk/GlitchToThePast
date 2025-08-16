using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Audio
{
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

        #region Scene Change Detection
        private void OnSceneChanged(Scene from, Scene to)
        {
            if (pauseMenu != null) pauseMenu.SetActive(paused);

            if (paused) ForceSelect(firstSelectedButton);
        }
        #endregion

        #region Public Functions
        public void Toggle()
        {
            if (pauseMenu.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (pauseMenu != null)
                pauseMenu.SetActive(true);

            Time.timeScale = 0f;
            Cursor.visible = true;

            ForceSelect(firstSelectedButton);
        }

        public void Resume()
        {
            paused = false;

            if (pauseMenu != null)
                pauseMenu.SetActive(false);

            if (Mathf.Approximately(Time.timeScale, 0f))
                Time.timeScale = 1f;

            Cursor.visible = false;

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        #region Private Functions
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
        #endregion
    }
}