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
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite leftSprite;

        [Header("Attack")]
        [SerializeField] private Transform attackTransformHolder;

        public int FacingDirection { get; private set; } = 1;
        private int lastAppliedFacing = 1;
        #endregion

        private void Update()
        {
            int derivedFacing = DeriveFacingDirection();

            if (derivedFacing != lastAppliedFacing)
            {
                FacingDirection = derivedFacing;
                UpdateSpriteVisuals(FacingDirection);
                lastAppliedFacing = FacingDirection;
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

        /// <summary>
        /// Updates sprite visuals to suit the direction the player is facing.
        /// </summary>
        private void UpdateSpriteVisuals(int direction)
        {
            if (spriteRenderer == null) return;

            if (direction == 1)
            {
                if (spriteRenderer.sprite != rightSprite)
                    spriteRenderer.sprite = rightSprite;
            }
            else if (direction == -1)
            {
                if (spriteRenderer.sprite != leftSprite)
                    spriteRenderer.sprite = leftSprite;
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