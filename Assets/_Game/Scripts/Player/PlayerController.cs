using UnityEngine;

/// <summary>
/// Provides first person walking, sprinting, crouching and jumping with
/// smoothed horizontal acceleration, custom gravity and a capped downward speed.
///
/// Also accepts carried movement and temporary changes caused by slippery surfaces.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2.2f;

    // Determines how quickly the current horizontal velocity approaches the
    // requested velocity. This softens acceleration and direction changes.
    [SerializeField] private float acceleration = 8f;

    [Header("Jumping / Gravity")]
    [SerializeField] private float jumpHeight = 1.6f;

    // Controls downward acceleration and shapes the player's jump and fall.
    [SerializeField] private float gravity = -12f;

    // Limits the maximum downward velocity to maintain a controlled fall.
    [SerializeField] private float terminalVelocity = -9f;

    [Header("Crouch")]
    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 1.0f;

    // The assigned camera transform moves vertically when the player changes stance.
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float standCameraHeight = 1.6f;
    [SerializeField] private float crouchCameraHeight = 0.9f;

    private CharacterController controller;

    // Generated C# wrapper for the PlayerControls input-actions asset.
    private PlayerControls controls;

    // Horizontal and vertical velocities are stored separately so horizontal
    // movement can be smoothed without affecting jumping or gravity.
    private Vector2 moveInput;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    // Velocity supplied by another object for the next movement update.
    private Vector3 externalMotion;

    // Temporary influence applied by a slippery surface.
    private float slipperiness;

    private bool isSprinting;
    private bool isCrouching;
    private bool crouchInputHeld;

    /// <summary>
    /// Caches the CharacterController, creates the input-action wrapper and
    /// registers callbacks for button-based actions.
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Jump.performed += _ => TryJump();

        controls.Player.Crouch.performed += _ =>
        {
            crouchInputHeld = true;
            SetCrouch(true);
        };

        controls.Player.Crouch.canceled += _ =>
        {
            crouchInputHeld = false;
            SetCrouch(false);
        };

        controls.Player.Sprint.performed += _ => isSprinting = true;
        controls.Player.Sprint.canceled += _ => isSprinting = false;
    }

    /// <summary>
    /// Enables the generated input actions while the component is active.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();
    }

    /// <summary>
    /// Disables the generated input actions while the component is inactive.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// Reads movement input, updates velocity and moves the CharacterController.
    /// </summary>
    private void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        // Keep trying to stand after crouch is released. SetCrouch prevents the
        // change until the space above the player is clear.
        if (isCrouching && !crouchInputHeld)
        {
            SetCrouch(false);
        }

        // Crouching takes priority over sprinting when selecting movement speed.
        float targetSpeed = isCrouching
            ? crouchSpeed
            : isSprinting
                ? sprintSpeed
                : walkSpeed;

        // Convert two-dimensional input into movement relative to the direction
        // the player is currently facing.
        Vector3 targetVelocity =
            (transform.right * moveInput.x + transform.forward * moveInput.y)
            * targetSpeed;

        // Slipperiness reduces acceleration so existing momentum lasts longer.
        float effectiveAcceleration = Mathf.Lerp(
            acceleration,
            acceleration * 0.15f,
            slipperiness
        );

        horizontalVelocity = Vector3.Lerp(
            horizontalVelocity,
            targetVelocity,
            effectiveAcceleration * Time.deltaTime
        );

        // Apply gravity and limit the maximum downward velocity.
        verticalVelocity += gravity * Time.deltaTime;

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity = terminalVelocity;
        }

        // Maintain slight downward pressure while grounded.
        if (controller.isGrounded && verticalVelocity < -1f)
        {
            verticalVelocity = -1f;
        }

        Vector3 motion =
            horizontalVelocity
            + Vector3.up * verticalVelocity
            + externalMotion;

        controller.Move(motion * Time.deltaTime);

        // External movement must be supplied again for every affected frame.
        externalMotion = Vector3.zero;
        slipperiness = 0f;
    }

    /// <summary>
    /// Starts a jump when the player is grounded and not crouching.
    /// </summary>
    private void TryJump()
    {
        if (controller.isGrounded && !isCrouching)
        {
            // Calculate the upward velocity required to reach the configured
            // jump height under the current gravity value.
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    /// <summary>
    /// Enters or exits the crouched state by updating the movement state,
    /// collider dimensions and camera height.
    /// </summary>
    private void SetCrouch(bool crouch)
    {
        if (!crouch && !HasStandingClearance())
        {
            return;
        }

        isCrouching = crouch;

        // Keep the lower edge of the collider aligned with the ground when its
        // height changes.
        controller.height = crouch ? crouchHeight : standHeight;
        controller.center = new Vector3(
            0f,
            controller.height / 2f,
            0f
        );

        if (playerCamera != null)
        {
            Vector3 cameraPosition = playerCamera.localPosition;
            cameraPosition.y = crouch
                ? crouchCameraHeight
                : standCameraHeight;

            playerCamera.localPosition = cameraPosition;
        }
    }

    /// <summary>
    /// Checks the space that the upper part of the standing collider would occupy.
    /// The player's own colliders and trigger volumes are ignored.
    /// </summary>
    private bool HasStandingClearance()
    {
        if (controller.height >= standHeight)
        {
            return true;
        }

        float castRadius = Mathf.Max(0.01f, controller.radius - 0.01f);

        Vector3 controllerBottom = transform.TransformPoint(
            controller.center - Vector3.up * (controller.height * 0.5f)
        );

        Vector3 castOrigin =
            controllerBottom
            + transform.up * (controller.height - controller.radius);

        float castDistance = standHeight - controller.height;

        RaycastHit[] hits = Physics.SphereCastAll(
            castOrigin,
            castRadius,
            transform.up,
            castDistance,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.collider.transform;

            bool isPlayerCollider =
                hit.collider == controller
                || hitTransform == transform
                || hitTransform.IsChildOf(transform);

            if (!isPlayerCollider)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Adds world-space movement inherited from an external object.
    /// </summary>
    public void AddExternalMotion(Vector3 velocity)
    {
        externalMotion += velocity;
    }

    /// <summary>
    /// Replaces the current vertical velocity with an externally supplied
    /// upward velocity.
    /// </summary>
    public void Launch(float upwardVelocity)
    {
        verticalVelocity = upwardVelocity;
    }

    /// <summary>
    /// Sets the temporary influence of a slippery surface on acceleration.
    /// </summary>
    public void SetSlippery(float amount)
    {
        slipperiness = Mathf.Clamp01(amount);
    }

    /// <summary>
    /// Moves the player directly to a world-space position and clears stored
    /// movement so momentum does not continue after teleporting.
    /// </summary>
    public void TeleportTo(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;

        horizontalVelocity = Vector3.zero;
        verticalVelocity = 0f;
        externalMotion = Vector3.zero;
        slipperiness = 0f;
    }
}