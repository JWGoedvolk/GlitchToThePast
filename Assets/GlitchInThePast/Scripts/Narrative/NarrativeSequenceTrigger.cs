using UnityEngine;

namespace Narrative
{
    public class NarrativeSequenceTrigger : MonoBehaviour
    {
        public NarrativeSequence sequenceToPlay;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                NarrativeManager.Instance.PlaySequence(sequenceToPlay);
                Destroy(this);
            }
        }
    }
}