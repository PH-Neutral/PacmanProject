using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    List<Node> path;

    readonly float _delay = 0.2f;
    float _timer = 0f;

    Vector3 _startPosition, _endPosition;

    private void Start()
    {
        _timer = _delay;
    }
    private void Update() {
        _timer += Time.deltaTime;
        if (_timer >= _delay)
        {
            _timer -= _delay;
            Maze maze = Maze.Instance;
            Vector2Int selfCoordinate = maze.GetGridCoordFromPosition(transform.position);
            Vector2Int playerCoordinate = maze.GetPlayerGridPosition();
            path = maze.GetPath(selfCoordinate, playerCoordinate);
            Debug.Log("path lenght " + path.Count);
            _startPosition = transform.position = maze.GetWorldPositionFromGrid(selfCoordinate);
            _endPosition = Maze.Instance.GetWorldPositionFromGrid(path[0].Coordinate);
            //Maze.Instance.UpdatePlayerPosition();
        }
        else
        {
            transform.position = Vector3.Lerp(_startPosition, _endPosition, _timer / _delay);
        }
    }
}
