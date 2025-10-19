using UnityEngine;
using Player.Movement;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    [Header("Heal")]
    [SerializeField] private float healFraction = 0.5f; // 50% of max

    [Header("Speed Buff")]
    [SerializeField] private float speedMultiplier = 2f; // +100%
    [SerializeField] private float buffDuration   = 6f;  // seconds

    private bool consumed;

    private void OnTriggerEnter(Collider other)
    {
        if (consumed || !other.CompareTag(playerTag)) return;
        if (!other.TryGetComponent<PlayerController>(out var player)) return;

        if (Random.value < 0.5f)
        {
            // Heal 50% max
            player.Heal(player.MaxHealth * healFraction);
        }
        else
        {
            player.ApplySpeedBuff(speedMultiplier, buffDuration);
        }

        consumed = true;
        Destroy(gameObject);
    }
}