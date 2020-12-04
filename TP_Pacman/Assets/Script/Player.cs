using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float timer = 0;
    float delay = 0.2f;
    bool moveLock = false;
    Vector2Int direction = Vector2Int.zero;
    Vector3 startPosition, endPosition;
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        Vector2Int inputs = new Vector2Int((int)inputHorizontal, (int)inputVertical);
        Vector2Int pCoord = Maze.GetGridCoordFromPosition(transform.position);
        if (inputs != Vector2Int.zero && Maze.Instance.GetCellType(pCoord + inputs) != NodeType.None)
        {
            direction = inputs;
        }
        Vector2Int nextCell = pCoord + direction;
        NodeType type = Maze.Instance.GetCellType(nextCell);
        if (type == NodeType.None)
        {
            direction = Vector2Int.zero;
        }
        Debug.Log("type: " + type + " inputs: " + inputs + " pCoords: " + pCoord);

        if (moveLock)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer -= delay;
                moveLock = false;
                transform.position = endPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, timer / delay);
            }
        }
        // change la position du joueur
        if (direction != Vector2Int.zero && !moveLock)
        {
            endPosition = Maze.GetWorldPositionFromGrid(nextCell);
            startPosition = transform.position;
            moveLock = true;
        }
    }
}
