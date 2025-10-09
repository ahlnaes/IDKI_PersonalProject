using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private float speed = 60f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 10f;
    [SerializeField] private bool useGravity = false;
    
    private Rigidbody rb;
    private float life, maxLife;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Launch(Vector3 dir)
    {
        dir.y = 0f;
        rb.linearVelocity = dir.normalized * speed;
        life = 0f;
        maxLife = range;
    }

    private void FixedUpdate()
    {
        life  += Time.fixedDeltaTime;
        if(life >= maxLife) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
