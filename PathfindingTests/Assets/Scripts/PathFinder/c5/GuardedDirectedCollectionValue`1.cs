// Decompiled with JetBrains decompiler
// Type: C5.GuardedDirectedCollectionValue`1
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
  public class GuardedDirectedCollectionValue<T> : 
    GuardedCollectionValue<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private IDirectedCollectionValue<T> directedcollection;

    public GuardedDirectedCollectionValue(IDirectedCollectionValue<T> directedcollection)
      : base((ICollectionValue<T>) directedcollection)
    {
      this.directedcollection = directedcollection;
    }

    public virtual IDirectedCollectionValue<T> Backwards()
    {
      return (IDirectedCollectionValue<T>) new GuardedDirectedCollectionValue<T>(this.directedcollection.Backwards());
    }

    public virtual bool FindLast(Func<T, bool> predicate, out T item)
    {
      return this.directedcollection.FindLast(predicate, out item);
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public EnumerationDirection Direction => this.directedcollection.Direction;
  }
}
