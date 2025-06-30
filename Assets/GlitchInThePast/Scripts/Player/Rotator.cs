using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchInThePast.Scripts.Player
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Vector2 aimInput;
        [SerializeField] private Vector3 inNormal;
        [SerializeField] private InputAction aim;

        private void Awake()
        {
            aim = playerInput.actions.FindAction("Aim");
        }

        private void Update()
        {
            aimInput = aim.ReadValue<Vector2>();
            Debug.Log(aimInput);
            
            Ray ray = Camera.main.ScreenPointToRay(aimInput);
            Plane groundPlane = new Plane(inNormal, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                transform.LookAt(point, Vector3.forward);
            }
        }

        public void OnAim(Vector2 aim)
        {
            aimInput = aim;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Debug.DrawLine(transform.position, aimInput);
        }
    }
}