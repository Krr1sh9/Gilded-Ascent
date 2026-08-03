using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles stable first-person mouse look using a physical mouse.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Sensitivity")]
    [SerializeField] private float baseSensitivity = 0.05f;

    [Header("Vertical Rotation")]
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Input Protection")]
    [SerializeField] private float maximumDeltaPerFrame = 100f;

    public float Sensitivity => baseSensitivity;

    private PlayerControls controls;
    private float pitch;
    private bool ignoreNextLookFrame;

    // Creates the input controls and records the camera's starting pitch.
    private void Awake()
    {
        controls = new PlayerControls();
        RestrictInputToPhysicalMouse();

        if (cameraTransform == null)
        {
            return;
        }

        pitch = cameraTransform.localEulerAngles.x;

        if (pitch > 180f)
        {
            pitch -= 360f;
        }

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    // Prevents a virtual mouse from controlling the first-person camera.
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

    // Enables mouse input and locks the cursor.
    private void OnEnable()
    {
        controls.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ignoreNextLookFrame = true;
    }

    // Disables mouse input and releases the cursor.
    private void OnDisable()
    {
        controls.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Ignores the first mouse movement after the game regains focus.
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ignoreNextLookFrame = true;
        }
    }

    // Applies a safe sensitivity value supplied by the Settings slider.
    public void SetSensitivity(float sensitivity)
    {
        baseSensitivity = Mathf.Clamp(sensitivity, 0.01f, 0.15f);
    }

    // Reads mouse movement and rotates the Player and camera.
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

        Vector2 rawLookInput = controls.Player.Look.ReadValue<Vector2>();

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

        // Raw mouse delta is already measured per input update.
        Vector2 lookInput = rawLookInput * baseSensitivity;

        transform.Rotate(
            Vector3.up,
            lookInput.x,
            Space.Self
        );

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