using System;
using System.Collections.Generic;
using UnityEngine;

public static class PacTools {
    public static readonly Vector2Int[] v2IntDirections = new Vector2Int[4] {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left, 
        Vector2Int.up 
    };
    public static readonly Vector3[] eulerDirections = new Vector3[] { 
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 270),
        new Vector3(0, 0, 180),
        new Vector3(0, 0, 90)
    };

    public static Vector3 GetEuler(this Vector2Int direction) {
        for (int i=0; i<v2IntDirections.Length;i++) {
            if (direction == v2IntDirections[i]) {
                return eulerDirections[i];
            }
        }
        return Vector3.zero;
    }
}
