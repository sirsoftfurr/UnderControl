using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;

    [Header("Movement")]
    public float speed = 5f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpTime = 0.2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public Vector2 groundCheckSize = new Vector2(0.2f, 0.2f);

    private float jumpTimeCounter;
    private bool isJumping;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //------------------------------------------------
        // MOVEMENT
        //------------------------------------------------

        float moveInput = Input.GetAxisRaw("Horizontal");

        body.linearVelocity = new Vector2(
            moveInput * speed,
            body.linearVelocity.y
        );

        //------------------------------------------------
        // CHARACTER FLIP (RIGGED CHARACTER SAFE)
        //------------------------------------------------

        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;

            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput);

            transform.localScale = scale;
        }

        //------------------------------------------------
        // START JUMP
        //------------------------------------------------

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isJumping = true;

            jumpTimeCounter = jumpTime;

            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpForce
            );
        }

        //------------------------------------------------
        // CONTINUE JUMP
        //------------------------------------------------

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

        //------------------------------------------------
        // STOP JUMP EARLY
        //------------------------------------------------

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundMask
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(
            groundCheck.position,
            groundCheckSize
        );
    }
}


