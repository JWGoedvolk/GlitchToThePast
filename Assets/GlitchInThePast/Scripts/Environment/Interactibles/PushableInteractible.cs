using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.Objects.Interactibles
{
    public class PushableInteractible : MonoBehaviour
    {
        public float forceAmount = 1f;
        public List<string> WhiteListTags = new() {"Pushable"};
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (WhiteListTags.Contains(hit.gameObject.tag))
            {
                Rigidbody rigidbody = hit.gameObject.GetComponent<Rigidbody>();

                if (rigidbody != null)
                {
                    Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                    forceDirection.y = 0;
                    forceDirection.Normalize();
                    forceDirection *= forceAmount;
                    rigidbody.AddForceAtPosition(forceDirection, transform.position, ForceMode.Impulse);
                }
            }
        }
    }
}