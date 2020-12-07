using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public int Score {
        get; set;
    }

    Vector2Int GetMoveInputs() {
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        Vector2Int inputs = new Vector2Int(Mathf.RoundToInt(inputHorizontal), Mathf.RoundToInt(inputVertical));
        if(inputs.x != 0 && inputs.y != 0) {
            inputs.y = 0;
        }
        return inputs;
    }

    protected override void ChooseDirection() {
        //Debug.Log(name + " is on " + Coordinate.ToString() + " and the node is " + (Maze.Instance.GetNode(Coordinate) == null ? "" : "NOT ") + "null");
        Maze maze = Maze.Instance;
        Vector2Int inputs = GetMoveInputs(); // get the player inputs related to movement
        if(inputs != Vector2Int.zero && maze.GetNode(Coordinate).HasNeighbor(inputs)) {
            // if player is pressing a move key and there is a cell in this direction, choose this direction
            Direction = inputs;
        }
        if (!maze.GetNode(Coordinate).HasNeighbor(Direction)) {
            Direction = Vector2Int.zero;
        }
        //Debug.Log("Direction prepared! New: " + Direction.ToString() + " based on inputs: " + inputs.ToString());
    }
}
