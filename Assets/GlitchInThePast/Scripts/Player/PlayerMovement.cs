using System;
using GlitchInThePast.Scripts.Player;
using Player.Health;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.GenericMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IPauseable
    {
        #region Variables
        [Tooltip("Movement")]
        public float walkingSpeed = 2f;
        [Tooltip("This number gets multiplied to the walking speed")]
        public float runningSpeed = 1.5f;

        public bool initialiserUnlockedMovement = false;

        [Header("Dashing")]
        [SerializeField] private float dashSpeed = 12f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float dashCooldown = 1f;
        private float dashTimer;
        private float dashCooldownTimer;
        private bool isDashing;
        public bool IsDashing => isDashing;
        private float dashDurationTimer;
        private Vector3 dashDirection = Vector3.zero;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 4f;
        [Tooltip("The higher the number the stronger gravity is when falling")]
        [SerializeField] private float gravityMultiplier = 1.5f;

        [SerializeField] private Animator animator;

        public static float MaxXDistance = 14f;
        public static float MaxZDistance = 9f;

        [SerializeField] private bool isMovementLocked = true; // Please keep true as default.
        [SerializeField] private PlayerHealthSystem healthSystem;
        private PlayerMovement otherPlayer;
        private CharacterController characterController;
        private PlayerInput playerInput;
        private Vector2 moveInput;
        [SerializeField] private Rotator rotator;
        private bool isRunning;
        private float verticalVel;

        private SpriteRenderer spriteRenderer;

        [Header("Weapon Related")]
        [SerializeField] private PlayerWeaponSystem weaponSystem;
        [SerializeField] private Transform attackTransformHolder;

        [Header("Events")]
        [SerializeField] private UnityEvent onDashStart;
        [SerializeField] private UnityEvent onDashEnd;
        #endregion

        void Start()
        {
            healthSystem = GetComponent<PlayerHealthSystem>();
            dashDirection = Vector3.right;
            InGameButtons.Instance?.RegisterPauseable(this);
            AssignOtherPlayer();
            if (animator is null)
            {
                animator = GetComponent<Animator>();
            }
        }

        void OnDestroy()
        {
            InGameButtons.Instance?.UnregisterPauseable(this);
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
                Debug.LogError("There is no sprite Renderer, can't flip the sprite! ADD ONE NOW");


            weaponSystem = GetComponent<PlayerWeaponSystem>();
            if (rotator == null)
            {
                rotator = GetComponentInChildren<Rotator>();
            }
        }

        private void Update()
        {
            if (isMovementLocked == true) return;

            // Update timers
            dashCooldownTimer -= Time.deltaTime;

            if (isDashing)
            {
                dashTimer -= Time.deltaTime;

                if (dashTimer <= 0f)
                {
                    isDashing = false;
                    animator?.SetBool("isDashing", false);
                    onDashEnd?.Invoke();

                    if (healthSystem != null) healthSystem.isInvincible = false;
                }
            }

            if (characterController == null) return;

            if (characterController.isGrounded && verticalVel < 0f)
            {
                verticalVel = -2f;
            }

            verticalVel += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

            float speed = walkingSpeed * (isRunning ? runningSpeed : 1f);
            Vector3 horizontal = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Vector3 velocity = isDashing ? dashDirection * dashSpeed : horizontal * speed;
            velocity.y = verticalVel;

            if (otherPlayer == null || otherPlayer == this)
                AssignOtherPlayer();

            if (otherPlayer != null)
            {
                Vector3 nextPosition = transform.position + (velocity * Time.deltaTime);
                Vector3 otherPos = otherPlayer.transform.position;
                Vector3 delta = nextPosition - otherPos;

                if (Mathf.Abs(delta.x) > MaxXDistance || Mathf.Abs(delta.z) > MaxZDistance)
                {
                    Vector3 currentDelta = transform.position - otherPos;
                    bool movingFurtherX = Mathf.Abs(delta.x) > Mathf.Abs(currentDelta.x);
                    bool movingFurtherZ = Mathf.Abs(delta.z) > Mathf.Abs(currentDelta.z);

                    if ((MaxXDistance > 0 && movingFurtherX && Mathf.Abs(delta.x) > MaxXDistance) || (MaxZDistance > 0 && movingFurtherZ && Mathf.Abs(delta.z) > MaxZDistance))
                    {
                        if (isDashing)
                        {
                            onDashEnd?.Invoke();
                            isDashing = false;
                        }
                        return;
                    }
                }
            }

            bool isMoving = moveInput.magnitude > 0.1f;
            if (animator != null)
            {
                animator.SetBool("isWalking", isMoving);
                animator.SetBool("isRunning", isMoving && isRunning);
            }

            characterController.Move(velocity * Time.deltaTime);

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
            if (isMovementLocked == false) return;
            if (initialiserUnlockedMovement == true)
            {
                isMovementLocked = false;
            }
        }

        private void OnEnable()
        {
            if (playerInput == null) return;
            InputActionAsset action = playerInput.actions;

            #region Player input actions assignment
            if (action["Move"] != null)
            {
                // Debug.Log("Adding 'Move' action to input");
                action["Move"].performed += OnMove;
                action["Move"].canceled += OnMove;
            }
            if (action["Aim"] != null)
            {
                // Debug.Log("Adding 'Aim' action to input");
                action["Aim"].performed += OnAim;
                action["Aim"].canceled += OnAim;
            }
            if (action["Run"] != null)
            {
                // Debug.Log("Adding 'Run' action to input");
                action["Run"].performed += _ => isRunning = true;
                action["Run"].canceled += _ => isRunning = false;
            }
            if (action["Dash"] != null) action["Dash"].performed += _ => Dash();
            if (action["Attack"] != null)
            {
                // Debug.Log("Adding 'Attack' action to input");
                action["Attack"].performed += _ => weaponSystem.OnAttack();
            }
            if (action["Jump"] != null)
            {
                action["Jump"].performed += OnJump;
            }

            #endregion
        }

        private void OnDisable()
        {
            if (playerInput == null) return;
            InputActionAsset action = playerInput.actions;

            #region Player input actions removal
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
                action["Attack"].performed -= _ => weaponSystem.OnAttack();
            }
            if (action["Jump"] != null)
            {
                action["Jump"].performed -= OnJump;
            }
            #endregion
        }

        #region Public Functions
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
        #endregion

        #region Private Functions
        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        private void AssignOtherPlayer()
        {
            foreach (var pm in FindObjectsOfType<PlayerMovement>())
            {
                if (pm != this)
                {
                    otherPlayer = pm;
                    break;
                }
            }
        }

        private void OnAim(InputAction.CallbackContext ctx)
        {
            if (rotator != null)
            {
                rotator.OnAim(ctx);
            }
        }


        private void Dash()
        {
            if (!isDashing && dashCooldownTimer <= 0f)
            {
                isDashing = true;
                if (healthSystem != null) healthSystem.isInvincible = true;
                dashTimer = dashDuration;
                dashCooldownTimer = dashCooldown;

                Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                if (inputDirection.magnitude > 0.01f)
                {
                    dashDirection = inputDirection;
                }
                else
                {
                    dashDirection = spriteRenderer.flipX ? Vector3.left : Vector3.right;
                }

                animator?.SetBool("isDashing", true);
                onDashStart?.Invoke();
            }
        }


        private void FlipAttackTransform(int direction)
        {
            if (attackTransformHolder == null) return;
            if (direction == 1)
            {
                attackTransformHolder.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (direction == -1)
            {
                attackTransformHolder.localRotation = Quaternion.Euler(0, -90, 0);
            }
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && characterController.isGrounded)
            {
                verticalVel = jumpForce;
            }
        }
        #endregion

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