using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Detection : MonoBehaviour
{
    [Header("Algorithms")]
    [SerializeField] private List<DetectionBehaviour.DetectionBehaviour> detectionBehaviours;
    [SerializeField] private ResolutionBehaviour.ResolutionBehaviour resolutionBehaviour;

    [Header("Simulation settings")]
    [Tooltip("If an algorithm takes more than this time, cancel it for future runs")]
    [SerializeField] private float cancelDurationFactor = 2;

    [SerializeField] private int updatesToAverage = 1;

    [Tooltip("The time the number that is written should be multiplied by to avoid E-... values.")]
    [SerializeField] private float timeMultiplier;

    [SerializeField] private Vector2Int startPos = new(1, 1);
    [SerializeField] private Vector2Int endPos = new(8, 8);

    public static readonly HashSet<Tuple<Vector2Int, Vector2Int>> Lines = new();
    private readonly HashSet<int> _stoppedAlgorithms = new();
    private List<List<float[]>> _dataToWrite = new();
    private List<float[]> _algoTimes = new();
    private List<float[]> _nodesExploredToWrite = new();
    [SerializeField] private MapData mapData;

    private int _currentUpdate;

    private void Start()
    {
        // Initialize CSV file with dynamic headers based on the detection behaviours
        List<string> headers = new List<string> { "MapSize" };
        foreach (var behaviour in detectionBehaviours)
        {
            headers.Add(behaviour.name + "_Time");
            headers.Add(behaviour.name + "_ExploredNodes");
            headers.Add(behaviour.name + "_PathLength");
        }
        EnhancedCsvFileWriter.Initialize("DetectionData.csv", headers);

        _algoTimes = new List<float[]>();
        _nodesExploredToWrite = new List<float[]>();
        for (int i = 0; i < 3; i++)
        {
            _dataToWrite.Add(new List<float[]>());
            for (int j = 0; j < updatesToAverage; j++)
            {
                _dataToWrite[i].Add(new float[detectionBehaviours.Count]);
      
            }
        }
        
        for (int j = 0; j < updatesToAverage; j++)
        {
            _algoTimes.Add(new float[detectionBehaviours.Count]);
            _nodesExploredToWrite.Add(new float[detectionBehaviours.Count]);
        }

    }

    void Update()
    {
        Lines.Clear();

        if (_stoppedAlgorithms.Count == detectionBehaviours.Count)
            Application.Quit();

        RunAlgorithms();
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

        // Reset variable
        _currentUpdate = 0;

        WriteToFile();
    }

    private void RunDetectionAlgorithm(int i)
    {
        var algorithm = detectionBehaviours[i];
        var startTime = Time.realtimeSinceStartup;

        var algoResult = algorithm.GetShortestPath(startPos, endPos);
        var algorithmTime = Time.realtimeSinceStartup - startTime;

        resolutionBehaviour.Resolve(algoResult.Item1);
        
        _dataToWrite[0][_currentUpdate][i] = algorithmTime;
        _dataToWrite[1][_currentUpdate][i] = algoResult.nodesExplored;
        _dataToWrite[2][_currentUpdate][i] = algoResult.Item1.Count;
        _algoTimes[_currentUpdate][i] = algorithmTime;
        _nodesExploredToWrite[_currentUpdate][i] = algoResult.nodesExplored;
    }

    private void WriteToFile()
    {
        List<string> toWrite = new List<string> { mapData.MapSize.ToString() };
        for (var i = 0; i < detectionBehaviours.Count; i++)
        {
            float algorithmTime = 0;
            float nodesExplored = 0;
            float pathLength = 0;
            foreach (var floats in _dataToWrite[0])
            {
                algorithmTime += floats[i];
            }

            foreach (var num in _dataToWrite[1])
            {
                nodesExplored += num[i];
            }
            
            foreach (var num in _dataToWrite[2])
            {
                pathLength += num[i];
            }
            
            algorithmTime /= _dataToWrite[0].Count;
            nodesExplored /= _dataToWrite[1].Count;
            pathLength /= _dataToWrite[2].Count;

            if (algorithmTime > cancelDurationFactor)
                _stoppedAlgorithms.Add(i);

            toWrite.Add((algorithmTime * timeMultiplier).ToString(CultureInfo.InvariantCulture));
            toWrite.Add(nodesExplored.ToString(CultureInfo.InvariantCulture));
            toWrite.Add(pathLength.ToString(CultureInfo.InvariantCulture));
        }

        // Use the EnhancedCsvFileWriter to add a row
        EnhancedCsvFileWriter.AddRow(toWrite);

        // Optionally, you can flush data to file after adding each row or at specific intervals
        EnhancedCsvFileWriter.FlushDataToFile();
    }
}
