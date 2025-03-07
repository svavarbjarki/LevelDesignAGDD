﻿using System;
using UnityEngine;

namespace AGDDPlatformer
{
    public class PlayerController : KinematicObject
    {
        [Header("Movement")]
        public float maxSpeed = 7;
        public float jumpSpeed = 7;
        public float jumpDeceleration = 0.5f; // Upwards slow after releasing jump button
        public float cayoteTime = 0.1f; // Lets player jump just after leaving ground
        public float jumpBufferTime = 0.1f; // Lets the player input a jump just before becoming grounded
        public bool reversedGravity = false;
        public bool groundedReverse = false;
        public bool inGravitySwitch = false;
        Vector2 windForce;

        [Header("Dash")]
        public float dashSpeed;
        public float dashTime;
        public float dashCooldown;
        public Color canDashColor;
        public Color cantDashColor;
        float lastDashTime;
        Vector2 dashDirection;
        bool isDashing;
        bool canDash;
        bool wantsToDash;

        [Header("Audio")]
        public AudioSource source;
        public AudioClip jumpSound;
        public AudioClip dashSound;

        public Vector2 startPosition;
        public bool startOrientation;

        float lastJumpTime;
        float lastGroundedTime;
        bool canJump;
        bool jumpReleased;
        public Vector2 move;
        public float defaultGravityModifier;

        SpriteRenderer spriteRenderer;

        Vector2 jumpBoost;

        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            lastJumpTime = -jumpBufferTime * 2;

            startPosition = transform.position;
            startOrientation = spriteRenderer.flipX;

            defaultGravityModifier = gravityModifier;
        }

        void Update()
        {
            getInputs();

            isFrozen = GameManager.instance.timeStopped;

            if (reversedGravity && !groundedReverse)
            {
                if (isGrounded)
                {
                    groundedReverse = true;
                    Debug.Log("Reverse X direction");
                }
            }
            // Wait until reaching ceiling
            if (groundedReverse)
            {
                move.x *= -1;
            }

            // Wait until reaching ground
            if (!reversedGravity && isGrounded && groundedReverse)
            {
                groundedReverse = false;
                Debug.Log("Reverse X direction Back");
            }

            // Clamp directional input to 8 directions for dash
            Vector2 desiredDashDirection = new Vector2(
                move.x == 0 ? 0 : (move.x > 0 ? 1 : -1),
                move.y == 0 ? 0 : (move.y > 0 ? 1 : -1));

            if (desiredDashDirection == Vector2.zero)
            {
                // Dash in facing direction if there is no directional input;
                desiredDashDirection = spriteRenderer.flipX ? -Vector2.right : Vector2.right;
            }
            desiredDashDirection = desiredDashDirection.normalized;

            /* --- Compute Velocity --- */

            if (canDash && wantsToDash)
            {
                isDashing = true;
                dashDirection = desiredDashDirection;
                lastDashTime = Time.time;
                canDash = false;
                gravityModifier = 0;

                source.PlayOneShot(dashSound);
            }
            wantsToDash = false;

            if (isDashing)
            {
                dashAction();
            }
            else
            {
                if (isGrounded)
                {
                    // Store grounded time to allow for late jumps
                    lastGroundedTime = Time.time;
                    canJump = true;
                    if (!isDashing && Time.time - lastDashTime >= dashCooldown)
                        canDash = true;
                }

                // Check time for buffered jumps and late jumps
                float timeSinceJumpInput = Time.time - lastJumpTime;
                float timeSinceLastGrounded = Time.time - lastGroundedTime;

                if (canJump && timeSinceJumpInput <= jumpBufferTime && timeSinceLastGrounded <= cayoteTime)
                {
                    velocity.y = Mathf.Sign(gravityModifier) * jumpSpeed;
                    canJump = false;
                    isGrounded = false;

                    source.PlayOneShot(jumpSound);
                }
                else if (jumpReleased)
                {
                    // Decelerate upwards velocity when jump button is released
                    if ((gravityModifier >= 0 && velocity.y > 0) ||
                        (gravityModifier < 0 && velocity.y < 0))
                    {
                        velocity.y *= jumpDeceleration;
                    }
                    jumpReleased = false;
                }

                //velocity.x = move.x * maxSpeed;
                velocity.x = move.x * maxSpeed + windForce.x;
                // If the wind pushes upwards, counteract gravity
                if (windForce.y > 0 && velocity.y < windForce.y)
                {
                    velocity.y = windForce.y; // Override gravity for upward wind
                }
                else
                {
                    velocity.y += windForce.y * Time.deltaTime; // Apply wind gradually
                }

                if (isGrounded || (velocity + jumpBoost).magnitude < velocity.magnitude)
                {
                    jumpBoost = Vector2.zero;
                }
                else
                {
                    velocity += jumpBoost;
                    jumpBoost -= jumpBoost * Mathf.Min(1f, Time.deltaTime);
                }
            }

            updateSprite();
        }

        public void ResetPlayer()
        {
            transform.position = startPosition;
            spriteRenderer.flipX = startOrientation;

            lastJumpTime = -jumpBufferTime * 2;

            velocity = Vector2.zero;
            gravityModifier = 1;
            defaultGravityModifier = gravityModifier;
            inGravitySwitch = false;
            reversedGravity = false;
            if (spriteRenderer.flipY)
            {
                spriteRenderer.flipY = !spriteRenderer.flipY;
            }
        }

        public void ResetDash()
        {
            canDash = true;
        }

        //Add a short mid-air boost to the player (unrelated to dash). Will be reset upon landing.
        public void SetJumpBoost(Vector2 jumpBoost)
        {
            this.jumpBoost = jumpBoost;
        }

        public void SetWindForce(Vector2 force)
        {
            windForce = force;
        }

        public void ApplyPlatformVelocity(Vector2 platformVelocity)
        {

            this.velocity += platformVelocity;
        }

        private void dashAction()
        {
            velocity = dashDirection * dashSpeed;
            if (Time.time - lastDashTime >= dashTime)
            {
                isDashing = false;

                gravityModifier = defaultGravityModifier;
                if ((gravityModifier >= 0 && velocity.y > 0) ||
                    (gravityModifier < 0 && velocity.y < 0))
                {
                    velocity.y *= jumpDeceleration;
                }
            }
        }

        #region  Input Capture
        private void getInputs()
        {
            movementInput();
            jumpInput();
            dashInput();
        }

        private void movementInput()
        {
            move.x = Input.GetAxisRaw("Horizontal");
            move.y = Input.GetAxisRaw("Vertical");
        }

        private void jumpInput()
        {
            if (Input.GetButtonDown("Jump"))
            {
                lastJumpTime = Time.time;
            }
            if (Input.GetButtonUp("Jump"))
            {
                jumpReleased = true;
            }
        }

        private void dashInput()
        {
            if (Input.GetButtonDown("Dash"))
            {
                wantsToDash = true;
            }
        }

        #endregion

        private void updateSprite()
        {
            /* --- Adjust Sprite --- */

            // Assume the sprite is facing right, flip it if moving left
            if (move.x * gravityModifier > 0.01f && !inGravitySwitch)
            {
                spriteRenderer.flipX = false;
            }
            else if (move.x * gravityModifier < -0.01f && !inGravitySwitch)
            {
                spriteRenderer.flipX = true;
            }

            spriteRenderer.color = canDash ? canDashColor : cantDashColor;
        }

    }
}
