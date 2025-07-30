using UnityEngine;

namespace Player.Health
{
    public class HealthUIManager : MonoBehaviour
    {
        [SerializeField] private HealthDisplayUI[] healthUIs;

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
                    ui.UpdateHealth(healthSystem.currentHealth, healthSystem.maxHealth);
                    break;
                }
            }
        }
    }
}