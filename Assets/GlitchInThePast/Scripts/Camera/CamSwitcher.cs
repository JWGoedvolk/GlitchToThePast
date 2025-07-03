using UnityEngine;
using System.Collections.Generic;

namespace CameraScripts
{
    public class CamSwitcher : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Camera cameraToEnable;
        [SerializeField] private Camera cameraToDisable;

        private HashSet<GameObject> playersInside = new HashSet<GameObject>();
        private int totalPlayers = 2;
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                playersInside.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                playersInside.Remove(other.gameObject);
                CheckForSwitch();
            }
        }

        private void CheckForSwitch()
        {
            if (playersInside.Count == 0 && !cameraToEnable.gameObject.activeSelf)
            {
                cameraToEnable.gameObject.SetActive(true);
                cameraToDisable.gameObject.SetActive(false);
            }
        }
    }
}