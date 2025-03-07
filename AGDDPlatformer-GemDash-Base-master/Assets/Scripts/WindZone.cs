﻿using UnityEngine;
using AGDDPlatformer; // Add this to access PlayerController

public class WindZone : MonoBehaviour
{
    public Vector2 windForce = new Vector2(5f, 0f); // Default: pushes right
    private bool isBlocked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && isBlocked == false)
            {
                player.SetWindForce(windForce);
            }
        }
        if (other.CompareTag("Platform"))
        {
            setBlocked(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetWindForce(Vector2.zero); // Remove wind effect
            }
        }
        if (other.CompareTag("Platform"))
        {
            setBlocked(false);
        }
    }

    private void setBlocked(bool blocked)
    {
        isBlocked = blocked;
    }
}
