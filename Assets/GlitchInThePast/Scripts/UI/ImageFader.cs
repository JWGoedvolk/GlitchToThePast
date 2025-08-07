using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScreenFaders
{
    [RequireComponent(typeof(Image))]
    public class ImageFader : MonoBehaviour
    {
        #region Variables
        [Tooltip("How long does the image take to fade.")]
        [SerializeField] private float imageFadeDuration = 2.5f;

        [Tooltip("How long to stay fully opaque before starting fade.")]
        [SerializeField] private float holdDuration = 1.0f;

        [SerializeField] private UIBlocker uiBlockerScript;
        [SerializeField] private Image imageToFade;
        [SerializeField] private TMP_Text textToFade;
        #endregion

        private void Awake()
        {
            if (imageToFade is null) imageToFade = GetComponent<Image>();

            if (uiBlockerScript is not null) uiBlockerScript.LockButtons();

            StartCoroutine(FadeAndDisable());
        }

        private IEnumerator FadeAndDisable()
        {
            Color startColour = imageToFade.color;
            Color startTextColour = textToFade != null ? textToFade.color : new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(holdDuration);

            float timeElapsed = 0f;
            while (timeElapsed < imageFadeDuration)
            {
                timeElapsed += Time.deltaTime;
                float alphaValue = Mathf.Lerp(startColour.a, 0f, timeElapsed / imageFadeDuration);

                imageToFade.color = new Color(startColour.r, startColour.g, startColour.b, alphaValue);

                if (textToFade is not null)
                {
                    textToFade.color = new Color(startTextColour.r, startTextColour.g, startTextColour.b, alphaValue);
                }

                yield return null;
            }

            imageToFade.color = new Color(startColour.r, startColour.g, startColour.b, 0f);
            if (textToFade is not null)
            {
                textToFade.color = new Color(startTextColour.r, startTextColour.g, startTextColour.b, 0f);
            }
            if (uiBlockerScript is not null) uiBlockerScript.UnlockButton();

            gameObject.SetActive(false);
        }
    }
}