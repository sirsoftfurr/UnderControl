using System.Collections.Generic;
using UnityEngine;

public class UpgradedAI : MonoBehaviour
{
     public Transform player;
    public float moveSpeed = 3f;
    public float jumpForce = 8f;

    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    public Transform groundCheck;
    public float groundRadius = 0.2f;

    private Rigidbody2D rb;

    private List<GridNode> path = new List<GridNode>();
    private int pathIndex = 0;

    private float updateTimer;
    public float updateInterval = 0.4f;

    private GridGenerator grid;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grid = FindObjectOfType<GridGenerator>();
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer > updateInterval)
        {
            RecalculatePath();
            updateTimer = 0f;
        }

        Move();
    }

    // ---------------- SMART PATH ----------------

    void RecalculatePath()
    {
        // If direct path exists → skip A*
        if (HasLineOfSight())
        {
            path.Clear();
            return;
        }

        GridNode start = GetClosestNode(transform.position);
        GridNode target = GetClosestNode(player.position);

        path = FindPath(start, target);
        pathIndex = 0;
    }

    bool HasLineOfSight()
    {
        Vector2 dir = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, obstacleLayer);

        return hit.collider == null;
    }

    // ---------------- A* ----------------

    List<GridNode> FindPath(GridNode start, GridNode target)
    {
        List<GridNode> open = new List<GridNode>();
        HashSet<GridNode> closed = new HashSet<GridNode>();

        Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>();
        Dictionary<GridNode, float> gScore = new Dictionary<GridNode, float>();

        open.Add(start);
        gScore[start] = 0;

        while (open.Count > 0)
        {
            GridNode current = open[0];

            foreach (var n in open)
            {
                if (Score(n, target, gScore) < Score(current, target, gScore))
                    current = n;
            }

            if (current == target)
                return Retrace(cameFrom, current);

            open.Remove(current);
            closed.Add(current);

            foreach (var neighbor in current.neighbors)
            {
                if (closed.Contains(neighbor)) continue;

                float newCost = gScore[current] + Vector2.Distance(current.position, neighbor.position);

                if (!gScore.ContainsKey(neighbor) || newCost < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = newCost;

                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
        }

        return new List<GridNode>();
    }

    float Score(GridNode n, GridNode target, Dictionary<GridNode, float> gScore)
    {
        float g = gScore.ContainsKey(n) ? gScore[n] : Mathf.Infinity;
        float h = Vector2.Distance(n.position, target.position);
        return g + h;
    }

    List<GridNode> Retrace(Dictionary<GridNode, GridNode> cameFrom, GridNode current)
    {
        List<GridNode> result = new List<GridNode>();

        while (cameFrom.ContainsKey(current))
        {
            result.Add(current);
            current = cameFrom[current];
        }

        result.Reverse();
        return result;
    }

    GridNode GetClosestNode(Vector2 pos)
    {
        GridNode closest = null;
        float dist = Mathf.Infinity;

        foreach (var node in grid.nodes)
        {
            float d = Vector2.Distance(pos, node.position);
            if (d < dist)
            {
                dist = d;
                closest = node;
            }
        }

        return closest;
    }

    // ---------------- MOVEMENT ----------------

    void Move()
    {
        // Direct chase if visible
        if (path.Count == 0)
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            return;
        }

        if (pathIndex >= path.Count) return;

        GridNode target = path[pathIndex];
        Vector2 movedir = target.position - (Vector2)transform.position;

        // Horizontal move
        rb.linearVelocity = new Vector2(Mathf.Sign(movedir.x) * moveSpeed, rb.linearVelocity.y);

        // Smart jump
        if (movedir.y > 0.5f && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Vector2.Distance(transform.position, target.position) < 0.3f)
        {
            pathIndex++;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }
}

