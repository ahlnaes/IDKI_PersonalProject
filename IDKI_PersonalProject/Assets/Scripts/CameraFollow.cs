using UnityEngine;

//script that makes camera follow player around
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }
}
