using Player;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    [SerializeField] private Transform source;
    [SerializeField] private WeaponConfig config;
    
    private CooldownTimer cooldown;

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
        if (!config || !source) return false;
        return !cooldown.IsActive;
    }

    /// <summary>
    /// Vector3 target should be world-space so I convert before calling
    /// </summary>
    /// <param name="target"></param>
    public void Fire(Vector3 target)
    {
        if(!CanFire()) return;
        if (target == Vector3.zero) target = source.forward;

        var projectile = Instantiate(config.projectile, source.position, Quaternion.LookRotation(target));
        projectile.Launch(target);
        cooldown.Start(1f/config.fireRate);

    }
    
    public Vector3 SourcePosition => source.position;
}
