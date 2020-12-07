using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character {
    public Player target;

    List<Node> _path;

    protected override void Update() {
        base.Update();
        if (_path == null) { return; }
        for (int i=0; i<_path.Count; i++) {
            Vector3 startPos, endPos;
            if (i == 0) {
                startPos = _startPosition;
            } else {
                startPos = Maze.Instance.GetWorldPositionFromCoord(_path[i - 1].Coordinate);
            }
            endPos = Maze.Instance.GetWorldPositionFromCoord(_path[i].Coordinate);
            Debug.DrawLine(startPos, endPos, Color.yellow);
        }
    }

    protected override void ChooseDirection() {
        Maze maze = Maze.Instance;
        //Vector2Int playerCoordinate = maze.GetPlayerGridPosition();
        _path = maze.GetPath(Coordinate, target.Coordinate) ?? _path;
        //Debug.Log("Path is " + _path.Count + " cells long.");
        Direction = _path[0].Coordinate - Coordinate;
    }
}
