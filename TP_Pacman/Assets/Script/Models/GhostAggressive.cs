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
        float minLength = float.MaxValue;
        int minIndex = -1;
        for (int i=0; i< possibleDirections.Count; i++) {
            float distance = Vector2Int.Distance(Coordinate + possibleDirections[i], gm.Player.Coordinate + gm.Player.Direction * 2);
            if (distance < minLength) {
                minLength = distance;
                minIndex = i;
            }
        }
        return possibleDirections[minIndex];
    }

    protected override void ApplyModifiers() {
        Vector2Int searchCoord = Coordinate;
        while(Direction != Vector2Int.zero && gm.GetNode(searchCoord += Direction) != null) {
            if(searchCoord == gm.Player.Coordinate) {
                speedRatio = 1.15f;
                return;
            }
        }
        speedRatio = 1;
    }
}
