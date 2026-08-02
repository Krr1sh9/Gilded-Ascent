using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tracks required treasure and controls the exit in the current scene.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Exit")]
    [SerializeField] private GameObject exitBlocker;
    [SerializeField] private string nextSceneName;

    public int RequiredTreasureCount { get; private set; }
    public int CollectedTreasureCount { get; private set; }
    public bool IsExitOpen { get; private set; }

    public event Action OnTreasureCountChanged;

    /// <summary>
    /// Adds one required treasure to the total for this scene.
    /// </summary>
    public void RegisterTreasure()
    {
        RequiredTreasureCount++;
        OnTreasureCountChanged?.Invoke();
    }

    /// <summary>
    /// Records one collected treasure without exceeding the registered total.
    /// </summary>
    public void CollectTreasure()
    {
        if (CollectedTreasureCount >= RequiredTreasureCount)
        {
            return;
        }

        CollectedTreasureCount++;
        OnTreasureCountChanged?.Invoke();

        if (CollectedTreasureCount >= RequiredTreasureCount)
        {
            OpenExit();
        }
    }

    /// <summary>
    /// Loads the configured scene only after the exit has opened.
    /// </summary>
    public void LoadNextScene()
    {
        if (!IsExitOpen || string.IsNullOrWhiteSpace(nextSceneName))
        {
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// Removes the blocker after all required treasure has been collected.
    /// </summary>
    private void OpenExit()
    {
        if (IsExitOpen)
        {
            return;
        }

        IsExitOpen = true;

        if (exitBlocker != null)
        {
            exitBlocker.SetActive(false);
        }
    }
}