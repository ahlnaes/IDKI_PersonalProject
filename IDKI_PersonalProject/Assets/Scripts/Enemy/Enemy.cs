using System;
using UnityEngine;

// simple enemy class
public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private float damageValue = 1f;
    private Animation spawnAnim;
    
    public float Damage => damageValue;

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
            PowerUpSpawner.Instance?.TrySpawn(transform.position);
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
