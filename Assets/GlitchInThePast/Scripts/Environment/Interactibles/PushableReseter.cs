using System.Collections.Generic;
using UnityEngine;

namespace JW.Objects.Interactibles
{
    public class PushableReseter : MonoBehaviour
    {
        public List<Transform> Pushables = new List<Transform>();
        public List<Transform> DefaultPositions = new List<Transform>();

        void Start()
        {
            var pushables = FindObjectsOfType<PushableInteractible>();
            foreach (var pushable in pushables)
            {
                Pushables.Add(pushable.transform);
                DefaultPositions.Add(pushable.transform);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ResetPushables();
            }
        }

        public void ResetPushables()
        {
            for (int i = 0; i < Pushables.Count; i++)
            {
                Pushables[i].position = DefaultPositions[i].position;
                Pushables[i].rotation = DefaultPositions[i].rotation;
            }
        }
    }
}