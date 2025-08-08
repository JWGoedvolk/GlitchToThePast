using System.Collections.Generic;
using UnityEngine;

namespace UI.FadingEffect.Boss
{
    [System.Serializable]
    public class BossStageUI
    {
        public GameObject Panel;
        public List<GameObject> Hearts;

        public BossStageUI(GameObject Panel, List<GameObject> Hearts)
        {
            this.Panel = Panel;
            this.Hearts = Hearts;
        }
        
        public void TakeDamage()
        {
            for (int i = Hearts.Count - 1; i > 0; i--)
            {
                if (!Hearts[i].activeSelf)
                {
                    continue;
                }
                
                Hearts[i].SetActive(false);
                break;
            }
        }

        public void StartStage()
        {
            Panel.SetActive(true);
        }

        public void EndStage()
        {
            Panel.SetActive(false);
        }
    }
}