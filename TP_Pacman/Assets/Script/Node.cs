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
            if (Neighbors.Count == 2) {
                return (Neighbors[0].Coordinate.x == Neighbors[1].Coordinate.x) || (Neighbors[0].Coordinate.y == Neighbors[1].Coordinate.y);
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
    public string Name { get; private set; }
    public Vector2Int Coordinate { get; private set; }
    public int Depth { get; set; }
    public List<Node> Neighbors { get; private set; }
    public Node Parent { get; set; }

    public Node(string name, Vector2Int coordinate) {
        Name = name;
        Coordinate = coordinate;
        Depth = -1;
        Neighbors = new List<Node>();
        Parent = null;
    }
}

public enum NodeType {
    None, Tunnel, Hallway, Corner, Crossroad
}
