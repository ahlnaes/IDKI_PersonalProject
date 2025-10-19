using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    
    public float Max => maxHealth;
    public float Current => currentHealth;
    public bool IsDead => currentHealth <= 0;

    //Actions allow other things to subscribe to events 
    public System.Action<float, float> OnChanged;
    public System.Action OnDied;

    private void Awake()
    {
        currentHealth = Mathf.Max(1f, maxHealth); // just to check that it isn't set to lower than 1
        OnChanged?.Invoke(currentHealth, maxHealth); 
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        OnChanged?.Invoke(currentHealth, maxHealth); //check if onChanged has atleast 1 sub and notify
        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDied?.Invoke();
    }
}
