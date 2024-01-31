using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Profiling;

public class Detection : MonoBehaviour
{
    [Header("Algorithms")]
    [SerializeField] private List<DetectionBehaviour.DetectionBehaviour> detectionBehaviour;
    [SerializeField] private ResolutionBehaviour.ResolutionBehaviour resolutionBehaviour;

    [Header("Simulation settings")] 
    [Tooltip("If an algorithm takes more then this time, cancel it for future runs")]
    [SerializeField] private float cancelDurationFactor = 2;
    
    [SerializeField] private int updatesToAverage = 1;
    
    [Tooltip("The time the time number that is written should be multiplied by to avoid E-... values.")]
    [SerializeField] private float timeMultiplier;
    
    [SerializeField] private Vector2Int startPos = new(1, 1);
    [SerializeField] private Vector2Int endPos = new(8, 8);
    
    
    public static readonly HashSet<Tuple<Vector2Int, Vector2Int>> Lines = new();
    private readonly HashSet<int> _stoppedAlgorithms = new();
    private List<float[]> _dataToWrite = new();
    [SerializeField] private MapData mapData;

    private int _currentUpdate;


    private void Start()
    {
        CsvFileWriter.CreateNewFile();
        CsvFileWriter.WriteHeaders(detectionBehaviour);
        _dataToWrite = new();
        for (int i = 0; i < updatesToAverage; i++)
        {
            _dataToWrite.Add(new float[3]);
        }
    }

    void Update()
    {
        Lines.Clear();
        
        if (_stoppedAlgorithms.Count == detectionBehaviour.Count)
            Application.Quit();
        
        RunAlgorithms();
    }

    private void RunAlgorithms()
    {
        for (var i = 0; i < detectionBehaviour.Count; i++)
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
        
        //Reset variable
        _currentUpdate = 0;
        
        WriteToFile();
    }

    private void RunDetectionAlgorithm(int i)
    {
        var algorithm = detectionBehaviour[i];
        
        Profiler.BeginSample(algorithm.name, this);
        var startTime = Time.realtimeSinceStartup;
            
        var linkedList = algorithm.GetShortestPath(startPos, endPos);
        
        resolutionBehaviour.Resolve(linkedList);

        var algorithmTime = Time.realtimeSinceStartup - startTime;

        _dataToWrite[_currentUpdate][i] = algorithmTime;
        
            
        Profiler.EndSample();
    }

    private void WriteToFile()
    {
        List<String> toWrite = new() {mapData.MapSize.ToString()};
        for (var i = 0; i < detectionBehaviour.Count; i++)
        {
            float algorithmTime = 0;
            foreach (var floats in _dataToWrite)
            {
                algorithmTime += floats[i];
            }

            algorithmTime /= _dataToWrite.Count;
            
            if (algorithmTime > cancelDurationFactor)
                _stoppedAlgorithms.Add(i);
            
            toWrite.Add((algorithmTime * timeMultiplier).ToString(CultureInfo.InvariantCulture));
            
        }
        
        CsvFileWriter.WriteToFile(toWrite);
    }

    private void OnDrawGizmos()
    {
        foreach (var line in Lines)
        { 
            /*if (Vector3.Distance(line.Item1, startPos) < 0.001f)
            {
                counter++;
                Gizmos.color = _gizmoColors[counter];
            }*/
            
            Gizmos.DrawLine(new Vector3(line.Item1.x, line.Item1.y), new Vector3(line.Item2.x, line.Item2.y));
        }
    }
}