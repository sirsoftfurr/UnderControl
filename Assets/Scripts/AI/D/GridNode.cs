using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
        public Vector2 position;
        public List<GridNode> neighbors = new List<GridNode>();
}
