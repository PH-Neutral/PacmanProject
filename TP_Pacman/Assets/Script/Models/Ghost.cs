using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ghost : Character {
    public GhostType Type {
        get; protected set;
    }
    public bool IsWaiting {
        get; set;
    }
    public bool Chase {
        get {
            return IsVulnerable ? false : _isInChaseMode;
        }
    }
    public Player playerTarget;
    public Vector2Int scatterTarget;
    public bool IsVulnerable = false;


    List<Vector3> _pathOutOfSpawner;
    float _vulnerableTimer = 0, _vulnerableDelay = 20f; // 20 seconds ? look it up
    float _chaseTimer = 0, _chaseDelay = 20f, _scatterDelay = 7f;
    bool _isInChaseMode = true; // as opposed to scatter mode

    protected override void Awake() {
        base.Awake();
        IsWaiting = true;
    }

    protected override void Update() {
        if (IsVulnerable) {
            _vulnerableTimer -= Time.deltaTime;
            if (_vulnerableTimer <= 0) {
                MakeVulnerable(false);
            }
        } else {

        }
        if (playerTarget != null) {
            base.Update();
        } else if (!IsWaiting) {
            //Debug.Log(this + " is not waiting anymore !");
            if (UpdateMovement(_pathOutOfSpawner[0])) {
                _pathOutOfSpawner.RemoveAt(0);
                if (_pathOutOfSpawner.Count == 0) {
                    playerTarget = gm.Player;
                    Debug.Log(this + " has left the house, the hunt is ON!");
                    SetReady();
                }
            }
        } else {
            //Debug.Log(this + " is waiting for something...");
        }
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
        //Debug.Log("currentNode = " + currentNode + "; Coordinate = " + Coordinate);
        List<Vector2Int> possibleDirections = new List<Vector2Int>();
        for(int i = 0; i < PacTools.v2IntDirections.Length; i++) {
            Vector2Int dir = PacTools.v2IntDirections[i];
            if(Coordinate + dir != _lastCoord && currentNode.HasNeighbor(dir)) {
                possibleDirections.Add(dir);
            }
        }
        if (possibleDirections.Count == 1) {
            Direction = possibleDirections[0];
        } else {
            if (Chase) {
                Direction = ChooseDirection(possibleDirections);
            } else {
                Direction = FindShorterPathDirection(Coordinate, possibleDirections.ToArray(), scatterTarget);
            }
        }

        if (Chase) {
            ApplyModifiers();
        }
    }

    protected abstract Vector2Int ChooseDirection(List<Vector2Int> possibleDirections);

    protected abstract void ApplyModifiers();

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

    public void MakeVulnerable(bool vulnerable) {
        IsVulnerable = vulnerable;
        if (vulnerable) {
            // ++ make them turn away ++ //
            Vector2Int lc = _lastCoord;
            _lastCoord = _nextCoord;
            _nextCoord = lc;
            // start countdown
            _vulnerableTimer = _vulnerableDelay;
            // make scatter for longer time ?

            // change animator to reflect vulnerable state
        } else {
            // reset scatter mode

            // change animator to reflect normal state
        }
    }

    public void SetOutOfSpawnerPath(List<Vector3> path) {
        _pathOutOfSpawner = path;
    }
}

public enum GhostType {
    Lazy, Obsessive, Aggressive, Witty
}
