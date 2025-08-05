using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Narrative
{
    public class NarrativeSequenceTrigger : MonoBehaviour
    {
        #region Variables
        public NarrativeSequence sequenceToPlay;
        [SerializeField] private bool playSequenceAtStart = false;
        [SerializeField] private bool isReplayable;
        [SerializeField] private TMP_Text playerCountText; 

        public UnityEvent OnSequenceEnd;
        private bool triggeredByPlayer;
        private HashSet<Collider> playersInTrigger = new HashSet<Collider>();
        #endregion

        private void OnEnable()
        {
            if (sequenceToPlay != null)
                sequenceToPlay.OnSequenceEnd.RemoveListener(HandleSequenceEnd);
        }

        private void Start()
        {
            if (playSequenceAtStart && NarrativeManager.Instance != null)
            {
                PlaySequence();
            }
            UpdatePlayerCountText();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                if (playersInTrigger.Add(other))
                {
                    UpdatePlayerCountText();
                }

                if (playersInTrigger.Count == 2 && (!triggeredByPlayer || isReplayable))
                {
                    PlaySequence();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                if (playersInTrigger.Remove(other))
                {
                    UpdatePlayerCountText();
                }
            }
        }

        #region Private Functions
        private void UpdatePlayerCountText()
        {
            if (playerCountText != null)
            {
                playerCountText.text = $"{playersInTrigger.Count}/2";
            }
        }

        private void PlaySequence()
        {
            if (sequenceToPlay == null || NarrativeManager.Instance == null)
                return;

            SubscribeToSequenceEnd();
            NarrativeManager.Instance.PlaySequence(sequenceToPlay);

            if (!isReplayable)
            {
                triggeredByPlayer = true;
            }
        }

        private void SubscribeToSequenceEnd()
        {
            sequenceToPlay.OnSequenceEnd.AddListener(HandleSequenceEnd);
        }

        private void HandleSequenceEnd()
        {
            OnSequenceEnd?.Invoke();
            sequenceToPlay.OnSequenceEnd.RemoveListener(HandleSequenceEnd);
        }

        private void OnDisable()
        {
            // Safety: Always unsubscribe when this object is disabled
            if (sequenceToPlay != null)
                sequenceToPlay.OnSequenceEnd.RemoveListener(HandleSequenceEnd);
        }
        #endregion
    }
}
