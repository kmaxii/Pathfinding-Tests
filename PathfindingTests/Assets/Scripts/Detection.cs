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
    [Tooltip("The program will shut down if this is met")]
    [SerializeField] private int maxSimulationTime = 60;
    [Tooltip("If an algorithm takes more then this time, cancel it for future runs")]
    [SerializeField] private float cancelDurationFactor = 2;
    
    [SerializeField] private int updatesToAverage = 1;
    
    [Tooltip("The time the time number that is written should be multiplied by to avoid E-... values.")]
    [SerializeField] private float timeMultiplier;
    
    [Header("Must haves")]
    [SerializeField] private CircleSpawner circleSpawner;
    [SerializeField] private SphereTransforms spheres;

    [SerializeField] private Transform startObject;
    [SerializeField] private Transform endObject;
    
    
    public static readonly HashSet<Tuple<Vector3, Vector3>> Lines = new();
    private readonly HashSet<int> _stoppedAlgorithms = new();
    private List<float[]> _dataToWrite = new List<float[]>();
    private readonly List<Color> _gizmoColors = new List<Color>{Color.red, Color.green, Color.yellow};

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
        
        foreach (var sph in spheres.AllColliders)
        {
            sph.textMeshPro.text = "";

            if (sph.transform != startObject && sph.transform != endObject)
            {
                sph.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        
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

        circleSpawner.SpawnCircles();
        
        //Reset variable
        _currentUpdate = 0;
        
        WriteToFile();
    }

    private void RunDetectionAlgorithm(int i)
    {
        var algorithm = detectionBehaviour[i];
        
        Profiler.BeginSample(algorithm.name, this);
        var startTime = Time.realtimeSinceStartup;
            
        var linkedList = algorithm.GetShortestPath(startObject, endObject, spheres);
        
        resolutionBehaviour.Resolve(linkedList);

        var algorithmTime = Time.realtimeSinceStartup - startTime;

        _dataToWrite[_currentUpdate][i] = algorithmTime;
        
            
        Profiler.EndSample();
    }

    private void WriteToFile()
    {
        List<String> toWrite = new() {spheres.AllColliders.Count.ToString()};
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
        
        int counter = -1;
        foreach (var line in Lines)
        { 
            if (Vector3.Distance(line.Item1, startObject.transform.position) < 0.001f)
            {
                counter++;
                Gizmos.color = _gizmoColors[counter];
            }
            
            Gizmos.DrawLine(line.Item1, line.Item2);
        }
    }
}