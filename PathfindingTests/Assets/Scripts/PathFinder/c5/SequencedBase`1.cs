// Decompiled with JetBrains decompiler
// Type: C5.SequencedBase`1
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
  public abstract class SequencedBase<T> : 
    DirectedCollectionBase<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private const int HASHFACTOR = 31;
    private int iSequencedHashCode;
    private int iSequencedHashCodeStamp = -1;

    protected SequencedBase(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType)
      : base(itemequalityComparer, memoryType)
    {
    }

    public static int ComputeHashCode(
      ISequenced<T> items,
      IEqualityComparer<T> itemequalityComparer)
    {
      int hashCode = 0;
      foreach (T obj in (IEnumerable<T>) items)
        hashCode = hashCode * 31 + itemequalityComparer.GetHashCode(obj);
      return hashCode;
    }

    public static bool StaticEquals(
      ISequenced<T> collection1,
      ISequenced<T> collection2,
      IEqualityComparer<T> itemequalityComparer)
    {
      if (object.ReferenceEquals((object) collection1, (object) collection2))
        return true;
      if (((ICollection<T>) collection1).Count != ((ICollection<T>) collection2).Count || collection1.GetSequencedHashCode() != collection2.GetSequencedHashCode())
        return false;
      using (IEnumerator<T> enumerator = collection2.GetEnumerator())
      {
        foreach (T x in (IEnumerable<T>) collection1)
        {
          enumerator.MoveNext();
          if (!itemequalityComparer.Equals(x, enumerator.Current))
            return false;
        }
      }
      return true;
    }

    public virtual int GetSequencedHashCode()
    {
      if (this.iSequencedHashCodeStamp == this.stamp)
        return this.iSequencedHashCode;
      this.iSequencedHashCode = SequencedBase<T>.ComputeHashCode((ISequenced<T>) this, this.itemequalityComparer);
      this.iSequencedHashCodeStamp = this.stamp;
      return this.iSequencedHashCode;
    }

    public virtual bool SequencedEquals(ISequenced<T> otherCollection)
    {
      return SequencedBase<T>.StaticEquals((ISequenced<T>) this, otherCollection, this.itemequalityComparer);
    }

    public override EnumerationDirection Direction => EnumerationDirection.Forwards;

    public int FindIndex(Func<T, bool> predicate)
    {
      int index = 0;
      foreach (T obj in (EnumerableBase<T>) this)
      {
        if (predicate(obj))
          return index;
        ++index;
      }
      return -1;
    }

    public int FindLastIndex(Func<T, bool> predicate)
    {
      int lastIndex = this.Count - 1;
      foreach (T backward in (IEnumerable<T>) this.Backwards())
      {
        if (predicate(backward))
          return lastIndex;
        --lastIndex;
      }
      return -1;
    }
  }
}
