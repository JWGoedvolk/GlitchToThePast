using System.Collections;
using UnityEngine;

namespace Hazard.Laser
{
    public class LaserRoutine : MonoBehaviour
    {
        #region Variables
        [Tooltip("How long the laser stays on for")]
        public float activeTime = 3f;

        [Tooltip("How long the laser stays off for")]
        public float inactiveTime = 2f;

        public GameObject laserVisual;

        [SerializeField] private Laser laser;
        #endregion

        private void Awake()
        {
            if (laserVisual != null) laserVisual.SetActive(false);
        }
        private void Start()
        {
            StartCoroutine(LaserCoroutine());
        }

        #region Private Functions
        private IEnumerator LaserCoroutine()
        {
            while (true)
            {
                #region Laser ON
                SetLaserState(true);
                yield return new WaitForSeconds(activeTime);
                #endregion

                #region Laser Off
                SetLaserState(false);
                yield return new WaitForSeconds(inactiveTime);
                #endregion
            }
        }

        private void SetLaserState(bool active)
        {
            // laser.isLaserActive = active;
            if (laserVisual != null) laserVisual.SetActive(active);
        }
        #endregion
    }
}