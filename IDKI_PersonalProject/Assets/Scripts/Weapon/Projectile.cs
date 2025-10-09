using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;

    private void Setup(Vector3 )
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce();
    }
}
