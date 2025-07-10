using GlitchInThePast.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.GenericMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IPauseable
    {
        #region Variables
        [Tooltip("Walking speed")]
        public float walkingSpeed = 5f;
        [Tooltip("This number gets multiplied to the walking speed")]
        public float runningSpeed = 1.5f;
        [Tooltip("Dashing speed")]
        public float dashSpeed = 12f;
        [Tooltip("How long it takes players to dash (in seconds)")]
        public float dashDuration = 0.2f;
        [Tooltip("How long till players are premitted to dash again aka dash cooldown (in seconds)")]
        public float dashCooldown = 1f;

        private CharacterController characterController;
        private PlayerInput playerInput;
        private Vector2 moveInput;
        [SerializeField] private Rotator rotator;
        private bool isRunning;
        private bool isDashing;
        private float dashTimer;
        private float dashCooldownTimer;
        private float verticalVel;

        private SpriteRenderer spriteRenderer;

        // Added by JW
        [SerializeField] private PlayerWeaponSystem weaponSystem;
        [SerializeField] private Transform attackTransformHolder;
        #endregion

        void Start()
        {
            GamePauser.Instance?.RegisterPauseable(this);
        }

        void OnDestroy()
        {
            GamePauser.Instance?.UnregisterPauseable(this);
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
                Debug.LogError("There is no sprite Renderer, can't flip the sprite! ADD ONE NOW");

            weaponSystem = GetComponent<PlayerWeaponSystem>();
        }

        private void Update()
        {
            dashCooldownTimer -= Time.deltaTime;
            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                    isDashing = false;
            }

            if (spriteRenderer)
            {
                if (moveInput.x > 0.1f)
                {
                    spriteRenderer.flipX = false;
                    FlipAttackTransform(1);
                }
                else if (moveInput.x < -0.1f)
                {
                    spriteRenderer.flipX = true;
                    FlipAttackTransform(-1);
                }
            }

            // Vector2 aimInput = playerInput.actions["Aim"].ReadValue<Vector2>();
            // Debug.Log(aimInput);
        }

        private void FixedUpdate()
        {
            if (characterController == null) return;

            if (characterController.isGrounded && verticalVel < 0f) verticalVel = -2f;

            verticalVel += Physics.gravity.y * Time.fixedDeltaTime;

            float speed = walkingSpeed * (isRunning ? runningSpeed : 1f);
            Vector3 horizontal = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Vector3 velocity = isDashing ? horizontal * dashSpeed : horizontal * speed;
            velocity.y = verticalVel;

            characterController.Move(velocity * Time.fixedDeltaTime);
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            InputActionAsset action = playerInput.actions;

            #region Player input actions assignment
            if (action["Move"] != null)
            {
                Debug.Log("Adding 'Move' action to input");
                action["Move"].performed += OnMove;
                action["Move"].canceled += OnMove;
            }
            if (action["Aim"] != null)
            {
                Debug.Log("Adding 'Aim' action to input");
                action["Aim"].performed += OnAim;
                action["Aim"].canceled += OnAim;
            }
            if (action["Run"] != null)
            {
                Debug.Log("Adding 'Run' action to input");
                action["Run"].performed += _ => isRunning = true;
                action["Run"].canceled += _ => isRunning = false;
            }
            if (action["Dash"] != null) action["Dash"].performed += _ => Dash();
            if (action["Attack"] != null)
            {
                Debug.Log("Adding 'Attack' action to input");
                action["Attack"].performed += _ => weaponSystem.OnAttack();
            }

            #endregion
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            InputActionAsset action = playerInput.actions;

            if (action["Move"] != null)
            {
                action["Move"].performed -= OnMove;
                action["Move"].canceled -= OnMove;
            }
            if (action["Aim"] != null)
            {
                action["Aim"].performed -= OnAim;
                action["Aim"].canceled -= OnAim;
            }
            if (action["Run"] != null)
            {
                action["Run"].performed -= _ => isRunning = true;
                action["Run"].canceled -= _ => isRunning = false;
            }
            if (action["Dash"] != null)
                action["Dash"].performed -= _ => Dash();
            if (action["Attack"] != null)
            {
                Debug.Log("Adding 'Attack' action to input");
                action["Attack"].performed -= _ => weaponSystem.OnAttack();
            }
        }

        public void SetupAtSpawn(Vector3 spawnPos)
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            characterController.enabled = false;

            transform.position = spawnPos;
            transform.rotation = Quaternion.identity;

            characterController.enabled = true;

            verticalVel = 0f;
            isDashing = false;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnAim(InputAction.CallbackContext ctx)
        {
            // Debug.Log("Aiming");
            rotator.OnAim(ctx.ReadValue<Vector2>());
        }

        private void Dash()
        {
            if (!isDashing && dashCooldownTimer <= 0f && moveInput.sqrMagnitude > 0.1f)
            {
                isDashing = true;
                dashTimer = dashDuration;
                dashCooldownTimer = dashCooldown;
            }
        }

        private void FlipAttackTransform(int direction)
        {
            if (direction == 1)
            {
                attackTransformHolder.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (direction == -1)
            {
                attackTransformHolder.localRotation = Quaternion.Euler(0, -90, 0);
            }
        }

        #region IPauseable functions
        public void OnPause()
        {
            enabled = false;
        }

        public void OnUnpause()
        {
            enabled = true;
        }
        #endregion
    }
}