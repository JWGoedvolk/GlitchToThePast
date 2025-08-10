using TMPro;
using UnityEngine;

namespace GlitchInThePast.Scripts.Utility
{
    public class DamgeDebugger : MonoBehaviour
    {
        [Header("UI")]
        public TMP_Text UIText;

        public void DipslayDamageDealt(int damage)
        {
            Debug.Log($"[DEBUG][DAMAGEDEBUGGER] {gameObject.name} recieved {damage} damage.");

            if (UIText != null)
            {
                UIText.text = damage.ToString();
            }
        }
    }
}