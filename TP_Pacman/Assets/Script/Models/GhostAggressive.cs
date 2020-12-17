using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAggressive : Ghost {
    protected override void Awake() {
        base.Awake();
        Type = GhostType.Aggressive;
    }

    protected override Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        // just find the shortest path towards player using vectors
        return FindShorterPathDirectionVector(Coordinate, possibleDirections.ToArray(), gm.Player.Coordinate + gm.Player.Direction * 2);
    }

    protected override void ApplyModifiers() {
        Vector2Int searchCoord = Coordinate;
        // if player is visible in this direction, up this ghost's speed to +15% of base speed
        while(Direction != Vector2Int.zero && gm.GetNode(searchCoord += Direction) != null) {
            if(searchCoord == gm.Player.Coordinate) {
                _speedRatio = 1.15f;
                return;
            }
        }
        _speedRatio = 1;
    }
}
