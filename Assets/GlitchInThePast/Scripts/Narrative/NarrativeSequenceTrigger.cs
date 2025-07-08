using UnityEngine;

namespace Narrative
{
    public class NarrativeSequenceTrigger : MonoBehaviour
    {
        public NarrativeSequence sequenceToPlay;
        [SerializeField] private bool playSequenceAtStart = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                NarrativeManager.Instance.PlaySequence(sequenceToPlay);
                Destroy(this);
            }
        }

        private void Start()
        {
            if (playSequenceAtStart is true)
            {
                NarrativeManager.Instance.PlaySequence(sequenceToPlay);
            }
        }
    }
}