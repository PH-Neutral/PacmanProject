using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTunnel : Node {
    public Node LinkedNode { get; set; }

    public NodeTunnel(Vector2Int coordinate) : base(coordinate) {}
    public NodeTunnel(Node node) : base(node.Coordinate) {
        Depth = node.Depth;
        Neighbors = node.Neighbors;
        Parent = node.Parent;
        LinkedNode = null;
    }
}
