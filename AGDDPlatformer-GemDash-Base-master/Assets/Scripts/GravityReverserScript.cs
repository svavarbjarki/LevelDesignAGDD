using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDDPlatformer;

public class GravityReverserScript : MonoBehaviour
{
    private bool isPlayerInside = false;
    private PlayerController player;
    private float lastGravitySwitchTime = -Mathf.Infinity; // Track last switch time
    public float gravitySwitchCooldown = 0.5f; // Cooldown time in seconds

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            isPlayerInside = true;
            player = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            isPlayerInside = false;
            player = null;
        }
    }

    void Update()
    {
        if (isPlayerInside && Time.time - lastGravitySwitchTime >= gravitySwitchCooldown)
        {
            Debug.Log("Gravity SWITCH");
            ReverseGravity();
            lastGravitySwitchTime = Time.time; // Update last switch time
        }
    }

    private void ReverseGravity()
    {
        if (player != null)
        {
            player.reversedGravity = !player.reversedGravity;
            player.inGravitySwitch = true;

            player.gravityModifier *= -1; // Flip gravity
            player.defaultGravityModifier *= -1;

            Transform spriteTransform = player.transform.Find("Square512");

            if (spriteTransform != null)
            {
                SpriteRenderer spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipY = !spriteRenderer.flipY;
                }
            }
            player.inGravitySwitch = false;
        }
    }
}