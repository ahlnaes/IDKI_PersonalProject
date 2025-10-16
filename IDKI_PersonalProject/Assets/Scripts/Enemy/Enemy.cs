using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int scoreValue = 1;
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
            ScoreManager.Instance?.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(nameof(Projectile)))
        {
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            currentHealth -= projectile.Damage;
            Destroy(other.gameObject);
        }
    }
}
