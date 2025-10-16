using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private float speed = 60f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1f;
    [SerializeField] private bool useGravity = false;
    private AudioSource audioSource;
    
    private Rigidbody rb;
    private float life, maxLife;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Launch(Vector3 dir)
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
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

    private void OnTriggerEnter(Collider other)
    {
    }
}
