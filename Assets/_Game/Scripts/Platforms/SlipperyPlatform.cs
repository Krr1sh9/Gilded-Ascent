using UnityEngine;

/// <summary>
/// Reduces the player's movement acceleration while they remain inside the
/// platform's trigger.
/// </summary>
public class SlipperyPlatform : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float slipperiness = 0.9f;

    private PlayerController playerOnBoard;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            playerOnBoard = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player)
            && player == playerOnBoard)
        {
            playerOnBoard = null;
        }
    }

    private void LateUpdate()
    {
        // The value must be applied again while the player remains on the surface.
        if (playerOnBoard != null)
        {
            playerOnBoard.SetSlippery(slipperiness);
        }
    }
}