using System.Collections;
using UnityEngine;

/// <summary>
/// Restores some of the Player's oxygen, temporarily disappears and then
/// becomes available again.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AirBubble : MonoBehaviour
{
    [Header("Oxygen")]
    [SerializeField] private float refillAmount = 45f;

    [Header("Return")]
    [SerializeField] private float returnDelay = 5f;
    [SerializeField] private Renderer bubbleRenderer;

    private Collider triggerCollider;
    private bool isAvailable = true;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        if (bubbleRenderer == null)
        {
            bubbleRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAvailable)
        {
            return;
        }

        PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();

        if (playerStats == null)
        {
            return;
        }

        playerStats.RefillOxygen(refillAmount);
        StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        isAvailable = false;
        triggerCollider.enabled = false;

        if (bubbleRenderer != null)
        {
            bubbleRenderer.enabled = false;
        }

        yield return new WaitForSeconds(returnDelay);

        if (bubbleRenderer != null)
        {
            bubbleRenderer.enabled = true;
        }

        triggerCollider.enabled = true;
        isAvailable = true;
    }
}