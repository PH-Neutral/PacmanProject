using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public bool IsTunnel {
        get {
            return Neighbors.Count == 1;
        }
    }
    public bool IsHallway {
        get {
            if(Neighbors.Count == 2) {
                return (Neighbors[0].Coordinate.x == Neighbors[1].Coordinate.x)
                    || (Neighbors[0].Coordinate.y == Neighbors[1].Coordinate.y);
            }
            return false;
        }
    }
    public bool IsCorner {
        get {
            if(Neighbors.Count == 2) {
                return !IsHallway;
            }
            return false;
        }
    }
    public bool IsCrossroad {
        get {
            return Neighbors.Count > 2;
        }
    }
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
}

public enum NodeType {
    None, Tunnel, Hallway, Corner, Crossroad
}
