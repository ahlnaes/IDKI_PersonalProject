using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Scriptable Objects/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [Tooltip("How many times per sec")] 
    public float fireRate = 5f;
    
    [Header("Projectile")]
    public Projectile projectile;
}
