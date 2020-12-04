using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour {

    public static Maze Instance = null;

    public Vector3 MapScale {
        get { return pathGrid.cellSize; }
    }

    [SerializeField] Item prefabMarker = null;
    [SerializeField] Item prefabBall = null, prefabBonus = null;
    Dictionary<Vector2Int, Node> gridNodes = new Dictionary<Vector2Int, Node>();
    Dictionary<Vector2Int, Item> gridItems = new Dictionary<Vector2Int, Item>();

    Grid pathGrid, frameGrid;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        pathGrid = GameObject.Find("Grid_background").GetComponent<Grid>();
        frameGrid = GameObject.Find("Grid_foreground").GetComponent<Grid>();

        InitializeGraph();
        SpawnItems(prefabBonus);
        SpawnItems(prefabBall);

        AdaptCamera();
    }

    private void Start() {
    }

    void InitializeGraph() {
        // +++ Generate Nodes +++ //
        Tilemap pathMap = pathGrid.GetComponentInChildren<Tilemap>(); // recover the tilemap
        pathMap.CompressBounds(); // get rid of empty rows and columns in the cellBounds
        BoundsInt bounds = pathMap.cellBounds;
        // iterate over the tilemap from the bottom left corner to the top right corner
        for(int x = bounds.xMin; x < bounds.xMax; x++) {
            for(int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                if(pathMap.GetTile(gridPos) != null) {
                    // if there is a tile at these coordinates, create a node for it and add it to the appropriate dictionary
                    Vector2Int coord = new Vector2Int(gridPos.x, gridPos.y);
                    gridNodes[coord] = new Node(coord);
                }
            }
        }

        // +++ Link Nodes +++ //
        List<NodeTunnel> nodeTunnels = new List<NodeTunnel>();
        // prepare an array of the relative coordinates of adjacent nodes
        Vector2Int[] deltaCoords = new Vector2Int[4];
        deltaCoords[0] = Vector2Int.left;
        deltaCoords[1] = Vector2Int.right;
        deltaCoords[2] = Vector2Int.down;
        deltaCoords[3] = Vector2Int.up;
        // iterate over every node in the dictionary
        foreach(Vector2Int coord in gridNodes.Keys) {
            Node n = gridNodes[coord];
            // iterate over every adjacent node
            foreach (Vector2Int deltaCoord in deltaCoords) {
                Vector2Int c = coord + deltaCoord;
                if(gridNodes.ContainsKey(c)) {
                    // if the neighboring node exists at these coordinates, link it as a neighbor of the node
                    n.Neighbors.Add(gridNodes[c]);
                }
            }
            if (n.IsTunnel) {
                // if the node is a tunnel (only 1 neighbor) replace it to reflect it
                n = new NodeTunnel(n);
                nodeTunnels.Add(n as NodeTunnel);
            }
        }
        // iterate over every tunnel node and compare it with every other tunnel nodes
        foreach(NodeTunnel n1 in nodeTunnels) {
            gridNodes[n1.Coordinate] = n1;
            foreach(NodeTunnel n2 in nodeTunnels) {
                if (n2.LinkedNode == null && (n1.Coordinate.x == n2.Coordinate.x ^ n1.Coordinate.y == n2.Coordinate.y)) {
                    // if the other tunnel node has not yet been linked and is on the same line xor column, link them
                    n1.LinkedNode = n2;
                    n2.LinkedNode = n1;
                }
            }
        }

        // +++ Clean Graph +++ //
        List<Node> uselessNodes = new List<Node>();
        foreach(Node n in gridNodes.Values) {
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
            gridNodes[n.Coordinate] = null;
        }
    }

    void SpawnItems(Item prefabItem) {
        if (prefabItem == null) { return; }
        GameObject pool = new GameObject(prefabItem.name + " Pool");
        pool.transform.SetParent(transform);
        foreach(Vector2Int coord in gridNodes.Keys) {
            if (!gridItems.ContainsKey(coord)) {
                if (gridNodes[coord] == null || (gridNodes[coord] != null && !gridNodes[coord].IsTunnel)) {
                    SpawnItem(prefabItem, coord, pool.transform);
                }
            }
        }
    }

    void SpawnItem(Item prefabItem, Vector2Int coord, Transform pool) {
        Item item = Instantiate(prefabItem, GetWorldPositionFromGrid(coord), Quaternion.identity, pool);
        item.name = prefabItem.name + " " + coord.ToString();
        gridItems[coord] = item;
    }

    public NodeType GetCellType(Vector2Int coordinate) {
        if(gridNodes.TryGetValue(coordinate, out Node node)) {
            if(node == null) { return NodeType.Hallway; }
            if(node.IsTunnel) { return NodeType.Tunnel; }
            if(node.IsCrossroad) { return NodeType.Crossroad; }
            if(node.IsCorner) { return NodeType.Corner; }
        }
        return NodeType.None;
    }

    public Node GetCell(Vector2Int coordinate) {
        return gridNodes[coordinate];
    }

    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPosition) {
        return new Vector3((gridPosition.x + 0.5f) * MapScale.x, (gridPosition.y + 0.5f) * MapScale.y) + transform.position;
    }
    
    public Vector2Int GetGridCoordFromPosition(Vector3 worldPosition) {
        worldPosition -= transform.position;
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x / MapScale.x), Mathf.FloorToInt(worldPosition.y / MapScale.y));
    }

    void AdaptCamera() {
        Tilemap map = frameGrid.GetComponentInChildren<Tilemap>();
        map.CompressBounds();
        BoundsInt bounds = map.cellBounds;
        //Vector3 xMin = GetWorldPositionFromGrid(new Vector2Int(bounds.xMin, 0));
        Vector3 xMax = GetWorldPositionFromGrid(new Vector2Int(bounds.xMax, 0));
        Vector3 yMin = GetWorldPositionFromGrid(new Vector2Int(0, bounds.yMin));
        Vector3 yMax = GetWorldPositionFromGrid(new Vector2Int(0, bounds.yMax));
        Vector3 newPos = Vector3.Lerp(xMax, yMax, 0.5f);
        newPos.x -= frameGrid.cellSize.x * 0.5f;
        newPos.y -= frameGrid.cellSize.y * 0.5f;
        newPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newPos;
        Camera.main.orthographicSize = Vector3.Distance(yMin, yMax) * 0.5f;
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
