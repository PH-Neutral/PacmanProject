using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maze : MonoBehaviour {

    public static Maze Instance = null;

    static readonly Vector2Int[] _directions = new Vector2Int[4] { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };

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
        // iterate over every node in the dictionary
        foreach(Vector2Int coord in gridNodes.Keys) {
            Node n = gridNodes[coord];
            // iterate over every adjacent node
            foreach (Vector2Int deltaCoord in _directions) {
                Vector2Int c = coord + deltaCoord;
                if(gridNodes.ContainsKey(c)) {
                    // if the neighboring node exists at these coordinates, link it as a neighbor of the node
                    n.Neighbors.Add(gridNodes[c]);
                }
            }
            if (n.Neighbors.Count == 0) {
                Debug.LogError("Lonely Node found at " + coord.ToString());
            } else if (n.Neighbors.Count == 1) {
                // if the node is a tunnel, replace it to reflect it
                n = new NodeTunnel(n);
                nodeTunnels.Add(n as NodeTunnel);
            } else if (n.Neighbors.Count == 2) {
                if ((n.Neighbors[0].Coordinate.x == n.Neighbors[1].Coordinate.x) || (n.Neighbors[0].Coordinate.y == n.Neighbors[1].Coordinate.y)) {
                    n.Type = NodeType.Hallway;
                } else {
                    n.Type = NodeType.Corner;
                }
            } else {
                n.Type = NodeType.Crossroad;
            }
        }
        // iterate over every tunnel node and compare it with every other tunnel nodes
        foreach(NodeTunnel n1 in nodeTunnels) {
            gridNodes[n1.Coordinate] = n1;
            if (n1.LinkedTunnel == null) {
                // if this tunnel has not yet been linked
                foreach(NodeTunnel n2 in nodeTunnels) {
                    if(n2.LinkedTunnel == null && (n1.Coordinate.y == n2.Coordinate.y)) {
                        // if the other tunnel node has not yet been linked and is on the same line
                        n1.LinkTunnel(n2); // link them together
                        /*/ duplicate nodes position for both tunnels
                        int sign = (int)Mathf.Sign(n1.Coordinate.x - n2.Coordinate.x);
                        Vector2Int n1DuplicateCoord = n2.Coordinate + Vector2Int.left * sign;
                        gridNodes[n1DuplicateCoord] = n1;
                        Vector2Int n2DuplicateCoord = n1.Coordinate + Vector2Int.right * sign;
                        gridNodes[n2DuplicateCoord] = n2;*/
                    }
                }
            }
        }

        /*/ +++ Clean Graph +++ //
        //List<Node> hallwayNodes = new List<Node>();
        List<Node> extremityNodes = new List<Node>();
        // iterate over every node and check if they are hallway
        foreach(Node n in gridNodes.Values) {
            Node[] neighbors = n.Neighbors.ToArray();
            if(n.IsHallway) {
                // if node is hallway, rewire neighbors so that in the end all corner/crossroad/tunnel nodes are only linked to each other
                //hallwayNodes.Add(n);
                int index0 = neighbors[0].Neighbors.IndexOf(n);
                neighbors[0].Neighbors[index0] = neighbors[1];
                int index1 = neighbors[1].Neighbors.IndexOf(n);
                neighbors[1].Neighbors[index1] = neighbors[0];
            }
        }
        // iterate over every non-hallway
        foreach(Node n in extremityNodes) {
            
        }*/
    }

    void SpawnItems(Item prefabItem) {
        if (prefabItem == null) { return; }
        GameObject pool = new GameObject(prefabItem.name + " Pool");
        pool.transform.SetParent(transform);
        foreach(Vector2Int coord in gridNodes.Keys) {
            if (!gridItems.ContainsKey(coord)) {
                if (GetNode(coord).Type != NodeType.Tunnel) {
                    SpawnItem(prefabItem, coord, pool.transform);
                }
            }
        }
    }

    void SpawnItem(Item prefabItem, Vector2Int coord, Transform pool) {
        Item item = Instantiate(prefabItem, GetWorldPositionFromCoord(coord), Quaternion.identity, pool);
        item.name = prefabItem.name + " " + coord.ToString();
        gridItems[coord] = item;
    }
    public List<Node> GetPath(Vector2Int from, Vector2Int to) {
        //Debug.Log("from: " + from.ToString() + " to: " + to.ToString());
        Node startNode = GetNode(from);
        //Debug.Log("startNode: " + startNode.ToString());
        Node endNode = GetNode(to);
        //Debug.Log("endNode: " + endNode.ToString());
        if(startNode == null) {
            Debug.Log("startNode is NULL");
        }
        if(endNode == null) {
            Debug.Log("endNode is NULL");
        }
        List<Node> path = null;
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
                        path = GetParentPath(n);
                        path.RemoveAt(path.Count - 1);
                        path.Reverse();
                    }
                }
            }
        }
        // Clean the nodes
        foreach(Node n in gridNodes.Values) {
            n.Clean();
        }
        // return the path
        return path;
    }

    List<Node> GetParentPath(Node node, List<Node> path = null) {
        if(path == null) { path = new List<Node>(); }
        if(node == null) {
            return path;
        }
        path.Add(node);
        return GetParentPath(node.Parent, path);
    }

    public void UpdateCharacterPosition(Character character) {
        //Debug.Log("UpdateCharacterPosition");
        // ++ check if character is in tunnel ++ //
        Node playerNode = GetNode(character.Coordinate);
        if(playerNode.Type == NodeType.Tunnel) {
            character.transform.position = GetWorldPositionFromCoord((playerNode as NodeTunnel).LinkedTunnel.Coordinate);
        }

        // +++ Player only +++ //
        if(character is Player player) {
            //Debug.Log("Character is player");
            // ++ check if player collides with a ghost ++ //
            foreach (Ghost g in GameManager.Instance.ghosts) {
                if (player.Coordinate == g.Coordinate) {
                    Debug.LogError("YA DEAD, BITCH.");
                }
            }
            // ++ check if player collides with an item ++ //
            Item item;
            if((item = GetItem(player.Coordinate)) != null) {
                // if there is an item at these coords
                // >>> check what kind of item it is
                gridItems.Remove(player.Coordinate);
                Destroy(item.gameObject);
                player.Score++;
                GameManager.Instance.RemainingBalls--;
            }
        }

        // +++ Ghost only +++ //
        else if (character is Ghost ghost) {
            // ++ check if ghost collides with player ++ //
            if(ghost.Coordinate == GameManager.Instance.player.Coordinate) {
                Debug.LogError("YA DEAD, BITCH.");
            }
        }
    }

    public Vector2Int GetPlayerGridPosition() {
        return GameManager.Instance.player.Coordinate;
    }

    /*public Node GetPlayerNode() {
        Player player = GameManager.Instance.players[0];
        Node pNode = gridNodes[player.Coordinate]; // get the node at player's coordinate
        if (pNode != null) { return pNode; } // if a node already exists there, return it
        pNode = new Node(player.Coordinate); // else, create a new one
        // check for every direction to find the adjacent nodes
        foreach(Vector2Int dir in _directions) {
            Node node;
            Vector2Int searchPos = player.Coordinate;
            if (!gridNodes.ContainsKey(searchPos + dir)) { continue; }
            do {
                searchPos += dir;
            } while((node = gridNodes[searchPos]) == null);
            for (int i = 0; i < )
            pNode.Neighbors.Add(node);
        }
    }*/

    /*public void ChangeNodePosition(Vector2Int lastPos, Vector2Int newPos){
        Node nodeLast = gridNodes[lastPos];
        Node nodeNew = gridNodes[newPos];
        if (nodeNew == null) {
            // if next cell is hallway
            if(nodeLast.IsHallway) {
                // if last cell is also hallway, then just move node to newPos
                gridNodes[newPos] = nodeLast;
                gridNodes[lastPos] = null;
            } else if(nodeLast.IsCrossroad || nodeLast.IsTunnel || nodeLast.IsCorner) {
                // if last cell has a permanent node, then create new node and relink neighbors
                // > setup new node
                Node newNode = new Node(newPos);
                newNode.Neighbors.Add(nodeLast);
                Vector2Int searchPos = newPos;
                Node nextNode;
                do {
                    searchPos += newPos - lastPos;
                } while((nextNode = gridNodes[newPos + searchPos]) == null); // search for next node in hallway
                newNode.Neighbors.Add(nextNode);
                // > relink neighbors

            }
        }
    }*/

    public Node GetNode(Vector2Int coordinate) {
        if (!gridNodes.ContainsKey(coordinate)) {
            return null;
        }
        return gridNodes[coordinate];
    }

    public Item GetItem(Vector2Int coordinate) {
        if (!gridItems.ContainsKey(coordinate)) {
            return null;
        }
        return gridItems[coordinate];
    }

    public Vector3 GetWorldPositionFromCoord(Vector2Int gridPosition) {
        return pathGrid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0)) + Vector3.one * 0.5f * MapScale.x;
        //return new Vector3((gridPosition.x + 0.5f) * MapScale.x, (gridPosition.y + 0.5f) * MapScale.y) + transform.position;
    }
    
    public Vector2Int GetCoordFromWorldPosition(Vector3 worldPosition) {
        Vector3Int coord3 = pathGrid.WorldToCell(worldPosition);
        return new Vector2Int(coord3.x, coord3.y);
        //worldPosition -= transform.position;
        //return new Vector2Int(Mathf.FloorToInt(worldPosition.x / MapScale.x - MapScale.x * 0.5f), Mathf.FloorToInt(worldPosition.y / MapScale.y - MapScale.y * 0.5f));
    }

    void AdaptCamera() {
        Tilemap map = frameGrid.GetComponentInChildren<Tilemap>();
        map.CompressBounds();
        BoundsInt bounds = map.cellBounds;
        //Vector3 xMin = GetWorldPositionFromGrid(new Vector2Int(bounds.xMin, 0));
        Vector3 xMax = GetWorldPositionFromCoord(new Vector2Int(bounds.xMax, 0));
        Vector3 yMin = GetWorldPositionFromCoord(new Vector2Int(0, bounds.yMin));
        Vector3 yMax = GetWorldPositionFromCoord(new Vector2Int(0, bounds.yMax));
        Vector3 newPos = Vector3.Lerp(xMax, yMax, 0.5f);
        newPos.x -= frameGrid.cellSize.x * 0.5f;
        newPos.y -= frameGrid.cellSize.y * 0.5f;
        newPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = newPos;
        Camera.main.orthographicSize = Vector3.Distance(yMin, yMax) * 0.5f;
    }

    
}
