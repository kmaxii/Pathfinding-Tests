using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using ProceduralNoiseProject;
using UnityEngine;


[CreateAssetMenu(menuName = "Custom/MapData")]

public class MapData : ScriptableObject
{
    public bool[,] map;

    [SerializeField] private GameEvent setMap;
    
    [SerializeField] private bool ensureConnectivity;

    private int _width;

    //Property to get the map size
    public int MapSize => map.GetLength(0);

    //Method to check a coordinate
    public bool CheckCoordinate(int x, int y)
    {
        return map[x, y];
    }
    
    

    //Method to set the map to x size and percent chance for each to be false
    public void SetMap(int width, Noise noise)
    {
        _width = width;
        map = new bool[width, width];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                float val = noise.Sample2D(x, y);
                
           //     float perlinValue = Mathf.PerlinNoise(x / noiseScale, y / noiseScale);
                // This is a simplified logic; you'd adjust your thresholds based on your desired map features
                map[x, y] = val > 0.5f; // Adjust this logic for walkable vs. solid areas
            }
        }
        
        
        if (ensureConnectivity)
        {
            EnsureConnectivity();
        }
        
        setMap.Raise();

    }
    
    void EnsureConnectivity()
    {
        List<List<Vector2Int>> regions = FindDisconnectedRegions();
        
        Debug.Log("Regions: " + regions.Count);
        
        if (regions.Count > 1)
        {
            ConnectRegions(regions);
        }
        regions = FindDisconnectedRegions();
        Debug.Log("Regions after: " + regions.Count);


    }

     private List<List<Vector2Int>> FindDisconnectedRegions()
    {
        bool[,] visited = new bool[_width, _width];
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _width; y++)
            {
                if (!visited[x, y] && map[x, y])
                {
                    List<Vector2Int> newRegion = new List<Vector2Int>();
                    Queue<Vector2Int> queue = new Queue<Vector2Int>();
                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x, y] = true;

                    while (queue.Count > 0)
                    {
                        Vector2Int tile = queue.Dequeue();
                        newRegion.Add(tile);

                        // Check all four directions
                        foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                        {
                            Vector2Int next = tile + dir;
                            if (next.x >= 0 && next.x < _width && next.y >= 0 && next.y < _width)
                            {
                                if (!visited[next.x, next.y] && map[next.x, next.y])
                                {
                                    queue.Enqueue(next);
                                    visited[next.x, next.y] = true;
                                }
                            }
                        }
                    }

                    regions.Add(newRegion);
                }
            }
        }

        return regions;
    }

  private void ConnectRegions(List<List<Vector2Int>> regions)
    {
        int regionCount = regions.Count;
        if (regionCount <= 1) return; // No need to connect if only one or no regions

        // Step 1: Create all possible edges with distances between regions
        List<(int regionA, int regionB, float distance, Vector2Int pointA, Vector2Int pointB)> edges = new List<(int, int, float, Vector2Int, Vector2Int)>();
        for (int i = 0; i < regionCount; i++)
        {
            for (int j = i + 1; j < regionCount; j++)
            {
                float closestDistance = float.MaxValue;
                Vector2Int closestPointA = Vector2Int.zero, closestPointB = Vector2Int.zero;
                foreach (var pointA in regions[i])
                {
                    foreach (var pointB in regions[j])
                    {
                        float distance = Vector2Int.Distance(pointA, pointB);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestPointA = pointA;
                            closestPointB = pointB;
                        }
                    }
                }
                edges.Add((i, j, closestDistance, closestPointA, closestPointB));
            }
        }

        // Step 2: Sort edges by distance to prepare for MST
        edges.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Step 3: Find MST (Kruskal's algorithm simplified version)
        int[] parent = new int[regionCount]; // Track the parent of each node in the MST
        for (int i = 0; i < regionCount; i++) parent[i] = i; // Initially, each node is its own parent

        foreach (var edge in edges)
        {
            int rootA = Find(edge.regionA, parent);
            int rootB = Find(edge.regionB, parent);

            if (rootA != rootB) // If adding this edge does not form a cycle
            {
                parent[rootB] = rootA; // Connect the trees
                CreatePath(edge.pointA, edge.pointB); // Create path between the closest points of these regions
            }
        }
    }

    int Find(int i, int[] parent)
    {
        if (i != parent[i]) parent[i] = Find(parent[i], parent); // Path compression
        return parent[i];
    }
    private void CreatePath(Vector2Int closestTileA, Vector2Int closestTileB)
    {
        /*// Create a simple straight path between the two closest tiles
        Vector2Int direction = closestTileB - closestTileA;
        int steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        Vector2Int stepDir = new Vector2Int((int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y));

        for (int step = 0; step <= steps; step++)
        {
            Vector2Int currentTile = closestTileA + stepDir * step;
            if (currentTile.x >= 0 && currentTile.x < _width && currentTile.y >= 0 && currentTile.y < _width)
            {
                map[currentTile.x, currentTile.y] = true; // Make the path walkable
            }
        }*/
        
        // Create a wider straight path between the two closest tiles
        Vector2Int direction = closestTileB - closestTileA;
        int steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        Vector2Int stepDir = new Vector2Int((int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y));

        Vector2Int perpStepDir; // Perpendicular step direction for widening the path
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // If primarily moving horizontally, set perpendicular direction vertically
            perpStepDir = new Vector2Int(0, 1);
        }
        else
        {
            // If primarily moving vertically or diagonal, set perpendicular direction horizontally
            perpStepDir = new Vector2Int(1, 0);
        }

        for (int step = 0; step <= steps; step++)
        {
            Vector2Int baseCurrentTile = closestTileA + stepDir * step;
            // Set the central tile as walkable
            SetTileWalkable(baseCurrentTile);

            // Widen the path by setting adjacent tiles as walkable
            // Adjust the range to make the path wider or narrower
            for (int offset = -1; offset <= 1; offset++)
            {
                Vector2Int currentTile = baseCurrentTile + perpStepDir * offset;
                SetTileWalkable(currentTile);
            }
        }
    }
    private void SetTileWalkable(Vector2Int tile)
    {
        if (tile.x >= 0 && tile.x < _width && tile.y >= 0 && tile.y < _width) // Assuming _height is the height of your map
        {
            map[tile.x, tile.y] = true; // Make the tile walkable
        }
    }
    
    public override string ToString()
    {
        string mapString = "";
        for (int i = 0; i < map.GetLength(0); i++)
        {
            mapString += "\n";
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j])
                {
                    mapString += "_";
                }
                else
                {
                    mapString += "X";
                }
            }
        }

        return mapString;
    }
}