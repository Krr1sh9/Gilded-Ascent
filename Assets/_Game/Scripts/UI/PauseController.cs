using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the gameplay pause state and pause-menu actions.
/// </summary>
public class PauseController : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button settingsBackButton;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Player Controls")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MouseLook mouseLook;

    public bool IsPaused { get; private set; }

    // Finds scene references that were not assigned through the Inspector.
    private void Awake()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        if (mouseLook == null)
        {
            mouseLook = FindFirstObjectByType<MouseLook>();
        }
    }

    // Connects each pause-menu button to its action.
    private void OnEnable()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(ShowSettingsWindow);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.AddListener(ShowPauseWindow);
        }
    }

    // Starts gameplay in an unpaused state.
    private void Start()
    {
        SetPaused(false);
    }

    // Handles Escape for pausing, resuming and leaving the Settings window.
    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (IsPaused &&
            settingsWindow != null &&
            settingsWindow.activeSelf)
        {
            ShowPauseWindow();
            return;
        }

        SetPaused(!IsPaused);
    }

    // Disconnects each button and restores normal time when disabled.
    private void OnDisable()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartLevel);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(ShowSettingsWindow);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(LoadMainMenu);
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveListener(ShowPauseWindow);
        }

        Time.timeScale = 1f;
    }

    // Resumes gameplay from the pause menu.
    public void ResumeGame()
    {
        SetPaused(false);
    }

    // Reloads the current scene from its original state.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Returns from gameplay to the Main Menu scene.
    public void LoadMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Displays the normal pause-menu buttons.
    public void ShowPauseWindow()
    {
        if (pauseWindow != null)
        {
            pauseWindow.SetActive(true);
        }

        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }
    }

    // Displays the Settings controls while keeping gameplay paused.
    public void ShowSettingsWindow()
    {
        if (pauseWindow != null)
        {
            pauseWindow.SetActive(false);
        }

        if (settingsWindow != null)
        {
            settingsWindow.SetActive(true);
        }
    }

    // Applies the requested pause state to gameplay, UI and cursor controls.
    private void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        ShowPauseWindow();

        if (pausePanel != null)
        {
            pausePanel.SetActive(paused);
        }

        if (playerController != null)
        {
            playerController.enabled = !paused;
        }

        if (mouseLook != null)
        {
            mouseLook.enabled = !paused;
        }

        Cursor.lockState = paused
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = paused;
    }
}