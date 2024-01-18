using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CollidersSet")]
public class SphereTransforms : ScriptableObject
{
   public readonly List<Sphere> AllColliders = new List<Sphere>();
   
   public void Add(Sphere gameObject)
   {
      AllColliders.Add(gameObject);
   }

   public void Remove(Sphere gameObject)
   {
      AllColliders.Remove(gameObject);
   }
   
}