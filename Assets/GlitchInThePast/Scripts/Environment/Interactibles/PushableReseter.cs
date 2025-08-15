using System.Collections.Generic;
using UnityEngine;

namespace JW.Objects.Interactibles
{
    public class PushableReseter : MonoBehaviour
    {
        public List<Transform> Pushables = new List<Transform>();
        public List<Vector3> DefaultPositions = new List<Vector3>();
        public List<Quaternion> DefaultRotations = new();

        void Start()
        {
            foreach (var pushable in Pushables)
            {
                DefaultPositions.Add(pushable.transform.position);
                DefaultRotations.Add(pushable.transform.localRotation);
            }
        }

        public void ResetPushables()
        {
            for (int i = 0; i < Pushables.Count; i++)
            {
                Pushables[i].position = DefaultPositions[i];
                Pushables[i].rotation = DefaultRotations[i];
            }
        }
    }
}