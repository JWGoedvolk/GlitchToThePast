using System;
using UnityEngine;

namespace GlitchInThePast.Scripts.Utility
{
    public class PointShower : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private float size = 0.05f;
        [SerializeField] private bool onSelectedOnly = false;

        void OnDrawGizmos()
        {
            if (onSelectedOnly) return;
            
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, size);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!onSelectedOnly) return;
            
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, size);
        }
    }
}