using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies values selected through the gameplay Settings menu.
/// </summary>
public class SettingsController : MonoBehaviour
{
    [Header("Master Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeLabel;

    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private MouseLook mouseLook;

    // Finds the Player camera control if it was not assigned in the Inspector.
    private void Awake()
    {
        if (mouseLook == null)
        {
            mouseLook = FindFirstObjectByType<MouseLook>();
        }
    }

    // Connects both sliders and displays their current values.
    private void OnEnable()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(AudioListener.volume);
            masterVolumeSlider.onValueChanged.AddListener(ApplyMasterVolume);
            UpdateMasterVolumeLabel(AudioListener.volume);
        }

        if (mouseSensitivitySlider != null)
        {
            if (mouseLook != null)
            {
                mouseSensitivitySlider.SetValueWithoutNotify(
                    mouseLook.Sensitivity
                );
            }

            mouseSensitivitySlider.onValueChanged.AddListener(
                ApplyMouseSensitivity
            );
        }
    }

    // Disconnects both sliders when this controller is disabled.
    private void OnDisable()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(
                ApplyMasterVolume
            );
        }

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.onValueChanged.RemoveListener(
                ApplyMouseSensitivity
            );
        }
    }

    // Applies the selected value to Unity's global audio volume.
    private void ApplyMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
        UpdateMasterVolumeLabel(AudioListener.volume);
    }

    // Displays the selected master volume as a percentage.
    private void UpdateMasterVolumeLabel(float volume)
    {
        if (masterVolumeLabel == null)
        {
            return;
        }

        int percentage = Mathf.RoundToInt(volume * 100f);
        masterVolumeLabel.text = $"MASTER VOLUME  {percentage}%";
    }

    // Applies the selected sensitivity to the Player's mouse-look control.
    private void ApplyMouseSensitivity(float sensitivity)
    {
        if (mouseLook != null)
        {
            mouseLook.SetSensitivity(sensitivity);
        }
    }
}