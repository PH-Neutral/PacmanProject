using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    List<Node> path;

    readonly float _delay = 0.2f;
    float _timer = 0f;

    private void Update() {
        Maze maze = Maze.Instance;
        Vector2Int selfCoordinate = maze.GetGridCoordFromPosition(transform.position);
        Vector2Int playerCoordinate = maze.GetPlayerGridPosition();
        _timer += Time.deltaTime;
        if (_timer >= _delay) {
            _timer -= _delay;
            // calculate path every 0.2f seconds
            path = maze.GetPath(selfCoordinate, playerCoordinate);
        }
    }
}
