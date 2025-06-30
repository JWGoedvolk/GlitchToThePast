using System.Collections.Generic;
using UnityEngine;

namespace JW.Roguelike.Objects
{
    public class CustomCollider : MonoBehaviour
    {
        public List<string> Whitelist;
        public List<GameObject> TriggeringObjects;

        public void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                if (!TriggeringObjects.Contains(other.gameObject))
                {
                    TriggeringObjects.Add(other.gameObject);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                if (TriggeringObjects.Contains(other.gameObject))
                {
                    TriggeringObjects.Remove(other.gameObject);
                }
            }
        }

        public virtual void OnTrigger(GameObject other)
        {
            
        }

        public virtual void OnTriggerExit(GameObject other)
        {
            
        }
    }
}