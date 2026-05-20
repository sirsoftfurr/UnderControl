using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;

    [Header("Movement")]
    public float speed = 5f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpTime = 0.2f;

    private float jumpTimeCounter;
    private bool isJumping;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public Vector2 groundCheckSize = new Vector2(0.2f, 0.2f);

    [Header("Graphics")]
    // Drag your rigged character object here
    public Transform graphics;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        // Gets Animator from graphics object
        anim = graphics.GetComponent<Animator>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // =========================
        // MOVEMENT
        // =========================

        body.linearVelocity = new Vector2(
            moveInput * speed,
            body.linearVelocity.y
        );

        // =========================
        // FLIP CHARACTER
        // =========================

        if (moveInput > 0)
        {
            graphics.localScale = new Vector3(0.14f,0.14f, 0.14f);
        }
        else if (moveInput < 0)
        {
            graphics.localScale = new Vector3(-0.14f, 0.14f, 0.14f);
        }

        // =========================
        // START JUMP
        // =========================

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;

            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpForce
            );
        }

        // =========================
        // HOLD JUMP
        // =========================

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                body.linearVelocity = new Vector2(
                    body.linearVelocity.x,
                    jumpForce
                );

                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // =========================
        // RELEASE JUMP
        // =========================

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // =========================
        // ANIMATIONS
        // =========================

        // Movement float
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // Jump bool
        anim.SetBool("isJumping", !IsGrounded());
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundMask
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(
            groundCheck.position,
            groundCheckSize
        );
    }
}


