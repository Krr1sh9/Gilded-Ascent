using UnityEngine;

/// <summary>
/// Handles first-person mouse look by rotating the player horizontally and
/// the assigned camera vertically within configured pitch limits.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float baseSensitivity = 12f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private PlayerControls controls;
    private float pitch;

    /// <summary>
    /// Creates the generated input-action wrapper.
    /// </summary>
    private void Awake()
    {
        controls = new PlayerControls();
    }

    /// <summary>
    /// Enables input and locks the cursor while the component is active.
    /// </summary>
    private void OnEnable()
    {
        controls.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    /// Reads mouse movement and applies horizontal and vertical rotation.
    /// </summary>
    private void Update()
    {
        Vector2 lookInput =
            controls.Player.Look.ReadValue<Vector2>()
            * baseSensitivity
            * Time.deltaTime;

        // Rotate the player around the vertical axis.
        transform.Rotate(Vector3.up * lookInput.x);

        // Clamp vertical rotation to prevent the camera from turning too far.
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