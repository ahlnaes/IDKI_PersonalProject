using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private int spawnAmount;

    private float timer;
    private Transform player;

    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (DebugVariables.Instance.enemySpawnOn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                spawnAmount = GetCurrentMinute();
                timer = 0;
                SpawnEnemy(spawnAmount);
            }
        }
    }

    void SpawnEnemy(int amount)
    {
        if (enemies.Length == 0 || player == null) return;

        var center = player.position;

        for (var i = 0; i < amount; i++)
        {
            var angle = Random.Range(0f, Mathf.PI * 2f);
            var spawnPos = new Vector3(
                center.x + Mathf.Cos(angle) * spawnRadius,
                3f,
                center.z + Mathf.Sin(angle) * spawnRadius
            );

            var prefab = enemies[Random.Range(0, enemies.Length)];
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    private int GetCurrentMinute()
    {
        return Mathf.FloorToInt((Time.time / 60) + 1);
    }
}
