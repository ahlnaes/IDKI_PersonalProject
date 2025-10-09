using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    private WeaponConfig config;
    private CooldownTimer cooldown;
    private Transform source;

    void Awake()
    {
        cooldown = new CooldownTimer();
    }

    void Update()
    {
        cooldown.Tick(Time.deltaTime);
    }
    public bool CanFire()
    {
        if (!config || !source || !projectile) return false;
        return !cooldown.IsActive;
    }

    public void Fire()
    {
        
    }
}
