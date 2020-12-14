using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostObsessive : Ghost {
    protected override void Awake() {
        base.Awake();
        Type = GhostType.Obsessive;
    }

    protected override Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        return FindShorterPathDirection(Coordinate, possibleDirections.ToArray(), target.Coordinate);
    }

    protected override void ApplyModifiers() {
        float ballsPercent = gm.RemainingBalls / (float)(gm.MaxBalls);
        _speedRatio = 1 + (ballsPercent > 0.8f ? 0 : 0.05f) + (ballsPercent > 0.5f ? 0 : 0.05f);
    }
}