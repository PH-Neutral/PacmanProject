using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character {
    public Player target;
    public GhostType type;
    public bool IsVulnerable = false;

    List<Node> _path;
    bool IsWaiting {
        get; set;
    }

    protected override void Awake() {
        base.Awake();
        IsWaiting = true;
    }

    protected override void Update() {
        if (IsWaiting) {

        }
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
        
        Vector2Int nextCoord = Coordinate;
        switch (type) {
            case GhostType.Dummy:
                //List<Node> possibleNeighbors = new List<Node>();
                List<Node> neighbors = gm.GetNode(Coordinate).Neighbors;
                nextCoord = neighbors[Random.Range(0, neighbors.Count)].Coordinate;
                break;
            case GhostType.Tracker:
                _path = gm.GetPath(Coordinate, target.Coordinate) ?? _path;
                nextCoord = _path[0].Coordinate;
                break;
            case GhostType.Seer:
                break;
            case GhostType.Embusher:
                break;
            default:
                //nextCoord = Coordinate;
                break;
        }
        Direction = nextCoord - Coordinate;
    }
}

public enum GhostType {
    Dummy, Tracker, Embusher, Seer
}
