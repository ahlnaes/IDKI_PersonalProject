using UnityEngine;
using Player.Movement;

public class ChaserEnemy : Enemy
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float contactDamage = 1f;
    [SerializeField] private float attackCooldown = 0.7f;

    private float attackTimer;

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.GetComponent<PlayerController>();
    }

    protected override void Tick()
    {
        if (!player) return;

        var to = player.transform.position - transform.position;
        to.y = 0f;
        var dist = to.magnitude;
        if (dist <= 0.01f) return;

        var dir = to / dist;
        transform.forward = dir;

        // mvmt
        if (dist > attackRadius)
            transform.position += dir * (moveSpeed * Time.deltaTime);

        // atack
        attackTimer -= Time.deltaTime;
        if (dist <= attackRadius && attackTimer <= 0f)
        {
            player.TakeDamage(contactDamage);
            attackTimer = attackCooldown;
        }
    }
}