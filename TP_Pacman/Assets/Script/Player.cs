using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Score {
        get; set;
    }
    public Vector2Int Coordinate {
        get { return _coordinate; }
    }

    Vector2Int _direction, _coordinate;
    Vector3 _startPosition, _endPosition;
    readonly float _delay = 0.2f;
    float _timer = 0;
    bool _moveLock = false;

    void Start()
    {
        GameManager.Instance.player = this;
    }

    void Update()
    {
        Vector2Int inputs = GetInputs();
        if(!_moveLock) {
            _coordinate = Maze.Instance.GetGridCoordFromPosition(transform.position);
        }
        if (inputs.x != 0 && inputs.y != 0)
        {
            inputs.y = 0;
        }
        if(inputs != Vector2Int.zero && Maze.Instance.GetNode(_coordinate + inputs) != null) {
            _direction = inputs;
        }
        Vector2Int nextNode = _coordinate + _direction;
        if (Maze.Instance.GetNode(nextNode) == null)
        {
            _direction = Vector2Int.zero;
        }
        //Debug.Log("type: " + type + " inputs: " + inputs + " pCoords: " + pCoord);

        if(_direction != Vector2Int.zero && !_moveLock) {
            _startPosition = transform.position;
            _endPosition = Maze.Instance.GetWorldPositionFromGrid(nextNode);
            _moveLock = true;
        }

        if (_moveLock)
        {
            LinearMovementUpdate();
        }
    }

    Vector2Int GetInputs() {
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        return new Vector2Int(Mathf.RoundToInt(inputHorizontal), Mathf.RoundToInt(inputVertical));
    }

    void LinearMovementUpdate() {
        _timer += Time.deltaTime;
        if(_timer >= _delay) {
            _timer -= _delay;
            _moveLock = false;
            Maze.Instance.UpdatePlayerPosition();
            
        } else {
            transform.position = Vector3.Lerp(_startPosition, _endPosition, _timer / _delay);
        }
    }
}
