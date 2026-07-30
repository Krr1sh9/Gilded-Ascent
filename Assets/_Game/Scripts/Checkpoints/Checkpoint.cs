using UnityEngine;

/// <summary>
/// Registers a new respawn position when the player enters the trigger.
/// Each checkpoint can only activate once.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Renderer checkpointRenderer;
    [SerializeField] private Material activatedMaterial;

    private CheckpointManager checkpointManager;
    private bool activated;

    private void Awake()
    {
        // Find the manager responsible for storing the current respawn position.
        checkpointManager = FindFirstObjectByType<CheckpointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore repeated activation and objects that are not the player.
        if (activated || !other.TryGetComponent(out PlayerController _))
        {
            return;
        }

        // Try again if the manager was added or enabled after this checkpoint.
        if (checkpointManager == null)
        {
            checkpointManager = FindFirstObjectByType<CheckpointManager>();
        }

        if (checkpointManager == null)
        {
            return;
        }

        // Use the assigned respawn point, or this object's position as a fallback.
        Vector3 position = respawnPoint != null
            ? respawnPoint.position
            : transform.position;

        checkpointManager.SetCheckpoint(position);
        activated = true;

        if (checkpointRenderer != null && activatedMaterial != null)
        {
            checkpointRenderer.sharedMaterial = activatedMaterial;
        }
    }
}