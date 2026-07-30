using UnityEngine;

/// <summary>
/// Launches the player upward when they enter the jump pad's trigger and
/// optionally plays an assigned particle effect.
/// </summary>
public class JumpPad : MonoBehaviour
{
    [SerializeField] private float launchVelocity = 12f;
    [SerializeField] private ParticleSystem burstEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.Launch(launchVelocity);

            if (burstEffect != null)
            {
                burstEffect.Play();
            }
        }
    }
}