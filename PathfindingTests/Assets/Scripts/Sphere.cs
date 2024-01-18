using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Sphere : MonoBehaviour
{
    [SerializeField] private SphereTransforms spheres;

    public TextMeshPro textMeshPro;
    
    protected virtual void OnEnable()
    {
        spheres.Add(this);
    }

    private void OnDisable()
    {
        spheres.Remove(this);
    }
}