using UnityEngine;
using Random = System.Random;

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

    public Vector2 groundCheckSize =
        new Vector2(0.2f, 0.2f);

    [Header("Graphics")]
    // Drag your rigged graphics object here
    public Transform graphics;

    // If true, face mouse instead of movement
    [HideInInspector]
    public bool faceMouse = false;
    
    [Header("Footsteps")]
    public AudioSource footstepSource;

    public AudioClip[] footstepSounds;

    public float footstepInterval = 0.4f;

    private float footstepTimer;

    private Vector3 originalScale;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        if (graphics != null)
        {
            anim = graphics.GetComponent<Animator>();
            originalScale = graphics.localScale;
        }
    }

    void Update()
    {
        float moveInput =
            Input.GetAxis("Horizontal");

        // =====================================
        // MOVEMENT
        // =====================================

        body.linearVelocity = new Vector2(
            moveInput * speed,
            body.linearVelocity.y
        );
        
        HandleFootsteps();

        // =====================================
        // FLIP CHARACTER
        // =====================================

        if (graphics != null)
        {
            // =================================
            // FACE MOUSE
            // =================================

            if (faceMouse)
            {
                Vector3 mousePos =
                    Camera.main.ScreenToWorldPoint(
                        Input.mousePosition
                    );

                mousePos.z = 0f;

                // Mouse RIGHT
                if (mousePos.x > transform.position.x)
                {
                    graphics.localScale = new Vector3(
                        -Mathf.Abs(originalScale.x),
                        originalScale.y,
                        originalScale.z
                    );
                }

                // Mouse LEFT
                else
                {
                    graphics.localScale = new Vector3(
                        Mathf.Abs(originalScale.x),
                        originalScale.y,
                        originalScale.z
                    );
                }
            }

            // =================================
            // FACE MOVEMENT
            // =================================

            else
            {
                // Moving RIGHT
                if (moveInput > 0)
                {
                    graphics.localScale = new Vector3(
                        Mathf.Abs(originalScale.x),
                        originalScale.y,
                        originalScale.z
                    );
                }

                // Moving LEFT
                else if (moveInput < 0)
                {
                    graphics.localScale = new Vector3(
                        -Mathf.Abs(originalScale.x),
                        originalScale.y,
                        originalScale.z
                    );
                }
            }
        }

        // =====================================
        // START JUMP
        // =====================================

        if (Input.GetButtonDown("Jump") &&
            IsGrounded())
        {
            isJumping = true;

            jumpTimeCounter = jumpTime;

            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpForce
            );
        }

        // =====================================
        // HOLD JUMP
        // =====================================

        if (Input.GetButton("Jump") &&
            isJumping)
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

        // =====================================
        // RELEASE JUMP
        // =====================================

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // =====================================
        // ANIMATIONS
        // =====================================

        if (anim != null)
        {
            // Movement float
            anim.SetFloat(
                "Speed",
                Mathf.Abs(moveInput)
            );

            // Jump bool
            anim.SetBool(
                "isJumping",
                !IsGrounded()
            );
        }
    }

    // =====================================
    // GROUND CHECK
    // =====================================

    bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundMask
        );
    }
    
    void HandleFootsteps()
    {
        // Not moving
        if (Mathf.Abs(body.linearVelocity.x) < 0.1f)
            return;

        // In air
        if (!IsGrounded())
            return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            if (!footstepSource.isPlaying)
            {
                PlayFootstep();
            }

            footstepTimer =
                footstepInterval;
        }
    }
    
    void PlayFootstep()
    {
        if (footstepSource == null)
            return;

        if (footstepSounds.Length == 0)
            return;

        AudioClip clip =
            footstepSounds[
                UnityEngine.Random.Range(
                    0,
                    footstepSounds.Length
                )
            ];

        footstepSource.pitch =
            UnityEngine.Random.Range(0.95f, 1.05f);

        footstepSource.PlayOneShot(clip);
    }
    

    // =====================================
    // GIZMOS
    // =====================================

    void OnDrawGizmosSelected()
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



