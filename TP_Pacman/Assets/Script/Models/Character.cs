using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
    public Vector2Int Coordinate {
        get { 
            return GameManager.Instance.WorldToCell(transform.position);
        }
    }
    public Vector2Int Direction {
        get { return _direction; }
        protected set { _direction = value; }
    }
    [SerializeField] protected float speed;
    protected float _speedRatio = 1;
    protected Animator animator;
    protected Vector2Int _direction;
    protected Vector2Int _lastCoord, _currentCoord, _nextCoord;
    protected GameManager gm; // reference to the GameManager for ease of use

    /// <summary>
    /// Move speed of player in cell/seconds.
    /// </summary>
    public float Speed {
        get { return speed * _speedRatio; }
    }

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        gm = GameManager.Instance;
        MakeAlive();
        SetReady();
    }

    protected virtual void Update() {
        if (gm.GamePaused) { return; }

        DoEachUpdate();

        MoveThroughMaze();
    }

    void MoveThroughMaze() {
        if(UpdateMovement(_nextCoord)) {
            // if character has reached its target, check if it can still go in the same direction
            _currentCoord = Coordinate;
            DoWhenCellReached();
            _lastCoord = _currentCoord;
            if(gm.GetNode(Coordinate).HasNeighbor(Direction)) {
                // if there is a cell in the current direction, then start moving towards it
                _nextCoord = Coordinate + Direction;
                UpdateAnimator();
            }
        } else {
            // if character has not arrived to its target, check if it is going through a tunnel
            Node nodeAtStart;
            if(gm.GetNode(Coordinate) == null && (nodeAtStart = gm.GetNode(_currentCoord)).Type == NodeType.Tunnel) {
                // if going through a tunnel, move the character from one end of the tunnel to the other while keeping the relative worldPosition to the last cell
                Vector3 startPos = gm.CellToWorld(_currentCoord);
                NodeTunnel nt1 = nodeAtStart as NodeTunnel;
                NodeTunnel nt2 = nt1.LinkedTunnel;
                Vector2Int newStartCoord = nt2.Coordinate + Vector2Int.left * (int)Mathf.Sign(nt1.Coordinate.x - nt2.Coordinate.x);
                Vector3 newStartPos = gm.CellToWorld(newStartCoord);
                transform.position -= startPos - newStartPos;
                _currentCoord = newStartCoord;
                _nextCoord = nt2.Coordinate;
                _lastCoord = _currentCoord;
            }
        }
    }

    public bool UpdateMovement(Vector2Int targetCoord) {
        return UpdateMovement(gm.CellToWorld(targetCoord));
    }

    /// <summary>
    /// Moves gameobject towards target point in world space with provided speed.
    /// </summary>
    /// <param name="targetPoint">The position in world space to move toward.</param>
    /// <param name="speed">The speed applied to the movement.</param>
    /// <returns>True when gameobject arrived at targetPoint.</returns>
    public bool UpdateMovement(Vector3 targetPoint) {
        float remainingDistance = Vector3.Distance(transform.position, targetPoint);
        if (remainingDistance != 0) {
            transform.position = Vector3.Lerp(transform.position, targetPoint, gm.DistanceCellToWorld(Time.deltaTime * Speed) / remainingDistance);
            return false;
        }
        return true;
    }

    protected virtual void UpdateAnimator() {
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

    protected virtual void SetReady() {
        _lastCoord = Coordinate;
        _currentCoord = Coordinate;
        _nextCoord = Coordinate;
    }

    public virtual void MakeDead() {
        animator.speed = 1f;
        animator.SetBool("Dead", true);
    }

    public virtual void MakeAlive() {
        animator.speed = Speed;
        animator.SetBool("Dead", false);
    }

    protected abstract void DoEachUpdate();

    protected abstract void DoWhenCellReached();

}
