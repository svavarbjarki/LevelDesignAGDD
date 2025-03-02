using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDDPlatformer;

public class CheckpointScript : MonoBehaviour
{
    private PlayerController player;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            player = other.GetComponent<PlayerController>();
            player.startPosition = transform.position;
        }
    }
}
