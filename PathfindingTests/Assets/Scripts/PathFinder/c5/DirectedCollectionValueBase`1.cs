// Decompiled with JetBrains decompiler
// Type: C5.DirectedCollectionValueBase`1
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
  public abstract class DirectedCollectionValueBase<T> : 
    CollectionValueBase<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public virtual EnumerationDirection Direction => EnumerationDirection.Forwards;

    public abstract IDirectedCollectionValue<T> Backwards();

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public virtual bool FindLast(Func<T, bool> predicate, out T item)
    {
      foreach (T backward in (IEnumerable<T>) this.Backwards())
      {
        if (predicate(backward))
        {
          item = backward;
          return true;
        }
      }
      item = default (T);
      return false;
    }
  }
}
