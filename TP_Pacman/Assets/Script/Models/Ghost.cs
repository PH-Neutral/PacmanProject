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
        private set {
            _isInChaseMode = value;
        }
    }
    public Player playerTarget;
    public Vector2Int scatterTarget;
    public bool IsVulnerable = false;
    public bool firstActivation = true;


    List<Vector3> _pathOutOfSpawnerRef, _pathOutOfSpawner;
    float _vulnerableTimer = 0, _vulnerableDelay = 10f; // 20 seconds is too long, half may be ok ?
    float _scatterChaseTimer = 0, _chaseDelay = 20f, _scatterDelay = 7f;
    bool _isInChaseMode = true; // as opposed to scatter mode

    protected override void Awake() {
        base.Awake();
        IsWaiting = true;
    }

    protected override void Update() {
        if (playerTarget != null) {
            if(IsVulnerable) {
                _vulnerableTimer -= Time.deltaTime;
                if(_vulnerableTimer <= 0) {
                    MakeVulnerable(false);
                }
            } else {
                _scatterChaseTimer -= Time.deltaTime;
                if(_scatterChaseTimer <= 0) {
                    SwitchChaseMode(!Chase);
                }

            }
            base.Update();
        } else if (!IsWaiting) {
            //Debug.Log(this + " is not waiting anymore !");
            if (UpdateMovement(_pathOutOfSpawner[0])) {
                _pathOutOfSpawner.RemoveAt(0);
                if (_pathOutOfSpawner.Count == 0) {
                    playerTarget = gm.Player;
                    //Debug.Log(this + " has left the house, the hunt is ON!");
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
        // ++ get the possible directions in which to move (where there isn't a wall and where the ghost isn't coming from) ++ //
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
            if (IsVulnerable) {
                // run from the player if in vulnerable state
                Direction = FindLongerPathDirection(Coordinate, possibleDirections.ToArray(), playerTarget.Coordinate);
            } else if (Chase) {
                // chase the player using specific behavior
                Direction = ChooseDirection(possibleDirections);
            } else {
                // scatter towards the ghost's prefered position
                Direction = FindShorterPathDirection(Coordinate, possibleDirections.ToArray(), scatterTarget);
            }
        }

        if (!IsVulnerable && Chase) {
            ApplyModifiers();
        }
    }

    protected override void UpdateAnimator() {
        base.UpdateAnimator();
        animator.SetBool("Vulnerable", IsVulnerable);
    }

    protected override void SetReady() {
        base.SetReady();
        SwitchChaseMode(Chase);
    }

    public override void MakeAlive() {
        base.MakeAlive();
    }

    public override void MakeDead() {
        base.MakeDead();
        transform.position = _pathOutOfSpawnerRef[0];
        _pathOutOfSpawner = new List<Vector3>(_pathOutOfSpawnerRef);
        playerTarget = null;
        IsWaiting = true;
        MakeVulnerable(false);
        Invoke("StopWaiting", 2f);
    }

    void StopWaiting() {
        IsWaiting = false;
    }

    protected abstract Vector2Int ChooseDirection(List<Vector2Int> possibleDirections);

    protected abstract void ApplyModifiers();

    public void MakeVulnerable(bool vulnerable) {
        IsVulnerable = vulnerable;
        if(vulnerable) {
            // ++ make them turn away ++ //
            Vector2Int lc = _lastCoord;
            _lastCoord = _nextCoord;
            _nextCoord = lc;
            // start countdown
            _vulnerableTimer = _vulnerableDelay;
        } else {
            // reset mode to "chase"
            SwitchChaseMode(true);
        }
        UpdateAnimator();
    }

    void SwitchChaseMode(bool chaseMode) {
        _scatterChaseTimer = chaseMode ? _chaseDelay : _scatterDelay;
        Chase = chaseMode;
    }

    public void SetOutOfSpawnerPath(List<Vector3> path) {
        _pathOutOfSpawnerRef = path;
        _pathOutOfSpawner = new List<Vector3>(path);
    }

    protected Vector2Int FindLongerPathDirection(Vector2Int startCoord, Vector2Int[] directions, Vector2Int targetCoord) {
        int longerPath = int.MinValue;
        int longerIndex = -1;
        for(int i = 0; i < directions.Length; i++) {
            List<Node> path = gm.GetPath(gm.GetNode(startCoord).GetNeighbor(directions[i]).Coordinate, targetCoord);
            if (path == null) { continue; }
            int pathLength = path.Count;
            if(pathLength > longerPath) {
                longerPath = pathLength;
                longerIndex = i;
            }
        }
        if (longerIndex == -1) {
            return FindLongerPathDirectionVector(startCoord, directions, targetCoord);
        }
        return directions[longerIndex];
    }

    protected Vector2Int FindShorterPathDirection(Vector2Int startCoord, Vector2Int[] directions, Vector2Int targetCoord) {
        int shorterPath = int.MaxValue;
        int shorterIndex = -1;
        for(int i = 0; i < directions.Length; i++) {
            List<Node> path = gm.GetPath(gm.GetNode(startCoord).GetNeighbor(directions[i]).Coordinate, targetCoord);
            if(path == null) { continue; }
            int pathLength = path.Count;
            if(pathLength < shorterPath) {
                shorterPath = pathLength;
                shorterIndex = i;
            }
        }
        if(shorterIndex == -1) {
            return FindShorterPathDirectionVector(startCoord, directions, targetCoord);
        }
        return directions[shorterIndex];
    }

    protected Vector2Int FindLongerPathDirectionVector(Vector2Int startCoord, Vector2Int[] directions, Vector2Int targetCoord) {
        float maxLength = float.MinValue;
        int maxIndex = -1;
        for(int i = 0; i < directions.Length; i++) {
            float distance = Vector2Int.Distance(startCoord + directions[i], targetCoord);
            if(distance > maxLength) {
                maxLength = distance;
                maxIndex = i;
            }
        }
        return directions[maxIndex];
    }

    protected Vector2Int FindShorterPathDirectionVector(Vector2Int startCoord, Vector2Int[] directions, Vector2Int targetCoord) {
        float minLength = float.MaxValue;
        int minIndex = -1;
        for(int i = 0; i < directions.Length; i++) {
            float distance = Vector2Int.Distance(startCoord + directions[i], targetCoord);
            if(distance < minLength) {
                minLength = distance;
                minIndex = i;
            }
        }
        return directions[minIndex];
    }
}

public enum GhostType {
    Lazy, Obsessive, Aggressive, Witty
}
