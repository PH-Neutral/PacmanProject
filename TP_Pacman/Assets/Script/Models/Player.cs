using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    public int Score {
        get; set;
    }

    Vector2Int _nextDir;

    protected override void DoEachUpdate() {
        //GameManager gm = GameManager.Instance;
        Vector2Int inputs = GetMoveInputs(); // get the player inputs related to movement
        Node nextNode = gm.GetNode(_nextCoord);
        if (nextNode == null && gm.GetNode(_currentCoord).Type == NodeType.Tunnel) {
            nextNode = (gm.GetNode(_currentCoord) as NodeTunnel).LinkedTunnel;
        }
        if(inputs != Vector2Int.zero) {
            // if player is pressing a move key
            if(nextNode.HasNeighbor(inputs)) {
                // if there is a cell in this direction, choose this direction
                Direction = inputs;
                _nextDir = Vector2Int.zero;
            }
        }

        if(!nextNode.HasNeighbor(Direction)) {
            // if cell in this direction is a wall, then stop moving
            Direction = Vector2Int.zero;
        } else if(inputs != Vector2Int.zero && !nextNode.HasNeighbor(inputs)) {
            _nextDir = inputs;
        }

        if (nextNode.HasNeighbor(_nextDir)) {
            // if cell in the prepared direction is not a wall, change direction
            Direction = _nextDir;
            _nextDir = Vector2Int.zero;
        }
    }

    protected override void DoWhenCellReached() {
        // do nothing
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
}
