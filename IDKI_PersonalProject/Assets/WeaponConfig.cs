using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Scriptable Objects/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public float fireRate;
    public float velocity;
    public float damage;
    public float range;
    public float gravity;
    public float drag;
    public LayerMask layerMask;
}
