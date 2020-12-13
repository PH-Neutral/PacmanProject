using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLazy : Ghost {
    protected override void Awake() {
        base.Awake();
        Type = GhostType.Lazy;
    }

    protected override Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        if (possibleDirections.Count == 1) {
            return possibleDirections[Random.Range(0, possibleDirections.Count)];
        }
        List<Vector2Int> directions = new List<Vector2Int>(possibleDirections);
        for(int i = 0; i < possibleDirections.Count; i++) {
            Vector2Int dir = possibleDirections[i];
            Vector2Int playerCoord = gm.Player.Coordinate;
            Vector2Int searchCoord = Coordinate;
            while(dir != Vector2Int.zero && gm.GetNode(searchCoord += dir) != null) {
                if(searchCoord == playerCoord) {
                    directions.Remove(dir);
                    break;
                }
            }
            if(directions.Count < 2) {
                break;
            }
        }
        return directions[Random.Range(0, directions.Count)];
    }

    protected override void ApplyModifiers() {
        // do nothing
    }
}