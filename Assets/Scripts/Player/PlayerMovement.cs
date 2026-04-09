using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;

    public float speed;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public LayerMask groundMask;
    public Vector2 groundCheckSize = new Vector2(0.2f, 0.2f);

    public float jumpTime = 0.2f;      // how long you can hold jump
    private float jumpTimeCounter;
    private bool isJumping;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movement
        body.linearVelocity = new Vector2(
            Input.GetAxis("Horizontal") * speed,
            body.linearVelocity.y
        );

        // Start jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        }

        // Continue jump while holding
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Stop jump early when released
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundMask);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}


