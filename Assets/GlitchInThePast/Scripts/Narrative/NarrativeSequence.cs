using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Narrative
{
    [CreateAssetMenu(fileName = "NewNarrativeSequence", menuName = "Narrative/Narrative Sequence")]
    public class NarrativeSequence : ScriptableObject
    {
        [Tooltip("How big is the narrative list? After how many steps will the narrative end?")]
        public int stepCount = 1;

        [Header("Narrative")]
        [Tooltip("")]
        public List<string> narrativeTexts = new List<string>();

        [Tooltip("")]
        public List<Sprite> speakerIcons = new List<Sprite>();
        [Tooltip("")]
        public List<Sprite> optionalImages = new List<Sprite>();
        [Tooltip("")]
        public List<AudioClip> voiceOvers = new List<AudioClip>();

        [Header("Tooltips")]
        [Tooltip("")]
        public List<bool> isSkippable = new List<bool>();
        [Tooltip("")]
        public List<bool> isTooltip = new List<bool>();
        [Tooltip("")]
        public List<float> tooltipDurations = new List<float>();

        public UnityEvent OnSequenceEnd;
    }
}