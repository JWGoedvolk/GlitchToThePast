using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace UI.FadingEffect
{
    public class ScreenFader : MonoBehaviour
    {
        #region Variables
        public static ScreenFader Instance;
        public RectTransform blackCircle;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update() // This is just for demonstration purposes. Naturally you'd call the FadeTransition scene when needed.
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Instance.FadeTransition("FadeEffectPartTwo", 1.5f, 1f); // You can test it with a scene

                Instance.FadeTransition(null, 1.5f, 1f); // You can use it without providing a scene name too ^^
            }
        }

        #region Public Functions
        /// <summary>
        /// Plays the iris transition. If a scene name is provided, it will load the scene during the transition.
        /// If the scene name is null it simply fades in, holds, and fades out!
        /// </summary>
        /// <param name="sceneName"> Either put the name of the scene you wish to load or leave it null or empty to just play the fade effect</param>
        /// <param name="transitionTime"> How fast the iris fades in and out.</param>
        /// <param name="holdDuration"> How long the screen stays black before fading out.</param>
        public void FadeTransition(string sceneName, float transitionTime = 1f, float holdDuration = 0.5f)
        {
            StartCoroutine(DoIrisTransition(sceneName, transitionTime, holdDuration));
        }

        public void HideFade(Action onComplete = null, float transitionTime = 1f, float holdDuration = 0f)
        {
            StartCoroutine(FadeIris(Vector3.zero, Vector3.one * 10, transitionTime, holdDuration, onComplete));
        }

        public void ShowFade(float transitionTime = 1f)
        {
            StartCoroutine(FadeIris(Vector3.one * 10, Vector3.zero, transitionTime, 0));
        }
        #endregion

        #region Private Functions

        private IEnumerator DoIrisTransition(string sceneName, float transitionTime, float holdDuration)
        {
            yield return StartCoroutine(FadeIris(Vector3.zero, Vector3.one * 10, transitionTime, holdDuration));

            #region Load a scene is a scene name was provided, otherwise just apply effect.
            if (!string.IsNullOrEmpty(sceneName))
            {
                yield return SceneManager.LoadSceneAsync(sceneName);
                yield return new WaitForSeconds(0.1f);
            }
            #endregion

            yield return StartCoroutine(FadeIris(Vector3.one * 10, Vector3.zero, transitionTime, 0));
        }

        private IEnumerator FadeIris(Vector3 from, Vector3 to, float duration, float holdTime = 0f, Action onComplete = null)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                blackCircle.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }

            blackCircle.localScale = to;

            if (holdTime > 0) yield return new WaitForSeconds(holdTime);

            onComplete?.Invoke();
        }

        #endregion
    }
}