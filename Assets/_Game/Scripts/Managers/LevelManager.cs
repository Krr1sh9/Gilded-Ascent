using System;
using UnityEngine;

/// <summary>
/// Tracks the required treasure registered and collected in the current scene.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public int RequiredTreasureCount { get; private set; }
    public int CollectedTreasureCount { get; private set; }

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
    }
}