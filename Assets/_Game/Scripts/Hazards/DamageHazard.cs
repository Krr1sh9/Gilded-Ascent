using UnityEngine;

/// <summary>
/// Applies a fixed amount of damage when the player enters this trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageHazard : MonoBehaviour
{
    [SerializeField] private int damage = 25;

    private void OnTriggerEnter(Collider other)
    {
        // Only objects containing PlayerStats can receive damage.
        if (other.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.TakeDamage(damage);
        }
    }
}