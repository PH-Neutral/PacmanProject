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
    /// <summary>
    /// Move speed of player in cell/seconds.
    /// </summary>
    public float speed;

    protected Animator animator;
    protected Vector2Int _direction;
    protected Vector3 _startPosition, _endPosition;
    protected Vector2Int _lastCoord, _currentCoord, _nextCoord;

    float _Delay {
        get { return 1f / speed; }
    }
    float _timer = 0f;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        SetReady();
    }

    protected virtual void Update() {
        GameManager gm = GameManager.Instance;
        if (gm.GamePaused) { return; }

        ChooseDirection(); // choose the direction to move towards

        _timer += Time.deltaTime;
        if(_timer >= _Delay) {
            _timer -= _Delay;
            transform.position = gm.CellToWorld(_nextCoord); // place character at center of cell
            if(gm.GetNode(Coordinate).HasNeighbor(Direction)) {
                // if there is a cell in the current direction, then start moving towards it
                _nextCoord = Coordinate + Direction;
                //_endPosition = gm.CellToWorld(Coordinate + Direction);
            } else {
                _timer = _Delay;
            }
            _currentCoord = Coordinate;
            _lastCoord = _currentCoord;
            //_startPosition = transform.position; // update startPosition
            //_lastNode = gm.GetNode(Coordinate);
            UpdateDirectionAnimation();
        } else {
            Vector3 startPos = gm.CellToWorld(_currentCoord);
            Vector3 endPos = gm.CellToWorld(_nextCoord);
            // ++ Character is moving toward the cell ++ //
            transform.position = Vector3.Lerp(startPos, endPos, _timer * speed);
            //Debug.Log(name + " coords: " + Coordinate.ToString() + "; position: " + transform.position.ToString() + "; startPos: " + _startPosition.ToString() + "; endPos: " + _endPosition.ToString());
            Node nodeAtStart;
            if (gm.GetNode(Coordinate) == null && (nodeAtStart = gm.GetNode(_currentCoord)).Type == NodeType.Tunnel) {
                NodeTunnel nt1 = nodeAtStart as NodeTunnel;
                NodeTunnel nt2 = nt1.LinkedTunnel;
                Vector2Int newStartCoord = nt2.Coordinate + Vector2Int.left * (int)Mathf.Sign(nt1.Coordinate.x - nt2.Coordinate.x);
                Vector3 newStartPos = gm.CellToWorld(newStartCoord);
                transform.position -= startPos - newStartPos;
                _currentCoord = newStartCoord;
                _nextCoord = nt2.Coordinate;
                //_startPosition = c2;
                //_endPosition = gm.CellToWorld(nt2.Coordinate);
            }
        }
    }

    public void SetReady() {
        _lastCoord = Coordinate;
        _currentCoord = Coordinate;
        _nextCoord = Coordinate;
        //_endPosition = GameManager.Instance.CellToWorld(Coordinate);
        _timer = _Delay;
        MakeAlive();
    }

    void UpdateDirectionAnimation() {
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

    public void MakeDead() {
        animator.speed = 1f;
        animator.SetBool("Dead", true);
    }

    public void MakeAlive() {
        animator.speed = speed;
        animator.SetBool("Dead", false);
    }

    protected abstract void ChooseDirection();

}
