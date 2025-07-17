using UnityEngine;
using UnityEngine.Events;

namespace Narrative
{
    public class NarrativeSequenceTrigger : MonoBehaviour
    {
        public NarrativeSequence sequenceToPlay;
        [SerializeField] private bool playSequenceAtStart = false;

        public UnityEvent OnSequenceEnd;  

        private void OnEnable()
        {
            // Safety: Unsubscribe first (prevents double-subscribing in rare cases)
            if (sequenceToPlay != null)
                sequenceToPlay.OnSequenceEnd.RemoveListener(HandleSequenceEnd);
        }

        private void OnDisable()
        {
            if (sequenceToPlay != null)
                sequenceToPlay.OnSequenceEnd.RemoveListener(HandleSequenceEnd);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                if (NarrativeManager.Instance is null) return;
                SubscribeToSequenceEnd();
                NarrativeManager.Instance.PlaySequence(sequenceToPlay);
                Destroy(this);
            }
        }

        private void Start()
        {
            if (playSequenceAtStart && NarrativeManager.Instance != null)
            {
                SubscribeToSequenceEnd();
                NarrativeManager.Instance.PlaySequence(sequenceToPlay);
            }
        }

        private void SubscribeToSequenceEnd()
        {
            if (sequenceToPlay != null)
                sequenceToPlay.OnSequenceEnd.AddListener(HandleSequenceEnd);
        }

        private void HandleSequenceEnd()
        {
            OnSequenceEnd?.Invoke();

            if (sequenceToPlay != null)
            {
                sequenceToPlay.OnSequenceEnd.RemoveListener(HandleSequenceEnd);
            }
        }
    }
}