using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Narrative
{
    [CreateAssetMenu(fileName = "NewNarrativeSequence", menuName = "Narrative/Narrative Sequence")]
    public class NarrativeSequence : ScriptableObject
    {
        [Tooltip("How big is the narrative list? After how many steps will the narrative end?")]
        public int stepCount = 1;
        [Tooltip("")]
        public List<bool> isSkippable = new List<bool>();

        [Header("Narrative")]
        [Tooltip("What will each narrative frame say? You can leave it empty if you do not want any narrative")]
        public List<string> narrativeTexts = new List<string>();

        [Tooltip("")]
        public List<Sprite> speakerIcons = new List<Sprite>();
        [Tooltip("Name of the speaker shown under the icon.")]
        public List<string> speakerNames = new List<string>();
        [Tooltip("")]
        public List<Sprite> optionalImages = new List<Sprite>();
        [Tooltip("")]
        public List<AudioClip> voiceOvers = new List<AudioClip>();

        [Header("Tooltips")]
        public List<string> tooltipHeaders = new List<string>();
        public List<Sprite> tooltipImages = new List<Sprite>();
        public List<bool> isTooltip = new List<bool>();
        [Tooltip("")]
        public List<float> tooltipDurations = new List<float>();

        public UnityEvent OnSequenceEnd;
    }
}