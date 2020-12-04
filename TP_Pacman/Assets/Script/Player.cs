using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Score {
        get; set;
    }

    float timer = 0;
    float delay = 0.2f;
    bool moveLock = false;
    Vector2Int direction = Vector2Int.zero, nextCell;
    Vector3 startPosition, endPosition;
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        Vector2Int inputs = new Vector2Int(Mathf.RoundToInt(inputHorizontal), Mathf.RoundToInt(inputVertical));
        Vector2Int pCoord = Maze.Instance.GetGridCoordFromPosition(transform.position);
        if (inputs != Vector2Int.zero && Maze.Instance.GetCellType(pCoord + inputs) != NodeType.None)
        {
            direction = inputs;
        }
        if (!moveLock) {
            nextCell = pCoord + direction;
        }
        NodeType type = Maze.Instance.GetCellType(nextCell);
        if (type == NodeType.None)
        {
            direction = Vector2Int.zero;
        }
        //Debug.Log("type: " + type + " inputs: " + inputs + " pCoords: " + pCoord);

        if(direction != Vector2Int.zero && !moveLock) {
            endPosition = Maze.Instance.GetWorldPositionFromGrid(nextCell);
            startPosition = transform.position;
            moveLock = true;
        }

        if (moveLock)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer -= delay;
                moveLock = false;
                Vector2Int targetCoord;
                if(Maze.Instance.GetCellType(targetCoord = Maze.Instance.GetGridCoordFromPosition(endPosition)) == NodeType.Tunnel) {
                    NodeTunnel tNode = Maze.Instance.GetCell(targetCoord) as NodeTunnel;
                    Debug.Log("Node is " + (tNode == null ? "NOT " : "") + "a tunnel.");
                    endPosition = Maze.Instance.GetWorldPositionFromGrid(tNode.LinkedNode.Coordinate);
                }
                transform.position = endPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, timer / delay);
            }
        }
    }
}
