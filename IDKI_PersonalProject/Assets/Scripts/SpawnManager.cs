using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform player;           // assign
    [SerializeField] private ScoreManager scoreManager;  // assign
    [SerializeField] private BossHealthBar bossHealthBar;// assign (UI)
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject boss;

    [Header("Spawning")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnDistance = 3f;
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private int bossScoreThreshold = 10;

    private float timer;
    private bool bossSpawned;
    private bool stopRegularSpawns;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!bossSpawned && scoreManager != null && scoreManager.Score >= bossScoreThreshold)
        {
            stopRegularSpawns = true;
            SpawnBoss();
            bossSpawned = true;
        }

        if (stopRegularSpawns) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy(spawnAmount);
        }
    }

    void SpawnEnemy(int amount)
    {
        if (enemies == null || enemies.Length == 0 || cam == null) return;

        var height = cam.orthographicSize * 2f;
        var width  = height * cam.aspect;
        var camPos = cam.transform.position;

        float left   = camPos.x - width  * 0.5f;
        float right  = camPos.x + width  * 0.5f;
        float top    = camPos.z - height * 0.5f;
        float bottom = camPos.z + height * 0.5f;

        for (int i = 0; i < amount; i++)
        {
            int edge = Random.Range(0, 4);
            Vector3 spawnPos = Vector3.zero;

            switch (edge)
            {
                case 0: spawnPos.x = left  - spawnDistance; spawnPos.z = Random.Range(top, bottom);  break; // left
                case 1: spawnPos.x = right + spawnDistance; spawnPos.z = Random.Range(top, bottom);  break; // right
                case 2: spawnPos.z = top   - spawnDistance; spawnPos.x = Random.Range(left, right);  break; // top
                case 3: spawnPos.z = bottom+ spawnDistance; spawnPos.x = Random.Range(left, right);  break; // bottom
            }

            spawnPos.y = 0f;
            var prefab = enemies[Random.Range(0, enemies.Length)];
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnBoss()
    {
        if (!boss) return;

        Vector3 spawnPos = player != null
            ? new Vector3(player.position.x + 10f, 0f, player.position.z + 10f)
            : new Vector3(10f, 0f, 10f);

        var go = Instantiate(boss, spawnPos, Quaternion.identity);

        // Bind boss health bar
        if (bossHealthBar != null && go.TryGetComponent<Health>(out var bossHealth))
        {
            bossHealthBar.Bind(bossHealth);
        }
    }
}
