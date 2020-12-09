using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character {
    public Player target;
    public GhostType type;

    public bool IsVulnerable = false;

    List<Node> _path;

    protected override void Update() {
        base.Update();
        if (_path == null) { return; }
        GameManager gm = GameManager.Instance;
        for (int i=0; i<_path.Count; i++) {
            Vector3 startPos, endPos;
            if (i == 0) {
                startPos = _startPosition;
            } else {
                startPos = gm.CellToWorld(_path[i - 1].Coordinate);
            }
            endPos = gm.CellToWorld(_path[i].Coordinate);
            Debug.DrawLine(startPos, endPos, Color.yellow);
        }
    }

    protected override void ChooseDirection() {
        GameManager gm = GameManager.Instance;
        /*if (!currentNode.HasNeighbor(Direction)) {
            // if ghost hit a wall

        }*/
        _path = gm.GetPath(Coordinate, target.Coordinate) ?? _path;
        Vector2Int nextCoord = _path[0].Coordinate;
        /*switch (type) {
            case GhostType.Dummy:
                List<Node> possibleNeighbors = new List<Node>();
                List<Node> neighbors = maze.GetNode(Coordinate).Neighbors;
                foreach(Node n in neighbors) {
                    if (n)
                } 
                break;
            case GhostType.Tracker:
                nextCoord = _path[0].Coordinate;
                break;
            case GhostType.Seer:
                break;
            case GhostType.Embusher:
                break;
            default:
                nextCoord = Coordinate;
                break;
        }*/
        Direction = nextCoord - Coordinate;
    }

    protected override void UpdateDirectionAnimation() {
        int dirInt = 0; // idle
        if(Direction == Vector2Int.right) {
            dirInt = 1; // moving right
        } else if(Direction == Vector2Int.down) {
            dirInt = 2; // moving down
        } else if(Direction == Vector2Int.left) {
            dirInt = 3; // moving left
        } else if(Direction == Vector2Int.up) {
            dirInt = 4; // moving up
        }
        animator.SetInteger("Direction", dirInt);
    }
}

public enum GhostType {
    Dummy, Tracker, Embusher, Seer
}
