using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObsessive : Ghost {
    protected override void Awake() {
        base.Awake();
        Type = GhostType.Obsessive;
    }

    protected override Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        // just find the shortest path towards player using the graph
        return FindShorterPathDirection(Coordinate, possibleDirections.ToArray(), playerTarget.Coordinate);
    }

    protected override void ApplyModifiers() {
        // gain speed based on percentage of balls eaten
        float ballsPercent = gm.RemainingBalls / (float)(gm.MaxBalls);
        // speed: +0% up to 20% balls eaten, +5% up to 50% balls eaten, then +10% till the end of the game
        _speedRatio = 1 + (ballsPercent > 0.8f ? 0 : 0.05f) + (ballsPercent > 0.5f ? 0 : 0.05f);
    }
}