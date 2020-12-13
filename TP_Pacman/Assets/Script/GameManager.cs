using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    public bool GamePaused {
        get; set;
    }
    public int RemainingBalls {
        get; private set;
    }
    public int MaxBalls {
        get; private set;
    }
    public Player Player {
        get; private set;
    }

    Grid Grid {
        get { return tilemapPath.layoutGrid; }
    }
    List<Ghost> Ghosts {
        get; set;
    }
    [SerializeField] Player prefabPlayer = null;
    [SerializeField] Vector2Int playerSpawnCoord = Vector2Int.zero;
    [SerializeField] Ghost[] prefabGhosts = null;
    [SerializeField] Vector2Int[] ghostSpawnBounds = new Vector2Int[2];
    [SerializeField] Vector2Int ghostLeaveSpawnerVector = Vector2Int.up * 2;
    [SerializeField] float ghostsFreeAtPercentBallsEaten = 0.3f;
    [SerializeField] Item prefabBall = null;
    Tilemap tilemapItem = null, tilemapPath = null, tilemapWalls = null;
    Dictionary<Vector2Int, Node> gridNodes = null;
    Dictionary<Vector2Int, Item> gridItems = null;
    private float _chrono;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        tilemapItem = GameObject.Find("Tilemap_emptyAreas").GetComponent<Tilemap>();
        tilemapPath = GameObject.Find("Tilemap_paths").GetComponent<Tilemap>();
        tilemapWalls = GameObject.Find("Tilemap_walls").GetComponent<Tilemap>();

        //InitializeGraph();
        //SpawnItems(prefabBonus);
        //SpawnBalls();

        //AdaptCamera();

        GamePaused = true;
    }

    private void Start() {
    }

    private void Update() {
        if (GamePaused) { return; }
        _chrono += Time.deltaTime;

        // ++ Check collision between player and balls ++ //
        Item item;
        if((item = GetItem(Player.Coordinate)) != null) {
            // if there is an item at these coords
            // >>> check what kind of item it is
            gridItems.Remove(Player.Coordinate);
            Destroy(item.gameObject);
            Player.Score++;
            RemainingBalls--;

        }
        UpdateOverlay();

        // ++ Check collision between player and ghosts ++ //
        for(int i = 0; i < Ghosts.Count; i++) {
            if(Player.Coordinate == Ghosts[i].Coordinate) {
                if (Ghosts[i].IsVulnerable) {
                    Ghosts[i].MakeDead();
                } else {
                    Player.MakeDead();
                    LoseGame();
                }
            }
        }

        // ++ Check if every ball has been eaten ++ //
        if (RemainingBalls <= 0)
        {
            WinGame();
        }

        CheckForGhostActivation();
    }

    void UpdateOverlay()
    {
        MenuManager.Instance.Update_Overlay(_chrono, Player.Score, RemainingBalls);
    }

    void WinGame() {
        MenuManager.Instance.ShowVictory();
        GamePaused = true;
    }

    void LoseGame() {
        MenuManager.Instance.ShowGameover();
        GamePaused = true;
    }

    public void StartGame() {
        InitializeGraph();
        AdaptCamera();
        SpawnBalls();
        SpawnGhosts();
        SpawnPacman();

        _chrono = 0;
        GamePaused = false;
    }

    public void RestartGame()
    {
        foreach (Item item in gridItems.Values)
        {
            Destroy(item.gameObject);
        }
        for (int i=0; i<Ghosts.Count; i++) {
            Destroy(Ghosts[i].gameObject);
        }
        Destroy(Player.gameObject);

        StartGame();
    }

    void CheckForGhostActivation() {
        //float maxPercent = MaxBalls * ghostsFreeAtPercentBallsEaten;
        float currentPercent = 1 - RemainingBalls / (float)MaxBalls;
        for(int i=0; i<Ghosts.Count; i++) {
            float percentToActivate = Mathf.Lerp(0, ghostsFreeAtPercentBallsEaten, i / (float)(Ghosts.Count - 1));
            if (currentPercent >= percentToActivate) {
                Ghosts[i].IsWaiting = false;
            }
            //Debug.Log("percentToActivate = " + percentToActivate + "; currentPercent = " + currentPercent);
        }

    }

    public Node GetNode(Vector2Int coordinate) {
        if(!gridNodes.ContainsKey(coordinate)) {
            return null;
        }
        return gridNodes[coordinate];
    }

    public Item GetItem(Vector2Int coordinate) {
        if(!gridItems.ContainsKey(coordinate)) {
            return null;
        }
        return gridItems[coordinate];
    }

    public Vector3 CellToWorld(Vector2Int gridPosition) {
        return tilemapPath.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        //return new Vector3((gridPosition.x + 0.5f) * MapScale.x, (gridPosition.y + 0.5f) * MapScale.y) + transform.position;
    }

    public Vector2Int WorldToCell(Vector3 worldPosition) {
        return (Vector2Int)Grid.WorldToCell(worldPosition);
        //return new Vector2Int(Mathf.FloorToInt(worldPosition.x / MapScale.x - MapScale.x * 0.5f), Mathf.FloorToInt(worldPosition.y / MapScale.y - MapScale.y * 0.5f));
    }

    public Vector3 DistanceCellToWorld(Vector2Int vectorGrid) {
        return new Vector3(vectorGrid.x * Grid.cellSize.x, vectorGrid.y * Grid.cellSize.y);
    }

    public List<Node> GetPath(Vector2Int from, Vector2Int to) {
        //Debug.Log("from: " + from.ToString() + " to: " + to.ToString());
        Node startNode = GetNode(from);
        //Debug.Log("startNode: " + startNode.ToString());
        Node endNode = GetNode(to);
        //Debug.Log("endNode: " + endNode.ToString());
        if(startNode == null) {
            Debug.Log("startNode is NULL for coord: " + from.ToString());
        }
        if(endNode == null) {
            Debug.Log("endNode is NULL for coord: " + to.ToString());
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

    void InitializeGraph() {
        // +++ Generate Nodes +++ //
        gridNodes = new Dictionary<Vector2Int, Node>();
        tilemapPath.CompressBounds(); // get rid of empty rows and columns in the cellBounds
        BoundsInt bounds = tilemapPath.cellBounds;
        // iterate over the tilemap from the bottom left corner to the top right corner
        for(int x = bounds.xMin; x < bounds.xMax; x++) {
            for(int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                if(tilemapPath.GetTile(gridPos) != null) {
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
            for(int i=0; i<PacTools.v2IntDirections.Length; i++) {
                Vector2Int c = coord + PacTools.v2IntDirections[i];
                if(gridNodes.ContainsKey(c)) {
                    // if the neighboring node exists at these coordinates, link it as a neighbor of the node
                    n.Neighbors.Add(gridNodes[c]);
                }
            }
            if(n.Neighbors.Count == 0) {
                Debug.LogError("Lonely Node found at " + coord.ToString());
            } else if(n.Neighbors.Count == 1) {
                // if the node is a tunnel, replace it to reflect it
                n = new NodeTunnel(n);
                nodeTunnels.Add(n as NodeTunnel);
            } else if(n.Neighbors.Count == 2) {
                if((n.Neighbors[0].Coordinate.x == n.Neighbors[1].Coordinate.x) || (n.Neighbors[0].Coordinate.y == n.Neighbors[1].Coordinate.y)) {
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
            if(n1.LinkedTunnel == null) {
                // if this tunnel has not yet been linked
                foreach(NodeTunnel n2 in nodeTunnels) {
                    if(n2.LinkedTunnel == null && (n1.Coordinate.y == n2.Coordinate.y)) {
                        // if the other tunnel node has not yet been linked and is on the same line
                        n1.LinkTunnel(n2); // link them together
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

    void SpawnBalls() {
        gridItems = new Dictionary<Vector2Int, Item>();
        GameObject pool = CreatePool(prefabBall.name + " Pool");
        foreach(Vector2Int coord in gridNodes.Keys) {
            if(!gridItems.ContainsKey(coord) && !tilemapItem.HasTile(new Vector3Int(coord.x, coord.y, 0))) {
                SpawnItem(prefabBall, coord, pool.transform);
            }
        }
        RemainingBalls = MaxBalls = gridItems.Count;
    }

    void SpawnItem(Item prefabItem, Vector2Int coord, Transform pool) {
        Item item = Instantiate(prefabItem, CellToWorld(coord), Quaternion.identity, pool);
        item.name = prefabItem.name + " " + coord.ToString();
        gridItems[coord] = item;
    }

    void SpawnGhosts() {
        Ghosts = new List<Ghost>();
        GameObject pool = CreatePool("Ghost Pool");
        Vector3 bound1 = CellToWorld(ghostSpawnBounds[0]), bound2 = CellToWorld(ghostSpawnBounds[1]);
        Vector3 path1 = Vector3.Lerp(bound1, bound2, 0.5f), path2 = path1 + DistanceCellToWorld(ghostLeaveSpawnerVector), path3 = CellToWorld(WorldToCell(path2));
        for (int i=0; i<prefabGhosts.Length; i++) {
            Ghost ghost = Instantiate(prefabGhosts[i], Vector3.Lerp(bound1, bound2, i / (float)(prefabGhosts.Length - 1)), Quaternion.identity, pool.transform);
            ghost.SetOutOfSpawnerPath(new List<Vector3> { path1, path2, path3 });
            Ghosts.Add(ghost);
        }
    }

    void SpawnPacman() {
        Vector3 spawnPoint = CellToWorld(playerSpawnCoord);
        Player = Instantiate(prefabPlayer, spawnPoint, Quaternion.identity);
    }

    GameObject CreatePool(string poolName) {
        GameObject pool = GameObject.Find(poolName) ?? new GameObject(poolName);
        pool.transform.SetParent(transform);
        return pool;
    }

    void AdaptCamera() {
        Camera mainCam = Camera.main;
        tilemapWalls.CompressBounds();
        BoundsInt bounds = tilemapWalls.cellBounds;
        Vector3 cornerBotLeft = CellToWorld(new Vector2Int(bounds.xMin, bounds.yMin));
        Vector3 cornerTopRight = CellToWorld(new Vector2Int(bounds.xMax, bounds.yMax));
        Vector3 cameraPos = Vector3.Lerp(cornerBotLeft, cornerTopRight, 0.5f);
        cameraPos.x -= Grid.cellSize.x * 0.5f;
        cameraPos.y -= Grid.cellSize.y * 0.5f;
        cameraPos.z = Camera.main.transform.position.z;
        //Debug.Log("cornerBotLeft: " + cornerBotLeft + "; cornerTopRight: " + cornerTopRight + "; bounds.size: " + bounds.size);
        mainCam.orthographicSize = bounds.size.y * 0.5f * Grid.cellSize.y;

        // ++ adapt maze position with overlay ++ // 
        float mazeScreenPercent = 0.75f; // percentage of horizontal screen space allowed to the maze 
        //float mazeHalfWidth = bounds.size.x * 0.5f * Grid.cellSize.x;
        cameraPos.x += (1 - mazeScreenPercent) * mainCam.orthographicSize * mainCam.aspect;
        //cameraPos.y += mainCam.orthographicSize;

        mainCam.transform.position = cameraPos;
    }

    /// <summary>
    /// Moves gameobject towards target point in world space with provided speed.
    /// </summary>
    /// <param name="targetPoint">The position in world space to move toward.</param>
    /// <param name="speed">The speed applied to the movement.</param>
    /// <returns>True when gameobject arrived at targetPoint.</returns>
    public bool UpdateMovementUntethered(Transform goTransform, Vector3 targetPoint, float speed) {
        float remainingDistance = Vector3.Distance(goTransform.position, targetPoint);
        goTransform.position = Vector3.Lerp(goTransform.position, targetPoint, Time.deltaTime * speed * Grid.cellSize.x / remainingDistance);
        return goTransform.position == targetPoint;
    }
}
