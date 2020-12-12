using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTunnel : Node {
    public NodeTunnel LinkedTunnel { get; private set; }

    public NodeTunnel(Vector2Int coordinate) : base(coordinate) {}
    public NodeTunnel(Node node) : base(node.Coordinate) {
        Type = NodeType.Tunnel;
        Neighbors = node.Neighbors;
        Depth = node.Depth;
        Parent = node.Parent;
        LinkedTunnel = null;
    }

    public void LinkTunnel(NodeTunnel node, bool linkBoth = true) {
        LinkedTunnel = node;
        Neighbors.Add(node);
        if (linkBoth) {
            node.LinkTunnel(this, false);
        }
    }

    public override Node GetNeighbor(Vector2Int relativeCoordinate) {
        //return base.GetNeighbor(relativeCoordinate);
        Node neighbor = base.GetNeighbor(relativeCoordinate);
        Vector2Int searchCoord = Coordinate + relativeCoordinate;
        if (neighbor == null && searchCoord.y == LinkedTunnel.Coordinate.y 
            && Mathf.Abs(searchCoord.x - LinkedTunnel.Coordinate.x) > Mathf.Abs(Coordinate.x - LinkedTunnel.Coordinate.x)) {
            return LinkedTunnel;
        }
        return neighbor;
    }
}
