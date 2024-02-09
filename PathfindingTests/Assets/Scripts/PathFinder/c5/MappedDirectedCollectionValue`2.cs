// Decompiled with JetBrains decompiler
// Type: C5.MappedDirectedCollectionValue`2
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
  internal abstract class MappedDirectedCollectionValue<T, V> : 
    DirectedCollectionValueBase<V>,
    IDirectedCollectionValue<V>,
    ICollectionValue<V>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<V>,
    IEnumerable<V>,
    IEnumerable
  {
    private IDirectedCollectionValue<T> directedcollectionvalue;

    public abstract V Map(T item);

    public MappedDirectedCollectionValue(
      IDirectedCollectionValue<T> directedcollectionvalue)
    {
      this.directedcollectionvalue = directedcollectionvalue;
    }

    public override V Choose() => this.Map(this.directedcollectionvalue.Choose());

    public override bool IsEmpty => this.directedcollectionvalue.IsEmpty;

    public override int Count => this.directedcollectionvalue.Count;

    public override Speed CountSpeed => this.directedcollectionvalue.CountSpeed;

    public override IDirectedCollectionValue<V> Backwards()
    {
      MappedDirectedCollectionValue<T, V> directedCollectionValue = (MappedDirectedCollectionValue<T, V>) this.MemberwiseClone();
      directedCollectionValue.directedcollectionvalue = this.directedcollectionvalue.Backwards();
      return (IDirectedCollectionValue<V>) directedCollectionValue;
    }

    public override IEnumerator<V> GetEnumerator()
    {
      foreach (T item in (IEnumerable<T>) this.directedcollectionvalue)
        yield return this.Map(item);
    }

    public override EnumerationDirection Direction => this.directedcollectionvalue.Direction;

    IDirectedEnumerable<V> IDirectedEnumerable<V>.Backwards()
    {
      return (IDirectedEnumerable<V>) this.Backwards();
    }
  }
}
