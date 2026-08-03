using System;
using UnityEngine;

/// <summary>
/// Tracks the player's health and oxygen, notifies listeners when either value
/// changes, and restores both values when either resource is depleted.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("Oxygen")]
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float oxygenDrainPerSecond = 6f;

    public int Health { get; private set; }
    public float Oxygen { get; private set; }

    // Gameplay systems can enable or disable oxygen loss as required.
    public bool OxygenDraining { get; set; }

    // Raised whenever either stat changes so dependent systems can update.
    public event Action OnStatsChanged;

    // Stores the time at which the player can next receive damage.
    private float invulnerableUntil;

    private CheckpointManager checkpointManager;

    private void Awake()
    {
        checkpointManager = FindFirstObjectByType<CheckpointManager>();
    }

    private void Start()
    {
        Health = maxHealth;
        Oxygen = maxOxygen;
        OnStatsChanged?.Invoke();
    }

    private void Update()
    {
        if (!OxygenDraining)
        {
            return;
        }

        Oxygen -= oxygenDrainPerSecond * Time.deltaTime;
        Oxygen = Mathf.Max(0f, Oxygen);
        OnStatsChanged?.Invoke();

        if (Oxygen <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Begins reducing oxygen every frame.
    /// </summary>
    public void StartOxygenDrain()
    {
        if (OxygenDraining)
        {
            return;
        }

        OxygenDraining = true;
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Stops oxygen loss and optionally restores oxygen to its maximum value.
    /// </summary>
    public void StopOxygenDrain(bool restoreOxygen)
    {
        bool statsChanged = OxygenDraining;

        OxygenDraining = false;

        if (restoreOxygen && Oxygen < maxOxygen)
        {
            Oxygen = maxOxygen;
            statsChanged = true;
        }

        if (statsChanged)
        {
            OnStatsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Reduces health unless the player is currently protected by the brief
    /// invulnerability period following a previous hit.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (Time.time < invulnerableUntil)
        {
            return;
        }

        invulnerableUntil = Time.time + 1f;

        Health -= amount;
        OnStatsChanged?.Invoke();

        if (Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Restores health without allowing it to exceed the configured maximum.
    /// </summary>
    public void Heal(int amount)
    {
        Health = Mathf.Min(maxHealth, Health + amount);
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Restores oxygen without allowing it to exceed the configured maximum.
    /// </summary>
    public void RefillOxygen(float amount)
    {
        Oxygen = Mathf.Min(maxOxygen, Oxygen + amount);
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Restores the player's resources and returns them to the current checkpoint.
    /// </summary>
    public void Die()
    {
        Health = maxHealth;
        Oxygen = maxOxygen;
        OxygenDraining = false;
        invulnerableUntil = Time.time + 1f;

        OnStatsChanged?.Invoke();

        if (checkpointManager == null)
        {
            checkpointManager = FindFirstObjectByType<CheckpointManager>();
        }

        checkpointManager?.RespawnPlayer();
    }
}