using System;
using UnityEngine;

//make the enemy move towards the player
public class EnemyFollowPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    private Camera vrCamera;

    void Start()
    {
        vrCamera = Camera.main;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, vrCamera.transform.position, speed * Time.deltaTime);
    }
}
