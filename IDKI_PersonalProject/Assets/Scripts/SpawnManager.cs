using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject boss;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnDistance; // distance from the edge of the camera view
    [SerializeField] private int spawnAmount;

    private float timer;
    private float elapsed;
    private bool bossSpawned;

    void Start()
    {
        if(!cam) cam = Camera.main;
        SpawnBoss();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        // if (timer >= spawnInterval)
        // {
        //     spawnAmount = GetCurrentMinute();
        //     timer = 0;
        //     SpawnEnemy(spawnAmount);
        // }
    }

    void SpawnEnemy(int amount)
    {
        if (enemies.Length == 0) return;
        // get what visible
        var height = cam.orthographicSize * 2f;
        var width = height * cam.aspect;
        var camPos = cam.transform.position;
        
        //corners of the view area
        var left = camPos.x - width / 2;
        var right = camPos.x + width / 2;
        var top = camPos.z - height / 2f;
        var bottom = camPos.z + height / 2f;

        for (var i = 0; i < amount; i++)
        {
            var edge = Random.Range(0, 4);
            var spawnPos = Vector3.zero;
            switch (edge)
            {
                case 0: // left
                    spawnPos.x = left - spawnDistance;
                    spawnPos.z = Random.Range(bottom, top);
                    break;
                case 1: // right
                    spawnPos.x = right + spawnDistance;
                    spawnPos.z = Random.Range(bottom, top);
                    break;
                case 2: // top
                    spawnPos.z = top + spawnDistance;
                    spawnPos.x = Random.Range(left, right);
                    break;
                case 3: // bottom
                    spawnPos.z = bottom - spawnDistance;
                    spawnPos.x = Random.Range(left, right);
                    break;
            }

            spawnPos.y = 3f;
            var prefab = enemies[Random.Range(0, enemies.Length)];
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }

    }

    void SpawnBoss()
    {
        Instantiate(boss, new Vector3(10,10,10), Quaternion.identity);
    }

    private int GetCurrentMinute()
    {
        return Mathf.FloorToInt((Time.time / 60) + 1);
    }
}
