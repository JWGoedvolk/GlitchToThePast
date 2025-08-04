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
                if (aimInput == Vector2.zero)
                    return;

                Ray ray = Camera.main.ScreenPointToRay(aimInput);
                Plane groundPlane = new Plane(Vector3.back, Vector3.zero);
                if (groundPlane.Raycast(ray, out float rayDistance))
                {
                    Vector3 worldPoint = ray.GetPoint(rayDistance);
                    Vector3 playerDirection = worldPoint - transform.position;

                    if (playerDirection.sqrMagnitude > 0.0001f)
                    {
                        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    }
                }
            }
            else
            {
                if (aimInput.magnitude > controllerDeadZone)
                {
                    float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, controllerRotationSmoothing * Time.deltaTime);
                }
            }

            if (target != null)
                target.position = transform.position + transform.right * 1f;
        }

        public void OnAim(InputAction.CallbackContext ctx)
        {
            Vector2 aimValue = ctx.ReadValue<Vector2>();
            aimInput = aimValue;
            isGamepad = ctx.control.device is Gamepad;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.right * 1.5f);
            if (target != null)
                Gizmos.DrawRay(target.position, target.right * 1.5f);
        }
    }
}