using System;
using UnityEngine;

//make the enemy move towards the player
public class EnemyFollowPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
