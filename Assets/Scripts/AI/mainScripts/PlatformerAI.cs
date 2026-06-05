using System.Collections.Generic;
using UnityEngine;

public class PlatformerEnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Graphics")]
    // Drag rigged graphics object here
    public Transform graphics;
    
    [Header("Footsteps")]
    public AudioSource footstepSource;

    public AudioClip[] footstepSounds;

    public float footstepInterval = 0.4f;

    private float footstepTimer;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector3 originalScale;

    // =========================================
    // PATHFINDING
    // =========================================

    private List<Node> path =
        new List<Node>();

    private int pathIndex = 0;

    private float pathUpdateTimer = 0f;

    public float pathUpdateInterval = 0.5f;

    // =========================================
    // START
    // =========================================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (graphics != null)
        {
            anim = graphics.GetComponent<Animator>();
            originalScale = graphics.localScale;
        }

        // Find player automatically
        GameObject p =
            GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    // =========================================
    // UPDATE
    // =========================================

    void Update()
    {
        player = LatchScript.currentTarget;
        
        // Always target current possession target
        if (LatchScript.currentTarget != null)
        {
            player = LatchScript.currentTarget;
        }

        pathUpdateTimer += Time.deltaTime;

        if (pathUpdateTimer >= pathUpdateInterval)
        {
            UpdatePath();

            pathUpdateTimer = 0f;
        }

        FollowPath();
        HandleFootsteps();

        UpdateAnimations();
    }

    // ==================================================
    // PATHFINDING
    // ==================================================

    void UpdatePath()
    {
        if (player == null)
            return;

        Node start =
            GetClosestNode(transform.position);

        Node target =
            GetClosestNode(player.position);

        if (start != null && target != null)
        {
            path = FindPath(start, target);

            pathIndex = 0;
        }
    }

    List<Node> FindPath(Node start, Node target)
    {
        List<Node> openSet =
            new List<Node>();

        HashSet<Node> closedSet =
            new HashSet<Node>();

        Dictionary<Node, Node> cameFrom =
            new Dictionary<Node, Node>();

        Dictionary<Node, float> gScore =
            new Dictionary<Node, float>();

        openSet.Add(start);

        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            foreach (Node node in openSet)
            {
                if (
                    GetScore(node, target, gScore)
                    <
                    GetScore(current, target, gScore)
                )
                {
                    current = node;
                }
            }

            if (current == target)
            {
                return RetracePath(
                    cameFrom,
                    current
                );
            }

            openSet.Remove(current);

            closedSet.Add(current);

            foreach (Node neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeG =
                    gScore[current]
                    +
                    Vector2.Distance(
                        current.transform.position,
                        neighbor.transform.position
                    );

                if (
                    !gScore.ContainsKey(neighbor)
                    ||
                    tentativeG < gScore[neighbor]
                )
                {
                    cameFrom[neighbor] = current;

                    gScore[neighbor] = tentativeG;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return new List<Node>();
    }

    float GetScore(
        Node node,
        Node target,
        Dictionary<Node, float> gScore
    )
    {
        float g =
            gScore.ContainsKey(node)
            ?
            gScore[node]
            :
            Mathf.Infinity;

        float h =
            Vector2.Distance(
                node.transform.position,
                target.transform.position
            );

        return g + h;
    }

    List<Node> RetracePath(
        Dictionary<Node, Node> cameFrom,
        Node current
    )
    {
        List<Node> result =
            new List<Node>();

        while (cameFrom.ContainsKey(current))
        {
            result.Add(current);

            current = cameFrom[current];
        }

        result.Reverse();

        return result;
    }

    Node GetClosestNode(Vector2 position)
    {
        Node[] nodes =
            FindObjectsOfType<Node>();

        Node closest = null;

        float minDist = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float dist =
                Vector2.Distance(
                    position,
                    node.transform.position
                );

            if (dist < minDist)
            {
                minDist = dist;

                closest = node;
            }
        }

        return closest;
    }

    // ==================================================
    // MOVEMENT
    // ==================================================

    void FollowPath()
    {
        if (
            path == null
            ||
            path.Count == 0
            ||
            pathIndex >= path.Count
        )
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );

            return;
        }

        Node targetNode = path[pathIndex];

        Vector2 direction =
            targetNode.transform.position -
            transform.position;

        float move =
            Mathf.Sign(direction.x);

        rb.linearVelocity = new Vector2(
            move * moveSpeed,
            rb.linearVelocity.y
        );

        // =====================================
        // FLIP GRAPHICS
        // =====================================

        if (graphics != null)
        {
            // Moving RIGHT
            if (move > 0)
            {
                graphics.localScale =
                    new Vector3(
                        -Mathf.Abs(originalScale.x),
                        originalScale.y,
                        originalScale.z
                    );
            }

            // Moving LEFT
            else if (move < 0)
            {
                graphics.localScale =
                    new Vector3(
                        Mathf.Abs(originalScale.x),
                        originalScale.y,
                        originalScale.z
                    );
            }
        }

        // =====================================
        // JUMP
        // =====================================

        if (
            direction.y > 1f
            &&
            IsGrounded()
        )
        {
            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );
        }

        // =====================================
        // NEXT NODE
        // =====================================

        if (
            Vector2.Distance(
                transform.position,
                targetNode.transform.position
            ) < 0.3f
        )
        {
            pathIndex++;
        }
    }

    // ==================================================
    // GROUND CHECK
    // ==================================================

    bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    // ==================================================
    // ANIMATIONS
    // ==================================================

    void UpdateAnimations()
    {
        if (anim == null)
            return;

        // Walking float
        anim.SetFloat(
            "Speed",
            Mathf.Abs(rb.linearVelocity.x)
        );

        // Jumping bool
        anim.SetBool(
            "isJumping",
            !IsGrounded()
        );
    }
    
    void HandleFootsteps()
    {
        // Not moving
        if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)
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
            UnityEngine.Random.Range(
                0.95f,
                1.05f
            );

        footstepSource.PlayOneShot(clip);
    }

    // ==================================================
    // GIZMOS
    // ==================================================

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}

