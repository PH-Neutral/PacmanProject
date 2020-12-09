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

    protected Vector2Int _direction;
    protected Vector3 _startPosition, _endPosition;
    protected Node _lastNode = null;

    float _Delay {
        get { return 1f / speed; }
    }
    float _timer = 0f;

    private void Start() {
        SetReady();
    }

    protected virtual void Update() {
        GameManager gm = GameManager.Instance;
        if (gm.GamePaused) { return; }
        _timer += Time.deltaTime;
        if(_timer >= _Delay) {
            _timer -= _Delay;
            // ++ check if character is in tunnel ++ //
            /*Node playerNode = gm.GetNode(Coordinate);
            if(playerNode.Type == NodeType.Tunnel) {
                // if player is on a tunnel
                _endPosition = gm.CellToWorld((playerNode as NodeTunnel).LinkedTunnel.Coordinate);
            }*/
            transform.position = _endPosition;
            // ++ Character is now at center of the cell ++ //
            Node currentNode = gm.GetNode(Coordinate);
            if (currentNode != _lastNode) {
                // if endPosition was updated (meaning: character is not idle)
                gm.UpdateCharacterPosition(this); // update new position in maze
            }

            ChooseDirection(); // choose the direction to move towards

            if(gm.GetNode(Coordinate).HasNeighbor(Direction)) {
                // if there is a cell in the current direction, then start moving towards it
                _endPosition = gm.CellToWorld(Coordinate + Direction);
            } else {
                _timer = _Delay;
            }
            _startPosition = transform.position; // update startPosition
            _lastNode = currentNode;
        } else {
            // ++ Character is moving toward the cell ++ //
            transform.position = Vector3.Lerp(_startPosition, _endPosition, _timer * speed);
            //Debug.Log(name + " coords: " + Coordinate.ToString() + "; position: " + transform.position.ToString() + "; startPos: " + _startPosition.ToString() + "; endPos: " + _endPosition.ToString());
            if (gm.GetNode(Coordinate) == null && gm.GetNode(gm.WorldToCell(_startPosition)).Type == NodeType.Tunnel) {
                NodeTunnel nt1 = gm.GetNode(gm.WorldToCell(_startPosition)) as NodeTunnel;
                NodeTunnel nt2 = nt1.LinkedTunnel;
                Vector3 c1 = _startPosition;
                Vector3 c2 = gm.CellToWorld(nt2.Coordinate + Vector2Int.left * (int)Mathf.Sign(nt1.Coordinate.x - nt2.Coordinate.x));
                transform.position = transform.position - (c1 - c2);
                _startPosition = c2;
                _endPosition = gm.CellToWorld(nt2.Coordinate);
            }
        }
    }

    public void SetReady() {
        _endPosition = GameManager.Instance.CellToWorld(Coordinate);
        _timer = _Delay;
    }

    protected abstract void ChooseDirection();
}
