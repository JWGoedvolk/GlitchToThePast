using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JW.Roguelike.Objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BreakableObject : MonoBehaviour
    {
        [Header("Filters")] 
        [SerializeField] private List<string> whiteListTags;
        [SerializeField] private bool isTrigger = true;
        
        [Header("Events")] 
        [SerializeField] private UnityEvent onBreak;
        
        // Start is called before the first frame update
        void OnEnable()
        {
            gameObject.GetComponent<BoxCollider2D>().isTrigger = isTrigger;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (whiteListTags.Contains(collision.tag))
            {
                onBreak?.Invoke();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (whiteListTags.Contains(other.collider.tag))
            {
                onBreak?.Invoke();
            }
        }
    }
}
