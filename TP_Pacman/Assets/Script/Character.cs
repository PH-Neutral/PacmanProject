using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
    public Vector2Int Coordinate {
        get { 
            return Maze.Instance.GetCoordFromWorldPosition(transform.position);
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

    protected Vector2Int _direction;
    protected Vector3 _startPosition, _endPosition;

    float _Delay {
        get { return 1f / speed; }
    }
    float _timer = 0f;

    private void Start() {
        SetReady();
    }

    protected virtual void Update() {

        Maze maze = Maze.Instance;
        _timer += Time.deltaTime;
        if(_timer >= _Delay) {
            _timer -= _Delay;
            transform.position = _endPosition;
            // ++ Character is now at center of the cell ++ //
            if (_startPosition != _endPosition) {
                // if endPosition was updated (meaning: character is not idle)
                maze.UpdateCharacterPosition(this); // update new position in maze
            }

            ChooseDirection(); // choose the direction to move towards

            if(maze.GetNode(Coordinate).HasNeighbor(Direction)) {
                // if there is a cell in the current direction, then start moving towards it
                _endPosition = maze.GetWorldPositionFromCoord(Coordinate + Direction);
            } else {
                _timer = _Delay;
            }
            _startPosition = transform.position; // update startPosition
        } else {
            // ++ Character is moving toward the cell ++ //
            transform.position = Vector3.Lerp(_startPosition, _endPosition, _timer * speed);
            //Debug.Log(name + " coords: " + Coordinate.ToString() + "; position: " + transform.position.ToString() + "; startPos: " + _startPosition.ToString() + "; endPos: " + _endPosition.ToString());
            if (maze.GetNode(Coordinate) == null && maze.GetNode(maze.GetCoordFromWorldPosition(_startPosition)).Type == NodeType.Tunnel) {
                NodeTunnel nt1 = maze.GetNode(maze.GetCoordFromWorldPosition(_startPosition)) as NodeTunnel;
                NodeTunnel nt2 = nt1.LinkedTunnel;
                Vector3 c1 = _startPosition;
                Vector3 c2 = maze.GetWorldPositionFromCoord(nt2.Coordinate + Vector2Int.left * (int)Mathf.Sign(nt1.Coordinate.x - nt2.Coordinate.x));
                transform.position = transform.position - (c1 - c2);
                _startPosition = c2;
                _endPosition = maze.GetWorldPositionFromCoord(nt2.Coordinate);
            }
        }
    }

    public void SetReady() {
        _endPosition = Maze.Instance.GetWorldPositionFromCoord(Coordinate);
        _timer = _Delay;
    }

    protected abstract void ChooseDirection();
}
