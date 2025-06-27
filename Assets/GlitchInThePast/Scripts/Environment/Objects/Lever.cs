using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JW.Roguelike.Objects.Objects
{
    public class Lever : MonoBehaviour
    { 
        public UnityEvent onActivated;
        [SerializeField] private List<string> whitelistedTags;
        [SerializeField] private GameObject interactPreview;
        [SerializeField] private List<GameObject> whitelistedObjects;
        public bool IsInteractebale;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (whitelistedTags.Contains(other.tag))
            {
                if (!whitelistedObjects.Contains(other.gameObject))
                {
                    whitelistedObjects.Add(other.gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (whitelistedTags.Contains(other.tag))
            {
                if (whitelistedObjects.Contains(other.gameObject))
                {
                    whitelistedObjects.Remove(other.gameObject);
                }
            }
        }

        private void Update()
        {
            interactPreview.SetActive(whitelistedObjects.Count > 0 || IsInteractebale);
        }
    }
}