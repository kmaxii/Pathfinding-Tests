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
    private List<float[]> _dataToWrite = new();
    private List<float[]> _nodesExploredToWrite = new();
    [SerializeField] private MapData mapData;

    private int _currentUpdate;

    private void Start()
    {
        // Initialize CSV file with dynamic headers based on the detection behaviours
        List<string> headers = new List<string> { "MapSize" };
        foreach (var behaviour in detectionBehaviours)
        {
            headers.Add(behaviour.name + "Time");
            headers.Add(behaviour.name + "ExploredNodes");
        }
        EnhancedCsvFileWriter.Initialize("DetectionData.csv", headers);

        _dataToWrite = new List<float[]>();
        _nodesExploredToWrite = new List<float[]>();
        for (int i = 0; i < updatesToAverage; i++)
        {
            _dataToWrite.Add(new float[detectionBehaviours.Count]);
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
        

        _dataToWrite[_currentUpdate][i] = algorithmTime;
        _nodesExploredToWrite[_currentUpdate][i] = algoResult.nodesExplored;
    }

    private void WriteToFile()
    {
        List<string> toWrite = new List<string> { mapData.MapSize.ToString() };
        for (var i = 0; i < detectionBehaviours.Count; i++)
        {
            float algorithmTime = 0;
            float nodesExplored = 0;
            foreach (var floats in _dataToWrite)
            {
                algorithmTime += floats[i];
            }

            foreach (var num in _nodesExploredToWrite)
            {
                nodesExplored += num[i];
            }
            
            algorithmTime /= _dataToWrite.Count;
            nodesExplored /= _nodesExploredToWrite.Count;

            if (algorithmTime > cancelDurationFactor)
                _stoppedAlgorithms.Add(i);

            toWrite.Add((algorithmTime * timeMultiplier).ToString(CultureInfo.InvariantCulture));
            toWrite.Add(nodesExplored.ToString(CultureInfo.InvariantCulture));
        }

        // Use the EnhancedCsvFileWriter to add a row
        EnhancedCsvFileWriter.AddRow(toWrite);

        // Optionally, you can flush data to file after adding each row or at specific intervals
        EnhancedCsvFileWriter.FlushDataToFile();
    }
}
