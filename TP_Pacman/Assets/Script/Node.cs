using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
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
