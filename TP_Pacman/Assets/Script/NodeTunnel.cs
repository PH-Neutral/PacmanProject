using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTunnel : Node {
    public Node LinkedNode { get; private set; }

    public NodeTunnel(Vector2Int coordinate) : base(coordinate) {}
    public NodeTunnel(Node node) : base(node.Coordinate) {
        Type = NodeType.Tunnel;
        Neighbors = node.Neighbors;
        Depth = node.Depth;
        Parent = node.Parent;
        LinkedNode = null;
    }

    public void LinkNode(NodeTunnel node, bool linkBoth = true) {
        LinkedNode = node;
        Neighbors.Add(node);
        if (linkBoth) {
            node.LinkNode(this, false);
        }
    }
}
