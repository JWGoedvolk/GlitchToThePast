using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchInThePast.Scripts.Player
{
    public class Rotator : MonoBehaviour
    {
        [Header("General")]
        private Vector2 aimInput;
        [SerializeField] private Transform target;
        
        [Header("Controller")]
        [SerializeField] private bool isGamepad;
        [SerializeField] private float controllerDeadZone = 0.01f;
        [SerializeField] private float controllerRotationSmoothing = 1000f;

        private void Update()
        {
            if (!isGamepad)
            {
                Ray ray = Camera.main.ScreenPointToRay(aimInput);
                Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);
                float rayDistance;

                if (groundPlane.Raycast(ray, out rayDistance))
                {
                    Vector3 point = ray.GetPoint(rayDistance);
                    Vector3 heightCorrectedPoint = new Vector3(point.x, point.y, transform.position.z);
                    transform.LookAt(heightCorrectedPoint, Vector3.forward);
                }
            }
            else
            {
                if (Mathf.Abs(aimInput.x) > controllerDeadZone || Mathf.Abs(aimInput.y) > controllerDeadZone)
                {
                    Vector3 playerDirection = Vector3.forward * aimInput.x + Vector3.up * aimInput.y;

                    if (playerDirection.sqrMagnitude > 0f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                        Quaternion fixedRotation = new Quaternion(0, 0, targetRotation.x, 1);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, fixedRotation, controllerRotationSmoothing * Time.deltaTime);
                    }
                }
            }
            
            //target.position = transform.position + transform.forward * 1f; // The target transform is the transform from which our projectiles will come so it needs to always be updated
        }

        public void OnAim(Vector2 aim)
        {
            aimInput = aim;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Debug.DrawRay(transform.position, transform.forward);
            Debug.DrawRay(target.position, target.forward);
        }
    }
}