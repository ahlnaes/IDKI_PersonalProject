using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private string targetTag = "Player"; // or "Enemy"

    private void OnCollisionEnter(Collision other)
    {
        TryDamage(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other.gameObject);
    }

    private void TryDamage(GameObject go)
    {
        if (!string.IsNullOrEmpty(targetTag) && !go.CompareTag(targetTag)) return;
        if (go.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
        }
    }
}