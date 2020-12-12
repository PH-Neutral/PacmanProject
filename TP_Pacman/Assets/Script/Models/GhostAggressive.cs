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
        try {
        float minLength = float.MaxValue;
        int minIndex = -1;
        for (int i=0; i< possibleDirections.Count; i++) {
            float distance = Vector2Int.Distance(Coordinate + possibleDirections[i], gm.player.Coordinate + gm.player.Direction * 2);
            if (distance < minLength) {
                minLength = distance;
                minIndex = i;
            }
        }
        return possibleDirections[minIndex];
        } catch(Exception e) {
            Debug.LogError(this + " threw error: " + e);
        }
        return Vector2Int.zero;
    }

    protected override void ApplyModifiers() {
        Vector2Int searchCoord = Coordinate;
        int DEBUG_LOOP = 0;
        while(Direction != Vector2Int.zero && gm.GetNode(searchCoord += Direction) != null) {
            if(searchCoord == gm.player.Coordinate) {
                speedRatio = 1.15f;
                return;
            }
            DEBUG_LOOP++;
            if(DEBUG_LOOP > 100) {
                Debug.LogError(this + " entered an infinite loop!");
                break;
            }
        }
        speedRatio = 1;
    }
}
