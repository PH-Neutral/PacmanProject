using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public string Name { get; protected set; }
    public NodeType Type { get; set; }
    public Vector2Int Coordinate { get; protected set; }
    public List<Node> Neighbors { get; protected set; }
    public int Depth { get; set; }
    public Node Parent { get; set; }

    public Node(Vector2Int coordinate) {
        Name = "Node " + coordinate.ToString();
        Coordinate = coordinate;
        Depth = -1;
        Neighbors = new List<Node>();
        Parent = null;
    }

    public bool HasNeighbor(Vector2Int direction) {
        return GetNeighbor(direction) != null;
    }

    /// <summary>
    /// Get the neighbor associated with this direction.
    /// </summary>
    /// <param name="direction">The direction from which to get the node.</param>
    /// <returns>The node associated with this direction. Null if there is no associated node in this direction.</returns>
    public virtual Node GetNeighbor(Vector2Int direction) {
        if (direction == Vector2Int.zero) { return null; }
        for(int i=0; i<Neighbors.Count; i++) {
            if (Neighbors[i].Coordinate == this.Coordinate + direction) {
                return Neighbors[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Reset Depth and Parent.
    /// </summary>
    public void Clean() {
        Depth = -1;
        Parent = null;
    }

    public override string ToString() {
        return "Node at " + Coordinate.ToString() + " of type " + Type + " with " + Neighbors.Count + " neighbors. [Depth = " + Depth + "; Parent = " + (Parent != null ? Parent.Name : "NULL") + "]";
    }
}

public enum NodeType {
    None, Tunnel, Hallway, Corner, Crossroad
}
