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

    public bool HasNeighbor(Vector2Int relativeCoordinate) {
        return GetNeighbor(relativeCoordinate) != null;
    }

    public virtual Node GetNeighbor(Vector2Int relativeCoordinate) {
        if (relativeCoordinate == Vector2Int.zero) { return null; }
        foreach(Node n in Neighbors) {
            if (n.Coordinate == this.Coordinate + relativeCoordinate) {
                return n;
            }
        }
        return null;
    }

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
