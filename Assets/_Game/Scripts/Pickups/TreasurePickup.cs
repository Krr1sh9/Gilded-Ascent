using UnityEngine;

/// <summary>
/// Registers one required treasure, animates it and records its collection.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TreasurePickup : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 2f;

    private LevelManager levelManager;
    private Vector3 startingPosition;
    private bool collected;

    private void Awake()
    {
        startingPosition = transform.position;
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    private void Start()
    {
        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
        }

        levelManager?.RegisterTreasure();
    }

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );

        Vector3 position = startingPosition;
        position.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.TryGetComponent(out PlayerController _))
        {
            return;
        }

        if (levelManager == null)
        {
            return;
        }

        collected = true;
        levelManager.CollectTreasure();
        gameObject.SetActive(false);
    }
}