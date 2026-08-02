using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles first-person mouse look using the physical mouse while filtering
/// abnormal input spikes and limiting the camera's vertical rotation.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    // Mouse delta is already measured per input update, so it should not be
    // multiplied by Time.deltaTime.
    [SerializeField] private float baseSensitivity = 0.08f;

    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    // Prevents an abnormal input event from rotating the camera by a large
    // amount in one frame.
    [SerializeField] private float maximumDeltaPerFrame = 100f;

    private PlayerControls controls;
    private float pitch;
    private bool ignoreNextLookFrame;

    /// <summary>
    /// Creates the input wrapper, restricts it to a physical mouse and records
    /// the camera's starting pitch.
    /// </summary>
    private void Awake()
    {
        controls = new PlayerControls();
        RestrictInputToPhysicalMouse();

        if (cameraTransform != null)
        {
            pitch = cameraTransform.localEulerAngles.x;

            if (pitch > 180f)
            {
                pitch -= 360f;
            }

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    /// <summary>
    /// Prevents a VirtualMouse device from controlling first-person camera look.
    /// </summary>
    private void RestrictInputToPhysicalMouse()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (device is not Mouse mouse)
            {
                continue;
            }

            if (mouse.name.StartsWith("VirtualMouse"))
            {
                continue;
            }

            controls.devices = new InputDevice[] { mouse };
            return;
        }
    }

    /// <summary>
    /// Enables input and locks the cursor while the component is active.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ignoreNextLookFrame = true;
    }

    /// <summary>
    /// Disables input and releases the cursor when the component is inactive.
    /// </summary>
    private void OnDisable()
    {
        controls.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Ignores the first mouse-delta event after the game regains focus because
    /// cursor locking can sometimes produce an unusually large value.
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ignoreNextLookFrame = true;
        }
    }

    /// <summary>
    /// Reads physical mouse movement and applies horizontal and vertical look.
    /// </summary>
    private void Update()
    {
        if (cameraTransform == null)
        {
            return;
        }

        if (ignoreNextLookFrame)
        {
            ignoreNextLookFrame = false;
            return;
        }

        Vector2 rawLookInput =
            controls.Player.Look.ReadValue<Vector2>();

        bool inputIsInvalid =
            float.IsNaN(rawLookInput.x)
            || float.IsNaN(rawLookInput.y)
            || float.IsInfinity(rawLookInput.x)
            || float.IsInfinity(rawLookInput.y);

        if (inputIsInvalid)
        {
            return;
        }

        rawLookInput = Vector2.ClampMagnitude(
            rawLookInput,
            maximumDeltaPerFrame
        );

        Vector2 lookInput = rawLookInput * baseSensitivity;

        // Rotate the Player horizontally.
        transform.Rotate(
            Vector3.up,
            lookInput.x,
            Space.Self
        );

        // Rotate only the camera vertically.
        pitch = Mathf.Clamp(
            pitch - lookInput.y,
            minPitch,
            maxPitch
        );

        cameraTransform.localRotation = Quaternion.Euler(
            pitch,
            0f,
            0f
        );
    }
}