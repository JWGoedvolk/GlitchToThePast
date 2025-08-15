using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchInThePast.Scripts.Player
{
    public class Rotator : MonoBehaviour
    {
        #region Variables
        [Header("General")]
        private Vector2 aimInput;
        [SerializeField] private Transform target;

        [Header("Controller")]
        [SerializeField] private bool isGamepad;
        [SerializeField] private float controllerDeadZone = 0.01f;
        [SerializeField] private float controllerRotationSmoothing = 1000f;

        [Header("Visual / Facing")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Clamp")]
        [SerializeField] private float clampRightMin = -0f;
        [SerializeField] private float clampRightMax = 175f;
        [SerializeField] private float clampLeftMin = -175f;
        [SerializeField] private float clampLeftMax = 0f;

        public int FacingDirection { get; private set; } = 1;
        private int lastAppliedFacing = 1;

        private int pendingFacing;
        private bool hasPendingFacing = false;
        #endregion

        private void Update()
        {
            int derivedFacing = DeriveFacingDirection();

            if (derivedFacing != lastAppliedFacing)
            {
                FacingDirection = derivedFacing;
                pendingFacing = derivedFacing;
                hasPendingFacing = true;
            }

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
                        float clamped = ClampToFacingCone(angle);
                        transform.rotation = Quaternion.Euler(0f, 0f, clamped);
                    }
                }
            }
            else
            {
                if (aimInput.magnitude > controllerDeadZone)
                {
                    float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
                    float clamped = ClampToFacingCone(angle);
                    Quaternion targetRotation = Quaternion.Euler(0f, 0f, clamped);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, controllerRotationSmoothing * Time.deltaTime);
                }
            }

            if (target != null)
                target.position = transform.position + transform.right * 1f;
        }

        private void LateUpdate()
        {
            if (!hasPendingFacing) return;

            UpdateSpriteFlips(pendingFacing);
            lastAppliedFacing = pendingFacing;
            hasPendingFacing = false;
        }

        #region Public Functions
        public void OnAim(InputAction.CallbackContext ctx)
        {
            Vector2 aimValue = ctx.ReadValue<Vector2>();
            aimInput = aimValue;
            isGamepad = ctx.control.device is Gamepad;
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Derives left/right facing from input: stick horizontal or mouse position.
        /// </summary>
        private int DeriveFacingDirection()
        {
            if (isGamepad && Mathf.Abs(aimInput.x) > controllerDeadZone)
            {
                return aimInput.x > 0f ? 1 : -1;
            }

            if (!isGamepad && aimInput != Vector2.zero)
            {
                Ray ray = Camera.main.ScreenPointToRay(aimInput);
                Plane groundPlane = new Plane(Vector3.back, Vector3.zero);
                if (groundPlane.Raycast(ray, out float rayDistance))
                {
                    Vector3 worldPoint = ray.GetPoint(rayDistance);
                    float dx = worldPoint.x - transform.position.x;
                    if (Mathf.Abs(dx) > 0.01f)
                        return dx > 0f ? 1 : -1;
                }
            }

            return FacingDirection;
        }

        private float ClampToFacingCone(float rawAngleDeg)
        {
            float baseAngle = (FacingDirection == 1) ? 0f : 180f;

            float delta = Mathf.DeltaAngle(baseAngle, rawAngleDeg);

            float min = (FacingDirection == 1) ? clampRightMin : clampLeftMin;
            float max = (FacingDirection == 1) ? clampRightMax : clampLeftMax;

            float clampedDelta = Mathf.Clamp(delta, min, max);
            return baseAngle + clampedDelta;
        }

        private void UpdateSpriteFlips(int direction)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (direction == 1)
            {
                spriteRenderer.flipY = false;
            }
            else if (direction == -1)
            {
                spriteRenderer.flipY = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.right * 1.5f);
            if (target != null)
                Gizmos.DrawRay(target.position, target.right * 1.5f);
        }
        #endregion
    }
}
