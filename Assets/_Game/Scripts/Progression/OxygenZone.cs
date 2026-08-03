using UnityEngine;

/// <summary>
/// Starts oxygen loss while the Player is inside this trigger and restores
/// oxygen when the Player leaves.
/// </summary>
[RequireComponent(typeof(Collider))]
public class OxygenZone : MonoBehaviour
{
    private PlayerStats playerInside;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();

        if (playerStats == null)
        {
            return;
        }

        playerInside = playerStats;
        playerInside.StartOxygenDrain();
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();

        if (playerStats == null || playerStats != playerInside)
        {
            return;
        }

        playerInside.StopOxygenDrain(true);
        playerInside = null;
    }

    private void OnDisable()
    {
        if (playerInside == null)
        {
            return;
        }

        playerInside.StopOxygenDrain(true);
        playerInside = null;
    }
}