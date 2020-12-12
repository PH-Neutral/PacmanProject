using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLazy : Ghost {
    protected override void Awake() {
        base.Awake();
        Type = GhostType.Lazy;
    }

    protected override Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        if (possibleDirections.Count > 1) {
            foreach(Vector2Int dir in possibleDirections) {
                Vector2Int playerCoord = gm.player.Coordinate;
                Vector2Int searchCoord = Coordinate;
                int DEBUG_LOOP = 0;
                while (dir != Vector2Int.zero && gm.GetNode(searchCoord += dir) != null) {
                    if (searchCoord == playerCoord) {
                        possibleDirections.Remove(dir);
                        break;
                    }
                    DEBUG_LOOP++;
                    if (DEBUG_LOOP > 100) {
                        Debug.LogError(this + " entered an infinite loop!");
                        break;
                    }
                }
                if (possibleDirections.Count < 2) {
                    break;
                }
            }
        }
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    protected override void ApplyModifiers() {
        // do nothing
    }
}