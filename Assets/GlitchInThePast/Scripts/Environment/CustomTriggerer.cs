using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JW.BeatEmUp.Objects
{
    public class CustomTriggerer : MonoBehaviour
    {
        public List<string> Whitelist;
        public List<GameObject> TriggeringObjects;
        public bool IsTriggering => TriggeringObjects != null && TriggeringObjects.Count > 0;
        [SerializeField] protected UnityEvent onTrigger;
        [SerializeField] protected UnityEvent onUnTrigger;

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
                    onUnTrigger?.Invoke();
                }
            }
        }

        public virtual void OnTrigger(GameObject other)
        {
            onTrigger?.Invoke();
        }

        public virtual void OnTriggerExit(GameObject other)
        {
            
        }
    }
}