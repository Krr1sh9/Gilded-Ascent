using UnityEngine;

/// <summary>
/// Stores the current respawn position for this scene and returns the player
/// to it through PlayerController.TeleportTo.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private Vector3 respawnPosition;

    private void Start()
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
    /// Replaces the current respawn position with an activated checkpoint.
    /// </summary>
    public void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
    }

    /// <summary>
    /// Returns the player to the current respawn position.
    /// </summary>
    public void RespawnPlayer()
    {
        if (player == null)
        {
            return;
        }

        player.TeleportTo(respawnPosition);
    }
}