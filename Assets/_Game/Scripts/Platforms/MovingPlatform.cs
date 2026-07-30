using UnityEngine;

/// <summary>
/// Moves a platform through a sequence of waypoints and passes its measured
/// velocity to a player detected within the platform's carry trigger.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float pauseAtWaypoint = 0.5f;

    // The first destination is the second entry in the waypoint array.
    private int targetIndex = 1;

    private float pauseTimer;

    // Calculated from the platform's movement so it can be passed to the player.
    private Vector3 currentVelocity;

    // Stores the player currently detected by the carry trigger.
    private PlayerController playerOnBoard;

    /// <summary>
    /// Moves towards the current waypoint and advances through the waypoint
    /// array whenever a destination is reached.
    /// </summary>
    private void Update()
    {
        // At least two waypoints are required to define a movement path.
        if (waypoints == null || waypoints.Length < 2)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        // Stop movement for the configured duration after reaching a waypoint.
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            currentVelocity = Vector3.zero;
            return;
        }

        Vector3 startingPosition = transform.position;
        Vector3 targetPosition = waypoints[targetIndex].position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Measure the platform's world-space velocity from this frame's movement.
        if (Time.deltaTime > 0f)
        {
            currentVelocity =
                (transform.position - startingPosition) / Time.deltaTime;
        }

        // Advance to the next waypoint and wrap back to the beginning when the
        // end of the array is reached.
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            targetIndex = (targetIndex + 1) % waypoints.Length;
            pauseTimer = pauseAtWaypoint;
        }
    }

    /// <summary>
    /// Supplies the platform's velocity to the player currently on board.
    /// </summary>
    private void LateUpdate()
    {
        if (playerOnBoard != null)
        {
            playerOnBoard.AddExternalMotion(currentVelocity);
        }
    }

    /// <summary>
    /// Records a player when they enter the platform's carry trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            playerOnBoard = player;
        }
    }

    /// <summary>
    /// Clears the stored player when they leave the carry trigger.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player)
            && player == playerOnBoard)
        {
            playerOnBoard = null;
        }
    }
}