using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float currentHealth;
    private Animation spawnAnim;

    private void Awake()
    {
        spawnAnim = GetComponent<Animation>();
    }

    private void Start()
    {
        spawnAnim.Play("enemy_spawn");
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(nameof(Projectile)))
        {
            currentHealth--;
            Destroy(other.gameObject);
        }
    }
}
