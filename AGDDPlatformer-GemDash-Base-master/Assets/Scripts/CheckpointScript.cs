using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDDPlatformer;

public class CheckpointScript : MonoBehaviour
{
    public Sprite activeSprite;
    public Sprite disabledSprite;
    private PlayerController player;
    private SpriteRenderer spriteRenderer;
    private bool isActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the default sprite to disabled at the start
        spriteRenderer.sprite = disabledSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") && !isActive) // Only activate once
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.startPosition = transform.position; // Set respawn point
                ActivateCheckpoint();
            }
        }
    }
    void ActivateCheckpoint()
    {
        isActive = true;
        spriteRenderer.sprite = activeSprite; // Change sprite to active
    }
}
