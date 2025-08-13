using System;
using UnityEngine;

namespace GlitchInThePast.Scripts.Utility
{
    public class LevelShower : MonoBehaviour
    {
        public Color DebugColor = Color.yellow;
        public bool IsOnSelectedOnly = false;
        public float Distance = 50f;
        private void OnDrawGizmos()
        {
            if (IsOnSelectedOnly)
            {
                return;
            }
            
            Gizmos.color = DebugColor;
            Debug.DrawRay(transform.position,  transform.right * Distance, DebugColor);
            Debug.DrawRay(transform.position, -transform.right * Distance, DebugColor);
        }
    }
}