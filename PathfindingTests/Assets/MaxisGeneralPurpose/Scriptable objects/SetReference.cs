using System.Collections.Generic;
using MaxisGeneralPurpose.Event;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;

namespace Scriptable_objects
{
   [CreateAssetMenu(menuName = "Custom/data/set")]
   public class SetReference : DataCarrier
   {
      private readonly HashSet<GameObject> _set = new HashSet<GameObject>();
      [SerializeField] private GameEvent raiseOnEmptyList;

      public void Clear()
      {
         _set.Clear();
      }

      public void Add(GameObject gameObject)
      {
         _set.Add(gameObject);
      }

      public void Remove(GameObject gameObject)
      {
         if (_set.Remove(gameObject) && _set.Count == 0 && raiseOnEmptyList)
         {
            raiseOnEmptyList.Raise();
         }
      }
   
      public void RemoveAndDestroy(GameObject gameObject)
      {
         Destroy(gameObject);
         Remove(gameObject);
      }

      public bool Contains(GameObject gameObject)
      {
         return _set.Contains(gameObject);
      }

      public bool IsEmpty()
      {
         return _set.Count == 0;
      }
   }
}
