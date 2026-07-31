using UnityEngine;

/// <summary>
/// Either applies fixed damage or immediately respawns the player when entered.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageHazard : MonoBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private bool respawnImmediately;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerStats playerStats))
        {
            return;
        }

        // Fall volumes bypass normal damage and trigger the existing death response.
        if (respawnImmediately)
        {
            playerStats.Die();
            return;
        }

        playerStats.TakeDamage(damage);
    }
}