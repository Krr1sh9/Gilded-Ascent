using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps the gameplay HUD synchronised with the current gameplay values.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private LevelManager levelManager;

    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private float displayedMaxHealth = 100f;

    [Header("Treasure")]
    [SerializeField] private Image treasureFill;
    [SerializeField] private TMP_Text treasureText;

    [Header("Oxygen")]
    [SerializeField] private GameObject oxygenPanel;
    [SerializeField] private Image oxygenFill;
    [SerializeField] private TMP_Text oxygenText;
    [SerializeField] private float displayedMaxOxygen = 100f;
    [SerializeField] private float oxygenDrainPerSecond = 6f;

    [Header("Messages")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;

    // Finds any gameplay references that were not assigned in the Inspector.
    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
        }
    }

    // Subscribes the HUD to gameplay value changes.
    private void OnEnable()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged += RefreshStats;
        }

        if (levelManager != null)
        {
            levelManager.OnTreasureCountChanged += RefreshTreasure;
        }
    }

    // Sets the initial HUD state when the scene begins.
    private void Start()
    {
        if (messageText != null)
        {
            messageText.text = string.Empty;
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        RefreshStats();
        RefreshTreasure();
    }

    // Removes event subscriptions when the HUD is disabled.
    private void OnDisable()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= RefreshStats;
        }

        if (levelManager != null)
        {
            levelManager.OnTreasureCountChanged -= RefreshTreasure;
        }
    }

    // Refreshes all HUD elements controlled by PlayerStats.
    private void RefreshStats()
    {
        if (playerStats == null)
        {
            return;
        }

        UpdateHealthDisplay();
        UpdateOxygenDisplay();
    }

    // Updates the health bar and health number.
    private void UpdateHealthDisplay()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                Mathf.Clamp01(playerStats.Health / displayedMaxHealth);
        }

        if (healthText != null)
        {
            healthText.text = $"HEALTH {playerStats.Health}";
        }
    }

    // Updates the visibility, bar and countdown of the oxygen display.
    private void UpdateOxygenDisplay()
    {
        bool showOxygen = playerStats.OxygenDraining;

        if (oxygenPanel != null)
        {
            oxygenPanel.SetActive(showOxygen);
        }

        if (!showOxygen)
        {
            return;
        }

        if (oxygenFill != null)
        {
            oxygenFill.fillAmount =
                Mathf.Clamp01(playerStats.Oxygen / displayedMaxOxygen);
        }

        if (oxygenText != null)
        {
            // Converts the remaining oxygen value into the approximate time left.
            float secondsRemaining =
                oxygenDrainPerSecond > 0f
                    ? playerStats.Oxygen / oxygenDrainPerSecond
                    : 0f;

            oxygenText.text = $"OXYGEN {secondsRemaining:0.0}s";
        }
    }

    // Updates the treasure number and gold progress bar.
    private void RefreshTreasure()
    {
        if (levelManager == null)
        {
            if (treasureText != null)
            {
                treasureText.text = "TREASURE 0 / 0";
            }

            if (treasureFill != null)
            {
                treasureFill.fillAmount = 0f;
            }

            return;
        }

        int collected = levelManager.CollectedTreasureCount;
        int required = levelManager.RequiredTreasureCount;

        if (treasureText != null)
        {
            treasureText.text = $"TREASURE {collected} / {required}";
        }

        if (treasureFill != null)
        {
            // Prevents division by zero before any treasure has registered.
            treasureFill.fillAmount =
                required > 0
                    ? Mathf.Clamp01((float)collected / required)
                    : 0f;
        }
    }
}