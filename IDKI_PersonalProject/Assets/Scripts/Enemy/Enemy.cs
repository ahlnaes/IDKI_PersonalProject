using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Health/Score")]
    [SerializeField] protected Health health; 
    [SerializeField] protected int scoreValue = 10;

    public bool IsDead => health.IsDead;

    public System.Action<Enemy> OnDied;
    public System.Action<float> OnDamaged;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        
        health.OnChanged += (cur, max) => OnDamaged?.Invoke(cur);
        health.OnDied    += Die;
    }

    protected virtual void OnDestroy()
    {
        if (!health) return;
        health.OnDied -= Die;
    }

    protected virtual void Update()
    {
        if (IsDead) return;
        Tick();
    }

    protected abstract void Tick();

    // IDamageable
    public virtual void TakeDamage(float amount) => health?.TakeDamage(amount);

    protected virtual void Die()
    {
        OnDied?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        var proj = other.GetComponent<Projectile>();
        if (proj)
        {
            TakeDamage(proj.Damage);    
            Destroy(other.gameObject);
        }
    }
}