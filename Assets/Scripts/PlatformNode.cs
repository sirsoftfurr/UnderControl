using System.Collections.Generic;
using UnityEngine;

public class PlatformNode : MonoBehaviour
{
    public List<NodeConnection> connections = new List<NodeConnection>();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (var conn in connections)
        {
            if (conn.targetNode != null)
            {
                Gizmos.DrawLine(transform.position, conn.targetNode.transform.position);
            }
        }
    }
}

[System.Serializable]
public class NodeConnection
{
    public PlatformNode targetNode;
    public ConnectionType type;
    public float cost = 1f;
}

public enum ConnectionType
{
    Walk,
    Jump,
    Drop
}

