using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JW.Roguelike.Objects
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class CustomTriggerer : MonoBehaviour
    {
        // TODO: Make these variables private and expose them with public getters
        [Header("Collision Filtering")]
        public List<string> Whitelist;
        
        [Header("States")]
        public List<GameObject> TriggeringObjects;
        public bool IsTriggering = false;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onTrigger;
        [SerializeField] private UnityEvent onUnTrigger;

        public void OnTriggerEnter(Collider other)
        {
            if (Whitelist.Contains(other.tag))
            {
                if (!TriggeringObjects.Contains(other.gameObject))
                {
                    TriggeringObjects.Add(other.gameObject);
                    OnTrigger(other.gameObject);
                    IsTriggering = true;
                    onTrigger?.Invoke();
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
                    
                    if (TriggeringObjects.Count == 0)
                    {
                        IsTriggering = TriggeringObjects.Count > 0;
                        onUnTrigger?.Invoke();
                    }
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