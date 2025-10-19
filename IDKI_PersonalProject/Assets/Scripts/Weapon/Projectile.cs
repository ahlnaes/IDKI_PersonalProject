using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private float speed = 60f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private bool useGravity;

    [SerializeField] private ParticleSystem hitParticles;
    
    public float Damage => damage;
    
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
        var contact = collision.contacts[0];
        var point = contact.point;
        var normal = contact.normal;
        HitVFX(point + normal * 0.02f, normal);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            // //used to position the "splash" fx from the projectile
            // var point = other.ClosestPoint(transform.position);
            //
            // var incoming = (rb)
            //     ? rb.linearVelocity.normalized
            //     : transform.forward;
            //
            // var normal = -incoming; // reversed direction
            //
            // HitVFX(point + normal * 0.02f, normal);
            // Destroy(gameObject);
            Destroy(gameObject);
        }
    }
    
    private void HitVFX(Vector3 pos, Vector3 normal)
    {
        if (!hitParticles) return;
        
        var rotation = Quaternion.LookRotation(pos, normal);
        var vfx = Instantiate(hitParticles, pos, rotation);
        var main = vfx.main;
        Destroy(vfx.gameObject, main.duration + main.startLifetime.constantMax);
    }
    
}
