using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ghost : Character {
    public Player target;
    public GhostType Type {
        get; protected set;
    }
    public bool IsVulnerable = false;

    bool IsWaiting {
        get; set;
    }

    protected override void Awake() {
        base.Awake();
        IsWaiting = true;
    }

    protected override void Update() {
        if (IsWaiting) {
            // ---
        }
        base.Update();
        /*/ Debug
        for (int i=0; i<_path.Count; i++) {
            Vector3 startPos, endPos;
            if (i == 0) {
                startPos = _startPosition;
            } else {
                startPos = gm.CellToWorld(_path[i - 1].Coordinate);
            }
            endPos = gm.CellToWorld(_path[i].Coordinate);
            Debug.DrawLine(startPos, endPos, Color.yellow, 1f / speed);
        }*/
    }

    protected override void DoEachUpdate() {
       // do nothing
    }

    protected override void DoWhenCellReached() {
        Node currentNode = gm.GetNode(Coordinate);
        List<Vector2Int> possibleDirections = new List<Vector2Int>();
        for(int i = 0; i < PacTools.v2IntDirections.Length; i++) {
            Vector2Int dir = PacTools.v2IntDirections[i];
            if(_lastCoord != Coordinate + dir && currentNode.HasNeighbor(dir)) {
                possibleDirections.Add(dir);
            }
        }
        if (possibleDirections.Count == 1) {
            Direction = possibleDirections[0];
        } else {
            Direction = ChooseDirection(possibleDirections);
        }

        ApplyModifiers();
    }

    protected virtual Vector2Int ChooseDirection(List<Vector2Int> possibleDirections) {
        // to be implemented by child class
        return Direction;
    }

    protected virtual void ApplyModifiers() {
        // to be implemented by child class
    }

    protected Vector2Int FindShorterPathDirection(Vector2Int startCoord, Vector2Int[] directions, Vector2Int targetCoord) {
        int shorterPath = int.MaxValue;
        int shorterIndex = -1;
        for(int i = 0; i < directions.Length; i++) {
            int pathLength = gm.GetPath(startCoord + directions[i], targetCoord).Count;
            if(pathLength < shorterPath) {
                shorterPath = pathLength;
                shorterIndex = i;
            }
        }
        return directions[shorterIndex];
    }
}

public enum GhostType {
    Lazy, Obsessive, Aggressive, Witty
}
