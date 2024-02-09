// Decompiled with JetBrains decompiler
// Type: C5.EnumerableBase`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public abstract class EnumerableBase<T> : IEnumerable<T>, IEnumerable
  {
    public abstract IEnumerator<T> GetEnumerator();

    protected static int countItems(IEnumerable<T> items)
    {
      if (items is ICollectionValue<T> collectionValue)
        return collectionValue.Count;
      int num = 0;
      using (IEnumerator<T> enumerator = items.GetEnumerator())
      {
        while (enumerator.MoveNext())
          ++num;
      }
      return num;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
