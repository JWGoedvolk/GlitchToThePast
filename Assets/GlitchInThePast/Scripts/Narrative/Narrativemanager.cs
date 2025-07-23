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
        [SerializeField] private GameObject narrativePanel;
        [Tooltip("Drag the TMP Text UI that will display the narrative's text to this slot.")]
        [SerializeField] private TMP_Text narrativeText;
        [Tooltip("Drag the Image that will display who is speaking to this slot.")]
        [SerializeField] private Image speakerIcon;
        [Tooltip("Drag the text component that will display the speaker's name.")]
        [SerializeField] private TMP_Text speakerNameText;
        [Tooltip("Drag the Image component that covers the screen and is placed under the narrative speaker icon and narrative text to this slot.")]
        [SerializeField] private Image optionalImage;
        [Tooltip("Drag the text container into this slot")]
        [SerializeField] private Image textBackground;

        [Header("Tooltip UI")]
        [Tooltip("Drag the panel that will pop up just as a tooltip to this slot.")]
        public GameObject tooltipPanel;

        [Tooltip("Drag the header text that will appear above the tooltip message.")]
        public TMP_Text tooltipHeaderText;
        [Tooltip("Drag the text that will display tooltip information to this slot.")]
        public TMP_Text tooltipText;
        [Tooltip("Drag the image that will appear above the tooltip message.")]
        public Image tooltipImage;

        [Tooltip("Set how many letters in the narrative should appear per second.")]
        public float lettersPerSecond = 20f;
        private float autoContinueDuration = 3f;
        private Coroutine typewriterCoroutine;

        private NarrativeSequence currentSequence;
        private int currentIndex = 0;
        private bool isWaitingForInput = false;
        private bool autoContinueAfterTyping = false;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!isWaitingForInput) return;

            if (Keyboard.current?.enterKey.wasPressedThisFrame == true || Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
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
            GamePauser.Instance?.PauseGame();
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
            string speakerName = SafelyGet(currentSequence.speakerNames, currentIndex);
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

            textBackground.gameObject.SetActive(text != null);

            speakerIcon.sprite = speaker;
            speakerIcon.gameObject.SetActive(speaker != null);

            speakerNameText.text = speakerName;
            speakerNameText.gameObject.SetActive(speaker != null);

            optionalImage.sprite = image;
            optionalImage.gameObject.SetActive(image != null);

            if (voice != null) AudioSource.PlayClipAtPoint(voice, Camera.main.transform.position);

            if (skippable)
            {
                StartCoroutine(DelayInputEnable());
            }
            else
            {
                autoContinueAfterTyping = true;
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

            if (autoContinueAfterTyping)
            {
                autoContinueAfterTyping = false;
                StartCoroutine(AutoContinue(autoContinueDuration));
            }
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

            string header = SafelyGet(currentSequence.tooltipHeaders, currentIndex);
            tooltipHeaderText.text = string.IsNullOrEmpty(header) ? "" : header;
            tooltipHeaderText.gameObject.SetActive(!string.IsNullOrEmpty(header));

            Sprite tooltipSprite = SafelyGet(currentSequence.tooltipImages, currentIndex);
            tooltipImage.sprite = tooltipSprite;
            tooltipImage.gameObject.SetActive(tooltipSprite != null);

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
            narrativePanel.SetActive(false);
            if (currentSequence != null && currentSequence.OnSequenceEnd != null)
            {
                currentSequence.OnSequenceEnd.Invoke();
            }
            currentSequence = null;
            currentIndex = 0;
            isWaitingForInput = false;

            GamePauser.Instance?.UnpauseGame();
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