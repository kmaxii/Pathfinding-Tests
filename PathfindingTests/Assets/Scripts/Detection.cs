using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Visualization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Detection : MonoBehaviour
{
    [Header("Algorithms")]
    [SerializeField] private List<DetectionBehaviour.DetectionBehaviour> detectionBehaviours;
    [SerializeField] private ResolutionBehaviour.ResolutionBehaviour resolutionBehaviour;
    [SerializeField] private GridVisualizer gridVisual;
    
    [Header("Simulation settings")]
    [Tooltip("If an algorithm takes more than this time, cancel it for future runs")]
    [SerializeField] private float cancelDurationFactor = 2;

    [SerializeField] private int updatesToAverage = 1;

    [Tooltip("The time the number that is written should be multiplied by to avoid E-... values.")]
    [SerializeField] private float timeMultiplier;

    private readonly HashSet<int> _stoppedAlgorithms = new();
    private readonly List<List<float[]>> _dataToWrite = new();
    

    [SerializeField] private MapData mapData;

    [SerializeField] private bool randomizeMap;
    [SerializeField] private MapGenerator mapGenerator;

    [SerializeField] private int runsOnEachMap = 1;
    [SerializeField] private int mapSizeIncrease = 0;
    [SerializeField] private int stopAtMaxSize = 1000;
     private int _currentRunsOnMap;
    
    private int _currentUpdate;
    private int totalRuns;

    private void Start()
    {
        // Initialize CSV file with dynamic headers based on the detection behaviours
        List<string> headers = new List<string> { "MapSize" };
        headers.AddRange(detectionBehaviours.Select(behaviour => behaviour.name + "_Time"));       
        headers.AddRange(detectionBehaviours.Select(behaviour => behaviour.name + "_ExploredNodes"));
        headers.AddRange(detectionBehaviours.Select(behaviour => behaviour.name + "_PathLength"));
        EnhancedCsvFileWriter.Initialize("DetectionData.csv", headers);
        
        for (int i = 0; i < 3; i++)
        {
            _dataToWrite.Add(new List<float[]>());
            for (int j = 0; j < updatesToAverage; j++)
            {
                _dataToWrite[i].Add(new float[detectionBehaviours.Count]);
      
            }
        }
        SetRandomStartAndEndPos();

        
    }

    void Update()
    {

        if (_stoppedAlgorithms.Count == detectionBehaviours.Count)
            Application.Quit();


        if (resetStart) {
            resetStart = false;
            SetRandomStartAndEndPos();
            
            _currentRunsOnMap++;
            if (_currentRunsOnMap == runsOnEachMap)
            {
                _currentRunsOnMap = 0;
                mapGenerator.width += mapSizeIncrease;
                
                if (mapGenerator.width > stopAtMaxSize)
                    Application.Quit();
                
                mapGenerator.seed = Random.Range(0, 10000);
                mapGenerator.GenerateMap();
            }
            Debug.LogError($"Update on a new map: {_currentRunsOnMap}/{totalRuns}! Map Size: {mapGenerator.width}");

            
        }

        //  Reset the grid
        gridVisual.ResetGrid();
        
        //  Visualize The grid
        gridVisual.VisualizeGrid();
        
        //  Run the algorithms
        RunAlgorithms();
        
        //  Visualize the start and end point
        gridVisual.VisualizeStartAndEndPoints();
        
        //  Update run amount for debugs
        totalRuns++;
        Debug.Log("CurrentUpdate!");
    }

    private void SetRandomStartAndEndPos() {
        do {
            mapData.startPos = new Vector2Int(Random.Range(0, mapData.map.GetLength(0)),
                Random.Range(0, mapData.map.GetLength(1)));
        } while (!mapData.CheckCoordinate(mapData.startPos.x, mapData.startPos.y));

        do {
            mapData.endPos = new Vector2Int(Random.Range(0, mapData.map.GetLength(0)),
                Random.Range(0, mapData.map.GetLength(1)));
        } while (!mapData.CheckCoordinate(mapData.endPos.x, mapData.endPos.y));
    }

    private void RunAlgorithms()
    {
        for (var i = 0; i < detectionBehaviours.Count; i++)
        {
            if (_stoppedAlgorithms.Contains(i))
            {
                continue;
            }

            RunDetectionAlgorithm(i);
            
        }
        _currentUpdate++;
        

        if (_currentUpdate != updatesToAverage)
            return;
        
        //  Set random start and end positions
        resetStart = true;


        // Reset variable
        _currentUpdate = 0;

        WriteToFile();
    }

    private bool resetStart = false;

    private void RunDetectionAlgorithm(int i)
    {
        var algorithm = detectionBehaviours[i];
        var startTime = Time.realtimeSinceStartup;
        
        var algoResult = algorithm.GetShortestPath(mapData.startPos, mapData.endPos);
        if (algoResult.Item1.Count > 0) {
            var algorithmTime = Time.realtimeSinceStartup - startTime;

            resolutionBehaviour.Resolve(algoResult.Item1);
            //Calcualte the actual length of the path
            var pathLength = 0f;
            var path = algoResult.Item1.ToArray();
            for (int j = 0; j < path.Length - 1; j++) {
                pathLength += Vector2.Distance(path[j], path[j+1]);
            }
        
            _dataToWrite[0][_currentUpdate][i] = algorithmTime;
            _dataToWrite[1][_currentUpdate][i] = algoResult.nodesExplored;
            _dataToWrite[2][_currentUpdate][i] = pathLength;
        }
        else {
            SetRandomStartAndEndPos();
            gridVisual.ResetGrid();
            gridVisual.VisualizeGrid();
            RunDetectionAlgorithm(i);
        }
    }

    private void WriteToFile()
    {
        List<string> toWrite = new List<string> { mapData.MapSize.ToString() };
        for (var i = 0; i < detectionBehaviours.Count; i++)
        {
            var algorithmTime = _dataToWrite[0].Sum(floats => floats[i]);

            algorithmTime /= _dataToWrite[0].Count;

            if (algorithmTime > cancelDurationFactor)
                _stoppedAlgorithms.Add(i);

            toWrite.Add((algorithmTime * timeMultiplier).ToString(CultureInfo.InvariantCulture));

        }
        
        for (var i = 0; i < detectionBehaviours.Count; i++)
        {
            var nodesExplored = _dataToWrite[1].Sum(num => num[i]);

            nodesExplored /= _dataToWrite[1].Count;

            toWrite.Add(nodesExplored.ToString(CultureInfo.InvariantCulture));
        }
        
        for (var i = 0; i < detectionBehaviours.Count; i++)
        {
            var pathLength = _dataToWrite[2].Sum(num => num[i]);

            pathLength /= _dataToWrite[2].Count;

            toWrite.Add(pathLength.ToString(CultureInfo.InvariantCulture));
        }
        
        
        
        
        

        // Use the EnhancedCsvFileWriter to add a row
        EnhancedCsvFileWriter.AddRow(toWrite);

        // Optionally, you can flush data to file after adding each row or at specific intervals
        EnhancedCsvFileWriter.FlushDataToFile();
    }
}
