using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector2 gridSize = new Vector2(20, 10);
    public float nodeSpacing = 1f;
    public LayerMask groundLayer;

    public List<GridNode> nodes = new List<GridNode>();

    void Start()
    {
        GenerateGrid();
        ConnectNodes();
    }

    void GenerateGrid()
    {
        for (float x = 0; x < gridSize.x; x += nodeSpacing)
        {
            for (float y = 0; y < gridSize.y; y += nodeSpacing)
            {
                Vector2 worldPos = new Vector2(x, y);

                // Check if ground exists below
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.down, 1.5f, groundLayer);

                if (hit.collider != null)
                {
                    GameObject nodeObj = new GameObject("Node");
                    nodeObj.transform.position = hit.point + Vector2.up * 0.1f;

                    GridNode node = nodeObj.AddComponent<GridNode>();
                    node.position = nodeObj.transform.position;

                    nodes.Add(node);
                }
            }
        }
    }

    void ConnectNodes()
    {
        foreach (var node in nodes)
        {
            foreach (var other in nodes)
            {
                if (node == other) continue;

                float dist = Vector2.Distance(node.position, other.position);

                // WALK connection
                if (dist <= 1.5f && Mathf.Abs(node.position.y - other.position.y) < 0.5f)
                {
                    node.neighbors.Add(other);
                }

                // JUMP connection
                if (dist <= 3f && other.position.y > node.position.y && other.position.y - node.position.y < 2.5f)
                {
                    node.neighbors.Add(other);
                }
            }
        }
    }
}
