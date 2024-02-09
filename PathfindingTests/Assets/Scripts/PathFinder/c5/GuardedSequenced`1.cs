// Decompiled with JetBrains decompiler
// Type: C5.GuardedSequenced`1
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
  public class GuardedSequenced<T> : 
    GuardedCollection<T>,
    ISequenced<T>,
    ICollection<T>,
    IExtensible<T>,
    System.Collections.Generic.ICollection<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private ISequenced<T> sequenced;

    public GuardedSequenced(ISequenced<T> sorted)
      : base((ICollection<T>) sorted)
    {
      this.sequenced = sorted;
    }

    public int FindIndex(Func<T, bool> predicate)
    {
      if (this.sequenced is IIndexed<T> sequenced)
        return sequenced.FindIndex(predicate);
      int index = 0;
      foreach (T obj in (GuardedEnumerable<T>) this)
      {
        if (predicate(obj))
          return index;
        ++index;
      }
      return -1;
    }

    public int FindLastIndex(Func<T, bool> predicate)
    {
      if (this.sequenced is IIndexed<T> sequenced)
        return sequenced.FindLastIndex(predicate);
      int lastIndex = this.Count - 1;
      foreach (T backward in (IEnumerable<T>) this.Backwards())
      {
        if (predicate(backward))
          return lastIndex;
        --lastIndex;
      }
      return -1;
    }

    public int GetSequencedHashCode() => this.sequenced.GetSequencedHashCode();

    public bool SequencedEquals(ISequenced<T> that) => this.sequenced.SequencedEquals(that);

    public virtual IDirectedCollectionValue<T> Backwards()
    {
      return (IDirectedCollectionValue<T>) new GuardedDirectedCollectionValue<T>(this.sequenced.Backwards());
    }

    public virtual bool FindLast(Func<T, bool> predicate, out T item)
    {
      return this.sequenced.FindLast(predicate, out item);
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public EnumerationDirection Direction => EnumerationDirection.Forwards;
  }
}
