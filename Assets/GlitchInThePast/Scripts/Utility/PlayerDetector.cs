using System;
using System.Collections.Generic;
using Systems.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchInThePast.Scripts.Utility
{
    public class PlayerDetector : MonoBehaviour
    {
        public Action OnPlayerChanged;
        
        private List<GameObject> players = new List<GameObject>();

        [Header("Detection Settings")] 
        [SerializeField] private Vector3 detectionBox;
        private Vector3 defaultSize = Vector3.one;
        
        [Header("Detection States")]
        public GameObject ClosestPlayerInRange;
        public List<GameObject> PlayersInRange = new List<GameObject>();
        public bool IsPlayerInRange => PlayersInRange.Count > 0;

        private void Awake()
        {
            defaultSize = detectionBox;
            
            foreach (var player in PlayerInput.all)
            {
                players.Add(player.gameObject);
            }
        }

        private void Update()
        {
            PlayersInRange.Clear();
            var hits = Physics.OverlapBox(transform.position, detectionBox/2f);
            //Debug.Log(hits.Length);

            foreach (Collider hit in hits)
            {
                // Filter out all non player hits
                if (!hit.gameObject.tag.Contains("Player"))
                {
                    continue;
                }
                
                PlayersInRange.Add(hit.gameObject);
            }
            
            GameObject closest = ClosestPlayerInRange;
            
            // Get the closest player in range
            if (PlayersInRange.Count == 0)
            {
                ClosestPlayerInRange = null;
                return;
            }
            else if (PlayersInRange.Count == 1)
            {
                ClosestPlayerInRange = PlayersInRange[0];
                return;
            }
            
            foreach (GameObject player in PlayersInRange)
            {
                // Skip the current closest
                if (player == ClosestPlayerInRange)
                {
                    continue;
                }
                
                if (Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(player.transform.position, ClosestPlayerInRange.transform.position))
                {
                    ClosestPlayerInRange = player;
                }
            }

            if (closest != ClosestPlayerInRange)
            {
                OnPlayerChanged?.Invoke();
            }
        }

        public void ResetDetectionSize()
        {
            detectionBox = defaultSize;
        }

        public void SetDetectionSize(Vector3 newSize)
        {
            detectionBox = newSize;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, detectionBox);
        }
    }
}