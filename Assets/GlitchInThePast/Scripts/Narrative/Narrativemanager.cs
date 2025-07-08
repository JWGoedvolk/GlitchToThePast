using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Narrative
{
    public class NarrativeManager : MonoBehaviour
    {
        #region Variables
        public static NarrativeManager Instance;

        [Header("Narrative UI")]
        [Tooltip("Drag the Panel that parents all the narrative related UI components to this slot.")]
        public GameObject narrativePanel;
        [Tooltip("Drag the TMP Text UI that will display the narrative's text to this slot.")]
        public TMP_Text narrativeText;
        [Tooltip("Drag the Image that will display who is speaking to this slot.")]
        public Image speakerIcon;
        [Tooltip("Drag the Image component that covers the screen and is placed under the narrative speaker icon and narrative text to this slot.")]
        public Image optionalImage;

        [Header("Tooltip UI")]
        [Tooltip("Drag the panel that will pop up just as a tooltip to this slot.")]
        public GameObject tooltipPanel;
        [Tooltip("Drag the text that will display tooltip information to this slot.")]
        public TMP_Text tooltipText;

        [Tooltip("Set how many letters in the narrative should appear per second.")]
        public float lettersPerSecond = 20f;
        private float autoContinueDuration = 3f; // TODO: Auto continue only when the entire text is displayed.
        private Coroutine typewriterCoroutine;

        private NarrativeSequence currentSequence;
        private int currentIndex = 0;
        private bool isWaitingForInput = false;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!isWaitingForInput) return;

            if (Keyboard.current?.spaceKey.wasPressedThisFrame == true || Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
            {
                if (typewriterCoroutine != null)
                {
                    #region Immediately complete the sentence.
                    StopCoroutine(typewriterCoroutine);
                    narrativeText.text = SafelyGet(currentSequence.narrativeTexts, currentIndex - 1);
                    typewriterCoroutine = null;
                    return;
                    #endregion
                }

                ContinueSequence();
            }
        }

        #region Public Functions
        public void PlaySequence(NarrativeSequence sequence)
        {
            currentSequence = sequence;
            currentIndex = 0;
            isWaitingForInput = false;
            ContinueSequence();
        }
        #endregion

        #region Private Functions
        private void ContinueSequence()
        {
            if (currentSequence == null || currentIndex >= currentSequence.stepCount)
            {
                Debug.Log("[Narrative] Reached end of sequence.");
                EndSequence();
                return;
            }

            #region Display whatever the sequence contains
            string text = SafelyGet(currentSequence.narrativeTexts, currentIndex);
            Sprite speaker = SafelyGet(currentSequence.speakerIcons, currentIndex);
            Sprite image = SafelyGet(currentSequence.optionalImages, currentIndex);
            AudioClip voice = SafelyGet(currentSequence.voiceOvers, currentIndex);
            bool skippable = SafelyGet(currentSequence.isSkippable, currentIndex);
            bool tooltip = SafelyGet(currentSequence.isTooltip, currentIndex);
            float tooltipTime = SafelyGet(currentSequence.tooltipDurations, currentIndex);
            #endregion

            if (tooltip)
            {
                StartCoroutine(ShowTooltip(text, tooltipTime));
                return;
            }

            narrativePanel.SetActive(true);

            if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);

            typewriterCoroutine = StartCoroutine(TypeText(text));

            speakerIcon.sprite = speaker;
            speakerIcon.gameObject.SetActive(speaker != null);

            optionalImage.sprite = image;
            optionalImage.gameObject.SetActive(image != null);

            if (voice != null) AudioSource.PlayClipAtPoint(voice, Camera.main.transform.position);

            if (skippable)
            {
                StartCoroutine(DelayInputEnable());
            }
            else
            {
                StartCoroutine(AutoContinue(autoContinueDuration));
            }

            currentIndex++;
        }

        #region Coroutines
        private IEnumerator TypeText(string text)
        {
            narrativeText.text = "";
            float interval = 1f / lettersPerSecond;

            for (int i = 0; i < (text?.Length ?? 0); i++)
            {
                narrativeText.text += text[i];
                yield return new WaitForSeconds(interval);
            }
            narrativeText.text = text;
            typewriterCoroutine = null;
        }

        private IEnumerator AutoContinue(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ContinueSequence();
        }

        private IEnumerator ShowTooltip(string message, float duration)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = message;

            yield return new WaitForSeconds(duration);

            tooltipPanel.SetActive(false);

            currentIndex++;
            ContinueSequence();
        }

        private IEnumerator DelayInputEnable()
        {
            yield return null;
            isWaitingForInput = true;
        }
        #endregion

        private void EndSequence()
        {
            Debug.Log("[Narrative] Sequence ended.");
            narrativePanel.SetActive(false);
            currentSequence = null;
            currentIndex = 0;
            isWaitingForInput = false;
        }

        #region Safe getter to avoid 'out of range'' issues
        private T SafelyGet<T>(List<T> list, int index)
        {
            if (list == null || index >= list.Count)
            {
                return default;
            }
            return list[index];
        }
        #endregion
        #endregion
    }
}