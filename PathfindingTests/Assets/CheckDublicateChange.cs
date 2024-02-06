using System;
using System.Collections;
using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;

public class CheckDublicateChange : MonoBehaviour
{


    [SerializeField] private GameEventWithVector2Int toListen;
    [SerializeField] private GameEvent clear;
    
    private HashSet<Vector2Int> _visited = new HashSet<Vector2Int>();


    private void OnEnable()
    {
        toListen.RegisterListener(AddPosToDictionary);
        clear.RegisterListener(ClearDic);
    }
    
    private void OnDisable()
    {
        toListen.UnregisterListener(AddPosToDictionary);
        clear.UnregisterListener(ClearDic);
    }

    private void ClearDic()
    {
        _visited.Clear();
    }

    private void AddPosToDictionary(Vector2Int vector2Int)
    {
        if (_visited.Contains(vector2Int))
        {
            Debug.Log("Dublicate found at " + vector2Int);
        }
        else
        {
            _visited.Add(vector2Int);
        }
    }


}
