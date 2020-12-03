using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour {
    const int X_MIN = 0, X_MAX = 40, Y_MIN = 0, Y_MAX = 40;
    const float POS_OFFSET = 0.5f, MAP_SCALE = 0.32f;

    [SerializeField] GameObject prefabMarker = null;
    Dictionary<Vector2Int, Node> grid;
    Transform goDebug;

    private void Awake() {
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
            if (neighbors.Length == 2) {
                //if (neighbors[0] == ) {
                    uselessNodes.Add(n);
                    int index0 = neighbors[0].Neighbors.IndexOf(n);
                    neighbors[0].Neighbors[index0] = neighbors[1];
                    int index1 = neighbors[1].Neighbors.IndexOf(n);
                    neighbors[1].Neighbors[index1] = neighbors[0];
                //}
            }
        }
        foreach(Node n in uselessNodes) {
            grid.Remove(n.Coordinate);
        }
    }

    void IdentifyNodes() {
        foreach(Vector2Int coord in grid.Keys) {
            GameObject marker = Instantiate(prefabMarker, GetWorldPositionFromGrid(coord), Quaternion.identity, goDebug);
            marker.name = grid[coord].Name;
        }
    }

    static Vector3 GetWorldPositionFromGrid(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + POS_OFFSET, gridPosition.y + POS_OFFSET) * MAP_SCALE;
    }

    /*
    static Vector3Int GetGridPositionFromWorld(Vector3 worldPosition) {
        return new Vector3Int(worldPosition.x, worldPosition.y) * MAP_SCALE;
    }*/

    /*
     

        public static void StartExample() {
            int[][] gsl = new int[][] {
                new int[] { 1, 2 },
                new int[] { 0, 3, 4 },
                new int[] { 0 },
                new int[] { 1 },
                new int[] { 1, 5, 6, 7 },
                new int[] { 4 },
                new int[] { 4 },
                new int[] { 4 } 
            };

            Node[] nodes = GenerateNodes(gsl);
    Console.WriteLine("Shortest path from Node" + nodes[0].Name + " to Node" + nodes[7].Name + " is " + GetPathLength(nodes[0], nodes[7]) + " unit(s) long.");
        }

static Node[] GetNodes(Node startNode) {
    Queue<Node> queue = new Queue<Node>();
    List<Node> result = new List<Node>();
    queue.Enqueue(startNode);
    startNode.Depth = 0;

    while(queue.Count > 0) {
        Node node = queue.Dequeue();
        result.Add(node);
        foreach(Node n in node.Neighbors) {
            if(n.Depth < 0) {
                queue.Enqueue(n);
                n.Depth = node.Depth + 1;
            }
        }
    }
    return result.ToArray();
}


static int GetPathLength(Node startNode, Node endNode) {
    Queue<Node> queue = new Queue<Node>();
    queue.Enqueue(startNode);
    startNode.Depth = 0;

    while(queue.Count > 0) {
        Node node = queue.Dequeue();
        foreach(Node n in node.Neighbors) {
            if(n.Depth < 0) {
                queue.Enqueue(n);
                n.Depth = node.Depth + 1;
                if(n == endNode) { return n.Depth; }
            }
        }
    }
    return -1;
}
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

static Node[] GenerateNodes(int[][] gslArray) {
    Node[] nodes = new Node[gslArray.Length];
    for(int i = 0; i < nodes.Length; i++) {
        nodes[i] = new Node("" + i);
    }
    for(int i = 0; i < nodes.Length; i++) {
        for(int j = 0; j < gslArray[i].Length; j++) {
            nodes[i].Neighbors.Add(nodes[gslArray[i][j]]);
        }
    }
    return nodes;
}
*/
}
