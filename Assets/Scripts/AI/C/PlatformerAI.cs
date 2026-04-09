using System.Collections.Generic;
using UnityEngine;

public class PlatformerEnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float jumpForce = 7f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;

    private List<Node> path = new List<Node>();
    private int pathIndex = 0;

    private float pathUpdateTimer = 0f;
    public float pathUpdateInterval = 0.5f;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        
        
    }

    void Update()
    {
        FacePlayer(); // ✅ Always face player
        pathUpdateTimer += Time.deltaTime;

        if (pathUpdateTimer >= pathUpdateInterval)
        {
            UpdatePath();
            pathUpdateTimer = 0f;
        }

        FollowPath();
    }

    // ---------------- PATHFINDING ----------------

    void UpdatePath()
    {
        if (player == null || !player.gameObject)
            return;

        Node start = GetClosestNode(transform.position);
        Node target = GetClosestNode(player.position);

        if (start != null && target != null)
        {
            path = FindPath(start, target);
            pathIndex = 0;
        }
    }

    List<Node> FindPath(Node start, Node target)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();

        openSet.Add(start);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            foreach (var node in openSet)
            {
                if (GetScore(node, target, gScore) < GetScore(current, target, gScore))
                    current = node;
            }

            if (current == target)
                return RetracePath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = gScore[current] + Vector2.Distance(current.transform.position, neighbor.transform.position);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Node>();
    }

    float GetScore(Node node, Node target, Dictionary<Node, float> gScore)
    {
        float g = gScore.ContainsKey(node) ? gScore[node] : Mathf.Infinity;
        float h = Vector2.Distance(node.transform.position, target.transform.position);
        return g + h;
    }

    List<Node> RetracePath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> result = new List<Node>();

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
        Node[] nodes = GameObject.FindObjectsOfType<Node>();

        Node closest = null;
        float minDist = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float dist = Vector2.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    // ---------------- MOVEMENT ----------------

    void FollowPath()
    {
        if (path == null || path.Count == 0 || pathIndex >= path.Count)
            return;

        Node targetNode = path[pathIndex];

        Vector2 direction = targetNode.transform.position - transform.position;

        // Move horizontally
        float move = Mathf.Sign(direction.x);
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Jump if needed
        if (direction.y > 1f && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Reached node
        if (Vector2.Distance(transform.position, targetNode.transform.position) < 0.3f)
        {
            pathIndex++;
        }
    }
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    
    void FacePlayer()
    {
        if (player == null || !player.gameObject)
            return;

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(0.1f,0.1f , 0.1f);
        }
        else
        {
            transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
        }
    }
}

