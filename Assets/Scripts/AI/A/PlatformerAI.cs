using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlatformerAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float jumpForce = 6f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;

    [Header("Target")]
    public Transform target;

    [Header("Pathfinding")]
    public float repathRate = 0.5f;

    private Rigidbody2D rb;
    private PlatformNode[] allNodes;

    private List<PlatformNode> path = new List<PlatformNode>();
    private int pathIndex = 0;

    private bool isGrounded;
    private bool isJumping;
    private float lastJumpTime;
    public float jumpCooldown = 0.5f;

    private float repathTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        allNodes = FindObjectsOfType<PlatformNode>();
        
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        Debug.Log("Update running");
        CheckGround();

        HandleRepath();

        FollowPath();
    }

    void HandleRepath()
    {
        
        if (target == null) return;

        repathTimer += Time.deltaTime;

        if (repathTimer >= repathRate)
        {
            void HandleRepath()
            {
                if (target == null)
                {
                    Debug.Log("NO TARGET");
                    return;
                }
                
                Debug.Log("TARGET FOUND: " + target.name);

                repathTimer += Time.deltaTime;

                if (repathTimer >= repathRate)
                {
                    Debug.Log("Repathing...");
                    MoveToPosition(target.position);
                    repathTimer = 0f;
                }
            }
            MoveToPosition(target.position);
            repathTimer = 0f;
        }
    }

    public void MoveToPosition(Vector2 position)
    {
        PlatformNode start = FindClosestNode(transform.position);
        PlatformNode end = FindClosestNode(position);

        if (start == null || end == null) return;

        path = FindPathAStar(start, end);

        if (path == null || path.Count == 0)
            return;

        pathIndex = 0;
    }

    void FollowPath()
    {
        if (path == null || pathIndex >= path.Count) return;

        PlatformNode next = path[pathIndex];

        float dist = Vector2.Distance(transform.position, next.transform.position);

        if (dist < 0.3f)
        {
            pathIndex++;
            return;
        }

        Vector2 dir = (next.transform.position - transform.position).normalized;

        NodeConnection conn = GetConnection(pathIndex);
        if (conn == null) return;

        switch (conn.type)
        {
            case ConnectionType.Walk:
                Move(dir);
                break;

            case ConnectionType.Jump:
                Jump(dir);
                break;

            case ConnectionType.Drop:
                Drop(dir);
                break;
        }
    }

    void Move(Vector2 dir)
    {
        if (isJumping) return;

        float targetSpeed = dir.x * speed;
        float smooth = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, 10f * Time.deltaTime);

        rb.linearVelocity = new Vector2(smooth, rb.linearVelocity.y);
    }

    void Jump(Vector2 dir)
    {
        Move(dir);

        float heightDiff = GetNextNodeHeight();

        if (isGrounded && heightDiff > 0.5f && Time.time > lastJumpTime + jumpCooldown)
        {
            rb.linearVelocity = new Vector2(dir.x * speed, jumpForce);
            lastJumpTime = Time.time;
            isJumping = true;
        }
    }

    void Drop(Vector2 dir)
    {
        Move(dir);

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(dir.x * speed, -1f);
        }
    }

    float GetNextNodeHeight()
    {
        if (pathIndex >= path.Count) return 0;

        return path[pathIndex].transform.position.y - transform.position.y;
    }

    void CheckGround()
    {
        isGrounded = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (isGrounded)
            isJumping = false;
    }

    PlatformNode FindClosestNode(Vector2 pos)
    {
        PlatformNode closest = null;
        float minDist = Mathf.Infinity;

        foreach (var node in allNodes)
        {
            if (node == null) continue;

            float heightDiff = Mathf.Abs(node.transform.position.y - pos.y);
            if (heightDiff > 5f) continue;

            float d = Vector2.Distance(pos, node.transform.position);

            if (d < minDist)
            {
                minDist = d;
                closest = node;
            }
        }

        return closest;
    }

    NodeConnection GetConnection(int index)
    {
        if (index == 0 || index >= path.Count) return null;

        PlatformNode from = path[index - 1];
        PlatformNode to = path[index];

        foreach (var c in from.connections)
        {
            if (c.targetNode == to)
                return c;
        }

        return null;
    }

    // ===== A* =====

    List<PlatformNode> FindPathAStar(PlatformNode start, PlatformNode goal)
    {
        List<PlatformNode> open = new List<PlatformNode>();
        HashSet<PlatformNode> closed = new HashSet<PlatformNode>();

        Dictionary<PlatformNode, PlatformNode> cameFrom = new Dictionary<PlatformNode, PlatformNode>();
        Dictionary<PlatformNode, float> gScore = new Dictionary<PlatformNode, float>();
        Dictionary<PlatformNode, float> fScore = new Dictionary<PlatformNode, float>();

        open.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (open.Count > 0)
        {
            PlatformNode current = GetLowest(open, fScore);

            if (current == goal)
                return Reconstruct(cameFrom, current);

            open.Remove(current);
            closed.Add(current);

            foreach (var conn in current.connections)
            {
                if (conn.targetNode == null) continue;

                PlatformNode neighbor = conn.targetNode;
                if (neighbor == null) continue;

                if (closed.Contains(neighbor)) continue;

                float tentative = gScore[current] + conn.cost;

                if (!open.Contains(neighbor))
                    open.Add(neighbor);

                else if (tentative >= gScore.GetValueOrDefault(neighbor, Mathf.Infinity))
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative;
                fScore[neighbor] = tentative + Heuristic(neighbor, goal);
            }
        }

        return null;
    }

    float Heuristic(PlatformNode a, PlatformNode b)
    {
        return Vector2.Distance(a.transform.position, b.transform.position);
    }

    PlatformNode GetLowest(List<PlatformNode> list, Dictionary<PlatformNode, float> f)
    {
        PlatformNode best = list[0];
        float bestScore = f.GetValueOrDefault(best, Mathf.Infinity);

        foreach (var n in list)
        {
            float score = f.GetValueOrDefault(n, Mathf.Infinity);

            if (score < bestScore)
            {
                best = n;
                bestScore = score;
            }
        }

        return best;
    }

    List<PlatformNode> Reconstruct(Dictionary<PlatformNode, PlatformNode> map, PlatformNode current)
    {
        List<PlatformNode> result = new List<PlatformNode>();

        while (current != null)
        {
            result.Add(current);
            map.TryGetValue(current, out current);
        }

        result.Reverse();
        return result;
    }
    
}

