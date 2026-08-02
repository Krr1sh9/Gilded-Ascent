using UnityEngine;

/// <summary>
/// Detects the Player entering an open level exit.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExitZone : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        if (levelManager != null)
        {
            levelManager.LoadNextScene();
        }
    }
}