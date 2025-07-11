using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Roguelike.Objects
{
    public class CustomTriggerer : MonoBehaviour
    {
        public List<string> Whitelist;
        public List<GameObject> TriggeringObjects;
        public bool IsTriggering => TriggeringObjects != null && TriggeringObjects.Count > 0;

        public void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                if (!TriggeringObjects.Contains(other.gameObject))
                {
                    TriggeringObjects.Add(other.gameObject);
                    OnTrigger(other.gameObject);
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
                    OnTriggerExit(other.gameObject);
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