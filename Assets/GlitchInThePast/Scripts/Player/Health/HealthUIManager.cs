using Player.Health;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Health
{
    public class HealthUIManager : MonoBehaviour
    {
        [SerializeField] private HealthDisplayUI[] healthUIs;

        public Image P1Image;
        public Image P2Image;
        public TMP_Text P1Text;
        public TMP_Text P2Text;
        public Sprite BobSprite;
        public Sprite TobSprite;
        public Color BobColor;
        public Color TobColor;

        private void OnEnable()
        {
            PlayerHealthSystem.OnPlayerSpawned += HandlePlayerSpawned;
        }

        private void OnDisable()
        {
            PlayerHealthSystem.OnPlayerSpawned -= HandlePlayerSpawned;
        }

        private void HandlePlayerSpawned(int index, PlayerHealthSystem healthSystem)
        {
            foreach (var ui in healthUIs)
            {
                if ((int)ui.playerID == index)
                {
                    healthSystem.AssignUI(ui);
                    // Debug.Log($"{ui} has been assigned to {healthSystem.gameObject.name}");
                    if (index == 0)
                    {
                        if (healthSystem.gameObject.name.Contains("BobsCollider"))
                        {
                            P1Image.sprite = BobSprite;
                            P1Text.color = BobColor;
                        }
                        else if (healthSystem.gameObject.name.Contains("TobbysCollider"))
                        {
                            P1Image.sprite = TobSprite;
                            P1Text.color = TobColor;
                        }
                    }
                    else if (index == 1)
                    {
                        if (healthSystem.gameObject.name.Contains("BobsCollider"))
                        {
                            P2Image.sprite = BobSprite;
                            P2Text.color = BobColor;
                        }
                        else if (healthSystem.gameObject.name.Contains("TobbysCollider"))
                        {
                            P2Image.sprite = TobSprite;
                            P2Text.color = TobColor;
                        }
                    }
                    break;
                }
            }
        }
    }
}