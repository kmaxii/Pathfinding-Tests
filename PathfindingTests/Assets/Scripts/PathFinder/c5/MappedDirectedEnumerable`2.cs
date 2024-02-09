// Decompiled with JetBrains decompiler
// Type: C5.MappedDirectedEnumerable`2
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
  internal abstract class MappedDirectedEnumerable<T, V> : 
    EnumerableBase<V>,
    IDirectedEnumerable<V>,
    IEnumerable<V>,
    IEnumerable
  {
    private IDirectedEnumerable<T> directedenumerable;

    public abstract V Map(T item);

    public MappedDirectedEnumerable(IDirectedEnumerable<T> directedenumerable)
    {
      this.directedenumerable = directedenumerable;
    }

    public IDirectedEnumerable<V> Backwards()
    {
      MappedDirectedEnumerable<T, V> directedEnumerable = (MappedDirectedEnumerable<T, V>) this.MemberwiseClone();
      directedEnumerable.directedenumerable = this.directedenumerable.Backwards();
      return (IDirectedEnumerable<V>) directedEnumerable;
    }

    public override IEnumerator<V> GetEnumerator()
    {
      foreach (T item in (IEnumerable<T>) this.directedenumerable)
        yield return this.Map(item);
    }

    public EnumerationDirection Direction => this.directedenumerable.Direction;
  }
}
