using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;
    public static readonly string keyHighscore = "highscore";

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

    public Grid Grid {
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
    [SerializeField] Item prefabPacGum = null;
    [SerializeField] Item prefabFruit = null;
    [SerializeField] Item prefabSuperPacGum = null;
    [SerializeField] Vector2Int[] superPacGumPositions = null;
    Tilemap tilemapIgnoreItem = null, tilemapPath = null, tilemapWalls = null;
    Dictionary<Vector2Int, Node> gridNodes = null;
    Dictionary<Vector2Int, Item> gridItems = null;
    int highscore = 0;
    float _chrono;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        tilemapIgnoreItem = GameObject.Find("Tilemap_emptyAreas").GetComponent<Tilemap>();
        tilemapPath = GameObject.Find("Tilemap_paths").GetComponent<Tilemap>();
        tilemapWalls = GameObject.Find("Tilemap_walls").GetComponent<Tilemap>();
        GamePaused = true;
    }

    private void Start() {
        LoadHighscore();
        MenuManager.Instance.UpdateOverlay(0, 0, highscore, 0);
    }

    private void Update() {
        if (GamePaused) { return; }
        _chrono += Time.deltaTime;
        CheckCollisionWithItems();
        UpdateOverlay();
        CheckCollisionWithGhosts();
        CheckForVictory();
        CheckForGhostActivation();
    }

    void CheckCollisionWithItems() {
        // ++ Check collision between player and items ++ //
        Item item;
        if((item = GetItem(Player.Coordinate)) != null) {
            Player.Score += item.pointValue;
            // check the kind of item it is
            if(item.type != ItemType.Fruit) {
                RemainingBalls--;
                if (item.type == ItemType.SuperPacGum) {
                    MakeGhostsVulnerable();
                }
            }
            // remove gameobject and reference in the dictionary
            gridItems.Remove(Player.Coordinate);
            Destroy(item.gameObject);

        }
    }

    void CheckCollisionWithGhosts() {
        // ++ Check collision between player and ghosts ++ //
        for(int i = 0; i < Ghosts.Count; i++) {
            if(Player.Coordinate == Ghosts[i].Coordinate) {
                // if player is at same coordinate than ghost
                if(Ghosts[i].IsVulnerable) {
                    // if ghost can be eaten
                    /*/ calculate number of remaining ghosts and adapt score to give player
                    int nbNotVulnerableGhosts = Ghosts.Count;
                    for (int j=0; j<Ghosts.Count; j++) {
                        if (Ghosts[j].IsVulnerable) {
                            nbNotVulnerableGhosts--;
                        }
                    }
                    Player.Score += 100 * (int)Mathf.Pow(2, nbNotVulnerableGhosts); // 1st ghost is 100, 2nd is 200, 3rd is 400 and 4th 800
                    // doesn't work well if some ghosts haven't spawned yet when eating one */
                    Player.Score += 250;
                    Ghosts[i].MakeDead();
                } else {
                    // if ghost is not vulnerable
                    Player.MakeDead();
                    LoseGame();
                    break;
                }
            }
        }
    }

    void CheckForVictory() {
        // ++ Check if every ball has been eaten ++ //
        if(RemainingBalls <= 0) {
            WinGame();
        }
    }

    void CheckForGhostActivation() {
        float currentPercent = 1 - RemainingBalls / (float)MaxBalls;
        for(int i = 0; i < Ghosts.Count; i++) {
            // check the percentage of balls eaten and lerp on it to make ghosts spawn based on it
            float percentToActivate = Mathf.Lerp(0, ghostsFreeAtPercentBallsEaten, i / (float)(Ghosts.Count - 1));
            if(currentPercent >= percentToActivate && Ghosts[i].firstActivation) {
                Ghosts[i].IsWaiting = false;
                Ghosts[i].firstActivation = false;
            }
        }

    }

    void MakeGhostsVulnerable() {
        for (int i=0; i<Ghosts.Count; i++) {
            if (!Ghosts[i].IsWaiting) {
                // if ghost is at least leaving the spawn, make it vulnerable
                Ghosts[i].MakeVulnerable(true);
            }
        }
    }

    void UpdateOverlay()
    {
        MenuManager.Instance.UpdateOverlay(_chrono, Player.Score, highscore, RemainingBalls);
    }

    void WinGame() {
        // calculate the score multiplier based on the time it took to win the game
        float scoreMultiplier = 3f / Mathf.Pow(_chrono - 11, 0.5f) + 1; // 12sec = x4, then descending and nearing 1
        if(_chrono < 12) {
            scoreMultiplier = 4;
        }
        Player.Score = (int)(Player.Score * scoreMultiplier);
        OnGameEnds();
        MenuManager.Instance.ShowVictory();
    }

    void LoseGame() {
        OnGameEnds();
        MenuManager.Instance.ShowGameover();
    }

    void OnGameEnds() {
        UpdateOverlay();
        SaveHighscore();
        GamePaused = true;
    }

    public void StartGame() {
        LoadHighscore();

        InitializeGraph();
        AdaptCamera();
        SpawnItems();
        SpawnGhosts();
        SpawnPacman();

        _chrono = 0;
        GamePaused = false;
    }

    public void RestartGame()
    {
        // destroy all items still in the maze
        foreach (Item item in gridItems.Values)
        {
            Destroy(item.gameObject);
        }
        // destroy all ghosts
        for (int i=0; i<Ghosts.Count; i++) {
            Destroy(Ghosts[i].gameObject);
        }
        // destroy the player
        Destroy(Player.gameObject);

        StartGame();
    }

    void LoadHighscore() {
        highscore = PlayerPrefs.HasKey(keyHighscore) ? PlayerPrefs.GetInt(keyHighscore) : 0;
    }

    void SaveHighscore() {
        if (Player.Score > highscore) {
            highscore = Player.Score;
        }
        PlayerPrefs.SetInt(keyHighscore, highscore);
        PlayerPrefs.Save();
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
        return tilemapPath.GetCellCenterWorld((Vector3Int)gridPosition);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition) {
        return (Vector2Int)Grid.WorldToCell(worldPosition);
    }

    public Vector3 VectorCellToWorld(Vector2Int vectorGrid) => VectorCellToWorld((Vector3Int)vectorGrid);
    public Vector3 VectorCellToWorld(Vector3 vectorGrid) {
        return new Vector3(vectorGrid.x * Grid.cellSize.x, vectorGrid.y * Grid.cellSize.y, vectorGrid.z * Grid.cellSize.z);
    }

    public float DistanceCellToWorld(float distanceGrid) {
        return distanceGrid * Grid.cellSize.x;
    }

    /// <summary>
    /// Get the path of nodes from one coordinate to another.
    /// </summary>
    /// <param name="from">The starting coordinate.</param>
    /// <param name="to">The ending coordinate.</param>
    /// <returns>A list of all nodes that compose the path, in order from the next one to the end one. 
    /// Null if the end Node can't be reached or if one of the coordinate doesn't point to an existing node.</returns>
    public List<Node> GetPath(Vector2Int from, Vector2Int to) {
        Node startNode = GetNode(from);
        Node endNode = GetNode(to);
        if(startNode == null || endNode == null) {
            return null;
        }
        List<Node> path = null;
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startNode);
        startNode.Parent = null;
        startNode.Depth = 0;

        // iterate over every node once by propagating to neighboring nodes
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
        // Clean the nodes before next time
        List<Node> nodes = new List<Node>(gridNodes.Values);
        for(int i=0; i<nodes.Count; i++) {
            nodes[i].Clean();
        }
        // return the path
        return path;
    }

    /// <summary>
    /// Get the list of all nodes from the first one to the last Parent.
    /// </summary>
    /// <param name="node">The starting node.</param>
    /// <param name="path">The list to with the methode will append the Parent nodes.</param>
    /// <returns>An empty list if <b>node</b> is null.</returns>
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
        // I let this unused code stay because I thought it was pretty smart.*/
    }

    void SpawnItems() {
        gridItems = new Dictionary<Vector2Int, Item>();
        GameObject pool = CreatePool("Item Pool");
        if (superPacGumPositions != null) {
            // spawn superpacgums at specified positions if any
            for(int i = 0; i < superPacGumPositions.Length; i++) {
                SpawnItem(prefabSuperPacGum, superPacGumPositions[i], pool.transform);
            }
        }
        // spawn pacgums in remaining empty spaces except where they shouldn't be placed (set in a special tilemap)
        foreach(Vector2Int coord in gridNodes.Keys) {
            if(!gridItems.ContainsKey(coord) && !tilemapIgnoreItem.HasTile((Vector3Int)coord)) {
                SpawnItem(prefabPacGum, coord, pool.transform);
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
        // calculate the pathPoint to leave the spawner
        Vector3 path1 = Vector3.Lerp(bound1, bound2, 0.5f), path2 = path1 + VectorCellToWorld(ghostLeaveSpawnerVector), path3 = CellToWorld(WorldToCell(path2));
        for (int i=0; i<prefabGhosts.Length; i++) {
            // instantiate ghost at position between the 2 bounds based on the prefabGhost list order
            Ghost ghost = Instantiate(prefabGhosts[i], Vector3.Lerp(bound1, bound2, i / (float)(prefabGhosts.Length - 1)), Quaternion.identity, pool.transform);
            ghost.SetOutOfSpawnerPath(new List<Vector3> { path1, path2, path3 });
            Ghosts.Add(ghost);
        }
    }

    void SpawnPacman() {
        Vector3 spawnPoint = CellToWorld(playerSpawnCoord);
        Player = Instantiate(prefabPlayer, spawnPoint, Quaternion.identity);
    }

    /// <summary>
    /// Gets the gameobject child of this one with specified name or creates a new one.
    /// </summary>
    /// <param name="poolName">The gameobject's name to set or to search for.</param>
    /// <returns>The found or created gameobject.</returns>
    GameObject CreatePool(string poolName) {
        GameObject pool = GameObject.Find(poolName) ?? new GameObject(poolName);
        pool.transform.SetParent(transform);
        return pool;
    }

    /// <summary>
    /// Place the camera at a position where the maze is at the center of what is left of the screen without the overlay
    /// </summary>
    void AdaptCamera() {
        Camera mainCam = Camera.main;
        tilemapWalls.CompressBounds(); // put the bounds at the closest possible size where they can still contain every tile
        BoundsInt bounds = tilemapWalls.cellBounds;
        Vector3 cornerBotLeft = CellToWorld(new Vector2Int(bounds.xMin, bounds.yMin));
        Vector3 cornerTopRight = CellToWorld(new Vector2Int(bounds.xMax, bounds.yMax));
        // find the center of the maze by lerping at half the position between opposite corners
        Vector3 cameraPos = Vector3.Lerp(cornerBotLeft, cornerTopRight, 0.5f);
        cameraPos.x -= Grid.cellSize.x * 0.5f;
        cameraPos.y -= Grid.cellSize.y * 0.5f;
        cameraPos.z = Camera.main.transform.position.z;
        mainCam.orthographicSize = bounds.size.y * 0.5f * Grid.cellSize.y;

        // ++ adapt maze position with overlay ++ // 
        float mazeScreenPercent = 0.75f; // percentage of horizontal screen space allowed to the maze, (= 1 - overlayWidthPercent)
        cameraPos.x += (1 - mazeScreenPercent) * mainCam.orthographicSize * mainCam.aspect;

        mainCam.transform.position = cameraPos;
    }

}
