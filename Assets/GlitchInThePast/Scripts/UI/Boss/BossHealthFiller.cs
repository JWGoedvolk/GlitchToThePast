using System.Collections.Generic;
using UnityEngine;

namespace UI.FadingEffect.Boss
{
    public class BossHealthFiller : MonoBehaviour
    {
        [Header("Prefabs")] 
        public GameObject StageHolderPrefab;
        public GameObject SegmentPrefab;

        [Header("Stage Details")] 
        public int healthCount = 1;
        public List<GameObject> StageSegments;
        public BossStageUI stageUI;

        [Header("Set Up")] 
        public RectTransform ParentObject;
        public BossHealthUI HealthUI;

        public void CreateNewStage()
        {
            GameObject stageHolder = Instantiate(StageHolderPrefab, ParentObject.transform); // Create a new stage holder
            
            // Get the holder's dimensions
            RectTransform stageHolderRect = stageHolder.GetComponent<RectTransform>();
            float width = stageHolderRect.rect.width;
            float height = stageHolderRect.rect.height;
            Debug.Log($"Created a new stage of dimension {width}, {height}");
            
            // Spawn in the health segments
            StageSegments = new List<GameObject>(); // Clear any previously created segments
            float segmentWidth = stageHolderRect.rect.width / healthCount; // Calculate how many segments are needed for the given amount of health amount
            
            for (int i = 0; i < healthCount; i++) // Create a new segment for each health point
            {
                GameObject segment = Instantiate(SegmentPrefab, stageHolderRect.transform); // The new segment is born
                
                RectTransform segmentRect = segment.GetComponent<RectTransform>();
                
                // Set the size of the segment
                segmentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, segmentWidth);
                float xPos = segmentWidth * i;
                segmentRect.anchoredPosition = new Vector2(xPos, 0);
                
                // Add it to the list of segments
                StageSegments.Add(segment);
            }
            
            stageUI = new BossStageUI(stageHolder, StageSegments); // Create the new Stage UI with the newly spawned in GameObjects
            HealthUI.AddStageUI(stageUI); // Add it to the list in the Health UI manager for use in the actual game
        }
    }
}