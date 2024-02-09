// Decompiled with JetBrains decompiler
// Type: C5.MappedCollectionValue`2
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
  internal abstract class MappedCollectionValue<T, V> : 
    CollectionValueBase<V>,
    ICollectionValue<V>,
    IEnumerable<V>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    private ICollectionValue<T> collectionvalue;

    public abstract V Map(T item);

    public MappedCollectionValue(ICollectionValue<T> collectionvalue)
    {
      this.collectionvalue = collectionvalue;
    }

    public override V Choose() => this.Map(this.collectionvalue.Choose());

    public override bool IsEmpty => this.collectionvalue.IsEmpty;

    public override int Count => this.collectionvalue.Count;

    public override Speed CountSpeed => this.collectionvalue.CountSpeed;

    public override IEnumerator<V> GetEnumerator()
    {
      foreach (T item in (IEnumerable<T>) this.collectionvalue)
        yield return this.Map(item);
    }
  }
}
