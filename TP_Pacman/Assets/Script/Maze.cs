using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour {
    const int X_MIN = 0, X_MAX = 40, Y_MIN = 0, Y_MAX = 40;
    const float POS_OFFSET = 0.5f, MAP_SCALE = 0.32f;

    public static Maze Instance = null;

    [SerializeField] GameObject prefabMarker = null;
    Dictionary<Vector2Int, Node> grid;
    Transform goDebug;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        goDebug = transform.GetChild(transform.childCount - 1);
    }

    private void Start() {
        InitializeGrid();
        IdentifyNodes();
    }

    void InitializeGrid() {
        grid = new Dictionary<Vector2Int, Node>();
        // +++ Generate Nodes +++ //
        Tilemap map = GameObject.Find("Tilemap_background").GetComponent<Tilemap>();
        for(int x = X_MIN; x <= X_MAX; x++) {
            for(int y = Y_MIN; y <= Y_MAX; y++) {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                try {
                    TileBase tile = map.GetTile(gridPos);
                    if(tile != null) {
                        Vector2Int coord = new Vector2Int(gridPos.x, gridPos.y);
                        grid[coord] = new Node("Node " + coord.ToString(), coord);
                        // DEBUG
                        //GameObject marker = Instantiate(prefabMarker, GetWorldPositionFromGrid(gridPos), Quaternion.identity, goDebug);
                        //marker.name += " (" + x + "," + y + ")";
                    }
                } catch(Exception e) {
                    //Debug.LogWarning("Exception caught: " + e.Message);
                }
            }
        }

        // +++ Link Nodes +++ //
        Vector2Int[] deltaCoords = new Vector2Int[4];
        deltaCoords[0] = Vector2Int.left;
        deltaCoords[1] = Vector2Int.right;
        deltaCoords[2] = Vector2Int.down;
        deltaCoords[3] = Vector2Int.up;
        foreach(Vector2Int coord in grid.Keys) {
            Node n = grid[coord];
            foreach (Vector2Int deltaCoord in deltaCoords) {
                Vector2Int c = coord + deltaCoord;
                if(grid.ContainsKey(c)) {
                    n.Neighbors.Add(grid[c]);
                }
            }
        }

        // +++ Clean Graph +++ //
        List<Node> uselessNodes = new List<Node>();
        foreach(Node n in grid.Values) {
            Node[] neighbors = n.Neighbors.ToArray();
            if(n.IsHallway) {
                uselessNodes.Add(n);
                int index0 = neighbors[0].Neighbors.IndexOf(n);
                neighbors[0].Neighbors[index0] = neighbors[1];
                int index1 = neighbors[1].Neighbors.IndexOf(n);
                neighbors[1].Neighbors[index1] = neighbors[0];
            }
        }
        foreach(Node n in uselessNodes) {
            grid[n.Coordinate] = null;
        }
    }

    void IdentifyNodes() {
        foreach(Vector2Int coord in grid.Keys) {
            GameObject marker = Instantiate(prefabMarker, GetWorldPositionFromGrid(coord), Quaternion.identity, goDebug);
            marker.name = grid[coord].Name;
        }
    }

    public NodeType GetCellType(Vector2Int coordinate) {
        if(grid.TryGetValue(coordinate, out Node node)) {
            if(node == null) { return NodeType.Hallway; }
            if(node.IsTunnel) { return NodeType.Tunnel; }
            if(node.IsCrossroad) { return NodeType.Crossroad; }
            if(node.IsCorner) { return NodeType.Corner; }
        }
        return NodeType.None;
    }

    static Vector3 GetWorldPositionFromGrid(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + POS_OFFSET, gridPosition.y + POS_OFFSET) * MAP_SCALE;
    }
    
    static Vector2Int GetGridCoordFromPosition(Vector3 worldPosition) {
        return new Vector2Int((int)(worldPosition.x / MAP_SCALE), (int)(worldPosition.y / MAP_SCALE));
    }

    /*
static List<Node> GetPath(Node startNode, Node endNode) {
    Queue<Node> queue = new Queue<Node>();
    queue.Enqueue(startNode);
    startNode.Parent = null;
    startNode.Depth = 0;

    while(queue.Count > 0) {
        Node node = queue.Dequeue();
        foreach(Node n in node.Neighbors) {
            if(n.Depth < 0) {
                queue.Enqueue(n);
                n.Depth = node.Depth + 1;
                n.Parent = node;
                if(n == endNode) {
                    return GetParentPath(n);
                }
            }
        }
    }
    return null;
}

static List<Node> GetParentPath(Node node, List<Node> path = null) {
    if(path == null) { path = new List<Node>(); }
    if(node == null) {
        return path;
    }
    path.Add(node);
    return GetParentPath(node.Parent, path);
}
*/
}
