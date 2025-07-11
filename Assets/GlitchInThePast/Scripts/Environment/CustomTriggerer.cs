using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Roguelike.Objects
{
    public class CustomTriggerer : MonoBehaviour
    {
        public List<string> Whitelist;
        public List<GameObject> TriggeringObjects;
        public bool IsTriggering = false;

        public void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                if (!TriggeringObjects.Contains(other.gameObject))
                {
                    TriggeringObjects.Add(other.gameObject);
                    OnTrigger(other.gameObject);
                    IsTriggering = true;
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
                    
                    IsTriggering = TriggeringObjects.Count > 0;
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