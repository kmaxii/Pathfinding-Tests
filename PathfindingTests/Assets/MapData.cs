using System.Collections.Generic;
using System.Diagnostics;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "Custom/MapData")]
public class MapData : ScriptableObject
{
    public bool[,] Map;

    [SerializeField] private GameEvent setMap;

    [SerializeField] private bool ensureConnectivity;
    
    [HideInInspector] public Vector2Int startPos;
    [HideInInspector] public Vector2Int endPos;

    [FormerlySerializedAs("_width")] public int width;

    //Property to get the map size
    public int MapSize => Map.GetLength(0);

    //Method to check a coordinate
    public bool CheckCoordinate(int x, int y) {
        //Return false if the coordinate is out of bounds
        if (x < 0 || x >= width || y < 0 || y >= width) {
            return false;
        }
        return Map[x, y];
    }
    
 

    //Method to set the map to x size and percent chance for each to be false
    public void SetMap(int newWidth, float frequency, int seed) {
        //  Measure the time for this function to run
        Stopwatch functionTimer = new Stopwatch();
        functionTimer.Start();
        this.width = newWidth;
        Map = new bool[newWidth, newWidth];
        for (int x = 0; x < newWidth; x++) {
            for (int y = 0; y < newWidth; y++) {
                float val = Mathf.PerlinNoise(seed + x / frequency, seed +y / frequency);
                Map[x, y] = val < 0.5f;
            }
        }


        if (ensureConnectivity) {
            for (int i = 0; i < 5; i++) {
                EnsureConnectivity();
            }
        }

        //  debug.log the measured time
        functionTimer.Stop();
        Debug.Log("Map generation time: " + functionTimer.Elapsed.TotalSeconds);
        
        
        //x33 y 3-4
        
        //x4  y7
      //  Map[4, 7] = true;
      
      //X 43  //Y 34
          Map[43, 34] = true;
          
          //x 49 Y: 14
            Map[49, 14] = true;
            
            //48 Y: 42
            Map[48, 42] = true;
        
        setMap.Raise();
    }
    
    void EnsureConnectivity() {
        List<List<Vector2Int>> regions = FindDisconnectedRegions();

        Debug.Log("Regions: " + regions.Count);

        if (regions.Count > 1) {
            ConnectRegions(regions);
        }

        regions = FindDisconnectedRegions();
        Debug.Log("Regions after: " + regions.Count);
    }

    private List<List<Vector2Int>> FindDisconnectedRegions() {
        //  Measure the time for this function to run
        Stopwatch test = new Stopwatch();
        test.Start();
        bool[][] visited = new bool[width][];
        for (int index = 0; index < width; index++)
        {
            visited[index] = new bool[width];
        }

        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width; y++) {
                if (!visited[x][y] && Map[x, y]) {
                    List<Vector2Int> newRegion = new List<Vector2Int>();
                    Queue<Vector2Int> queue = new Queue<Vector2Int>();
                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x][y] = true;

                    while (queue.Count > 0) {
                        Vector2Int tile = queue.Dequeue();
                        newRegion.Add(tile);

                        // Check all four directions
                        foreach (var dir in new[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right}) {
                            Vector2Int next = tile + dir;
                            if (next.x >= 0 && next.x < width && next.y >= 0 && next.y < width) {
                                if (!visited[next.x][next.y] && Map[next.x, next.y]) {
                                    queue.Enqueue(next);
                                    visited[next.x][next.y] = true;
                                }
                            }
                        }
                    }

                    regions.Add(newRegion);
                }
            }
        }

        //  debug.log the measured time
        test.Stop();
        Debug.Log("Finding regions took: " + test.Elapsed.TotalSeconds);
        return regions;
    }

    void ConnectRegions(List<List<Vector2Int>> regions) {
        // Assuming regions is a list of lists, where each sublist represents a region,
        // and each region is a list of Vector2Ints that are the tiles in that region.

        // Step 1: Find the centroid of each region to simplify the connection process.
        List<Vector2Int> centroids = new List<Vector2Int>();
        foreach (var region in regions) {
            Vector2Int centroid = FindCentroid(region);
            centroids.Add(centroid);
        }

        // Step 2: Connect centroids using a heuristic for the Traveling Salesman Problem.
        // For simplicity, we'll connect each centroid to its nearest neighbor not yet connected.
        // Note: This does not ensure the shortest overall path but is a reasonable heuristic.
        List<(Vector2Int, Vector2Int)> connections = ConnectCentroids(centroids);

        // Step 3: Draw connections between regions.
        foreach (var connection in connections) {
            CreatePath(connection.Item1, connection.Item2);
        }
    }

    Vector2Int FindCentroid(List<Vector2Int> region) {
        int x = 0, y = 0;
        foreach (var point in region) {
            x += point.x;
            y += point.y;
        }

        return new Vector2Int(x / region.Count, y / region.Count);
    }

    List<(Vector2Int, Vector2Int)> ConnectCentroids(List<Vector2Int> centroids) {
        int n = centroids.Count;
        float[][] distances = new float[n][]; // Store distances between all pairs of centroids
        for (int index = 0; index < n; index++)
        {
            distances[index] = new float[n];
        }

        for (int i = 0; i < n; i++) {
            for (int j = i + 1; j < n; j++) {
                float distance = Vector2.Distance(centroids[i], centroids[j]);
                distances[i][j] = distance;
                distances[j][i] = distance; // Symmetric, so fill both [i, j] and [j, i]
            }
        }

        List<(Vector2Int, Vector2Int)> connections = new List<(Vector2Int, Vector2Int)>();
        bool[] connected = new bool[n]; // Tracks which centroids are connected
        connected[0] = true; // Start by marking the first centroid as connected
        int lastConnectedIndex = 0;

        // Repeat until all centroids are connected
        for (int step = 1; step < n; step++) {
            float minDistance = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < n; i++) {
                if (!connected[i]) {
                    if (distances[lastConnectedIndex][i] < minDistance) {
                        minDistance = distances[lastConnectedIndex][i];
                        closestIndex = i;
                    }
                }
            }

            if (closestIndex != -1) {
                connected[closestIndex] = true;
                connections.Add((centroids[lastConnectedIndex], centroids[closestIndex]));
                lastConnectedIndex = closestIndex; // Update last connected for the next iteration
            }
        }

        return connections;
    }
    
    private void CreatePath(Vector2Int closestTileA, Vector2Int closestTileB) {

        // Create a wider straight path between the two closest tiles
        Vector2Int direction = closestTileB - closestTileA;
        //Debug.Log("PosA: " + closestTileA + " | PosB: " + closestTileB);
        int steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        Vector2Int stepDir = new Vector2Int((int) Mathf.Sign(direction.x), (int) Mathf.Sign(direction.y));

        Vector2Int perpStepDir; // Perpendicular step direction for widening the path
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
            // If primarily moving horizontally, set perpendicular direction vertically
            perpStepDir = new Vector2Int(0, 1);
        }
        else {
            // If primarily moving vertically or diagonal, set perpendicular direction horizontally
            perpStepDir = new Vector2Int(1, 0);
        }

        for (int step = 0; step <= steps; step++) {
            Vector2Int baseCurrentTile = closestTileA + stepDir * step;

            var mapLength = Mathf.Sqrt(Map.Length);
            
            if (baseCurrentTile.x >= 0 && baseCurrentTile.x <= mapLength - 1) {
                if (baseCurrentTile.y >= 0 && baseCurrentTile.y <= mapLength - 1) {
                    if (step > mapLength / 50 && Map[baseCurrentTile.x, baseCurrentTile.y])
                        return;
                }
            }


            // Set the central tile as walkable
            SetTileWalkable(baseCurrentTile);

            // Widen the path by setting adjacent tiles as walkable
            // Adjust the range to make the path wider or narrower
            for (int offset = -1; offset <= 1; offset++) {
                Vector2Int currentTile = baseCurrentTile + perpStepDir * offset;
                SetTileWalkable(currentTile);
            }
        }
    }

    private void SetTileWalkable(Vector2Int tile) {
        if (tile.x >= 0 && tile.x < width && tile.y >= 0 &&
            tile.y < width) // Assuming _height is the height of your map
        {
            Map[tile.x, tile.y] = true; // Make the tile walkable
        }
    }

    public override string ToString() {
        string mapString = "";
        for (int i = 0; i < Map.GetLength(0); i++) {
            mapString += "\n";
            for (int j = 0; j < Map.GetLength(1); j++) {
                if (Map[i, j]) {
                    mapString += "_";
                }
                else {
                    mapString += "X";
                }
            }
        }

        return mapString;
    }
    
}