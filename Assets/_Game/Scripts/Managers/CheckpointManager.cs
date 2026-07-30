using UnityEngine;

/// <summary>
/// Stores the current respawn position and returns the player to it.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private Vector3 respawnPosition;

    private void Awake()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (player != null)
        {
            respawnPosition = player.transform.position;
        }
    }

    /// <summary>
    /// Updates the position used for future respawns.
    /// </summary>
    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
    }

    /// <summary>
    /// Returns the player to the stored respawn position.
    /// </summary>
    [ContextMenu("Respawn Player")]
    public void RespawnPlayer()
    {
        if (player != null)
        {
            player.TeleportTo(respawnPosition);
        }
    }
}