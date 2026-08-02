using System.Collections;
using UnityEngine;

/// <summary>
/// Shakes after the player steps on it, temporarily disables its visible and
/// solid parts, then restores itself after a configurable delay.
/// </summary>
public class CrumblingPlatform : MonoBehaviour
{
    [Header("Crumble")]
    [SerializeField] private float shakeDuration = 0.8f;
    [SerializeField] private float shakeAmount = 0.05f;
    [SerializeField] private float respawnDelay = 4f;

    [Header("References")]
    [SerializeField] private Renderer platformRenderer;
    [SerializeField] private Collider solidCollider;

    private Vector3 startingPosition;
    private bool triggered;

    // Store the original position so shaking and restoration remain consistent.
    private void Awake()
    {
        startingPosition = transform.position;
    }

    // Begin the crumble sequence when the player enters the platform trigger.
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.TryGetComponent(out PlayerController _))
        {
            StartCoroutine(Crumble());
        }
    }

    /// <summary>
    /// Shakes the platform as a warning, disables it temporarily and restores it
    /// after the configured delay.
    /// </summary>
    private IEnumerator Crumble()
    {
        triggered = true;
        float elapsedTime = 0f;

        // Shake horizontally so the collider's top surface remains at a stable
        // height and does not interrupt the player's grounded state.
        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;

            Vector2 horizontalShake =
                Random.insideUnitCircle * shakeAmount;

            transform.position =
                startingPosition
                + new Vector3(
                    horizontalShake.x,
                    0f,
                    horizontalShake.y
                );

            yield return null;
        }

        // Reset the position before hiding the platform and disabling collision.
        transform.position = startingPosition;
        platformRenderer.enabled = false;
        solidCollider.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Restore the platform and allow it to be triggered again.
        platformRenderer.enabled = true;
        solidCollider.enabled = true;
        triggered = false;
    }
}