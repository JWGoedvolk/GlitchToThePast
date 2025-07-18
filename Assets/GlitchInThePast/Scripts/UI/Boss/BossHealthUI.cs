using System.Collections.Generic;
using UnityEngine;

namespace UI.FadingEffect.Boss
{
    public class BossHealthUI : MonoBehaviour
    {
        [SerializeField] private int bossStage = 1;
        
        [Header("Stage 1")] 
        [SerializeField] private GameObject stage1Background;
        [SerializeField] private List<GameObject> stage1Hearts;
        
        [Header("Stage 2")] 
        [SerializeField] private GameObject stage2Background;
        [SerializeField] private List<GameObject> stage2Hearts;

        /// <summary>
        /// Deactivates one heart from the current stage from right to left
        /// </summary>
        public void TakeDamage()
        {
            if (bossStage == 1)
            {
                for (int i = stage1Hearts.Count - 1; i >= 0; i--) // Loop through all the hearts in reverse
                {
                    if (stage1Hearts[i].activeSelf) // if the heart is active
                    {
                        stage1Hearts[i].SetActive(false); // deactivate it and leave
                        break;
                    }
                }
            }
            else if (bossStage == 2)
            {
                for (int i = stage2Hearts.Count - 1; i >= 0; i--) // Loop through all the hearts in reverse
                {
                    if (stage2Hearts[i].activeSelf) // if the heart is active
                    {
                        stage2Hearts[i].SetActive(false); // deactivate it and leave
                        break;
                    }
                }
            }
        }

        public void NextStage()
        {
            bossStage++;
            stage1Background.SetActive(false);
            stage2Background.SetActive(true);
        }
    }
}