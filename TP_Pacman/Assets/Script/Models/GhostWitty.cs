using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWitty : Ghost {
    [SerializeField] int _predictivePathLength = 4;
    protected override void Awake() {
        base.Awake();
        Type = GhostType.Witty;
    }

    protected override Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        Vector2Int playerDir = gm.Player.Direction;
        int length = 0;
        Node searchNode = gm.GetNode(gm.Player.Coordinate);
        Vector2Int targetCoord;
        do {
            targetCoord = searchNode.Coordinate;
            if (length++ >= _predictivePathLength && (searchNode.Type == NodeType.Corner || searchNode.Type == NodeType.Crossroad)) {
                break;
            }
        } while(playerDir != Vector2Int.zero && (searchNode = searchNode.GetNeighbor(playerDir)) != null) ;
            return FindShorterPathDirection(Coordinate, possibleDirections.ToArray(), targetCoord);
    }

    protected override void ApplyModifiers() {
        // do nothing
    }
}