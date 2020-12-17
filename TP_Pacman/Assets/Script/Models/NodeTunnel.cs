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

    /// <summary>
    /// Get the neighbor associated with this direction.
    /// </summary>
    /// <param name="direction">The direction from which to get the node.</param>
    /// <returns>The node associated with this direction. Null if there is no associated node in this direction.</returns>
    public override Node GetNeighbor(Vector2Int direction) {
        if(direction == Vector2Int.zero) { return null; }
        Node neighbor = base.GetNeighbor(direction);
        Vector2Int searchCoord = Coordinate + direction;
        if (neighbor == null && searchCoord.y == LinkedTunnel.Coordinate.y 
            && Mathf.Abs(searchCoord.x - LinkedTunnel.Coordinate.x) > Mathf.Abs(Coordinate.x - LinkedTunnel.Coordinate.x)) {
            return LinkedTunnel;
        }
        return neighbor;
    }
}
