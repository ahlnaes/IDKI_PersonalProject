using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public static PowerUpSpawner Instance { get; private set; }

    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField, Range(0f,1f)] private float dropChance = 0.20f; // 20%

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TrySpawn(Vector3 position)
    {
        if (Random.value > dropChance) return;

        position.y = 0f;
        Instantiate(powerUpPrefab, position, Quaternion.identity);
    }
}