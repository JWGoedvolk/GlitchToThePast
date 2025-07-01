using TMPro;
using UnityEngine;

namespace GlitchInThePast.Scripts.RoomGeneration
{
    public class Room : MonoBehaviour
    {
        [Header("DEBUGGING")]
        public TMP_Text roomNameText;
        [Header("NORMAL")]
        public string Name;
        public bool IsLocked;
        public virtual void ResetRoom() {}

        public void LockRoom()
        {
            
        }
    }
}