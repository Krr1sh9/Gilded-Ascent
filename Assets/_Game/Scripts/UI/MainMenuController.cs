using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the buttons and starting state of the Main Menu.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Scenes")]
    [SerializeField] private string firstLevelSceneName = "Level1_Graveyard";

    // Connects the Main Menu buttons when the component becomes active.
    private void OnEnable()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    // Restores normal time and makes the menu cursor available.
    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Disconnects the Main Menu buttons when the component is disabled.
    private void OnDisable()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(StartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
        }
    }

    // Loads the first gameplay level.
    private void StartGame()
    {
        if (!string.IsNullOrWhiteSpace(firstLevelSceneName))
        {
            SceneManager.LoadScene(firstLevelSceneName);
        }
    }

    // Closes the built game or stops Play Mode inside the Unity Editor.
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}