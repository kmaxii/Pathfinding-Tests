// Decompiled with JetBrains decompiler
// Type: C5.CollectionBase`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public abstract class CollectionBase<T> : CollectionValueBase<T>
  {
    protected bool isReadOnlyBase;
    protected int size;
    protected readonly IEqualityComparer<T> itemequalityComparer;
    private int iUnSequencedHashCode;
    private int iUnSequencedHashCodeStamp = -1;
    private static Type isortedtype = typeof (ISorted<T>);

    protected int stamp { get; set; }

    protected CollectionBase(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType)
    {
      this.itemequalityComparer = itemequalityComparer != null ? itemequalityComparer : throw new NullReferenceException("Item EqualityComparer cannot be null.");
      this.MemoryType = memoryType;
    }

    protected void checkRange(int start, int count)
    {
      if (start < 0 || count < 0 || start + count > this.size)
        throw new ArgumentOutOfRangeException();
    }

    public static int ComputeHashCode(
      ICollectionValue<T> items,
      IEqualityComparer<T> itemequalityComparer)
    {
      int hashCode1 = 0;
      foreach (T obj in (IEnumerable<T>) items)
      {
        uint hashCode2 = (uint) itemequalityComparer.GetHashCode(obj);
        hashCode1 += (int) hashCode2 * 1529784657 + 1 ^ (int) hashCode2 * -1382135419 ^ (int) hashCode2 * 1118771817 + 2;
      }
      return hashCode1;
    }

    public static bool StaticEquals(
      ICollection<T> collection1,
      ICollection<T> collection2,
      IEqualityComparer<T> itemequalityComparer)
    {
      if (object.ReferenceEquals((object) collection1, (object) collection2))
        return true;
      if (collection1 == null || collection2 == null || collection1.Count != collection2.Count || collection1.GetUnsequencedHashCode() != collection2.GetUnsequencedHashCode())
        return false;
      if (collection1 is ISorted<T> sorted1 && collection2 is ISorted<T> sorted2 && sorted1.Comparer == sorted2.Comparer)
      {
        using (IEnumerator<T> enumerator1 = collection2.GetEnumerator())
        {
          using (IEnumerator<T> enumerator2 = collection1.GetEnumerator())
          {
            while (enumerator2.MoveNext())
            {
              enumerator1.MoveNext();
              if (!itemequalityComparer.Equals(enumerator2.Current, enumerator1.Current))
                return false;
            }
            return true;
          }
        }
      }
      else
      {
        if (!collection1.AllowsDuplicates && (collection2.AllowsDuplicates || collection2.ContainsSpeed >= collection1.ContainsSpeed))
        {
          foreach (T obj in (IEnumerable<T>) collection1)
          {
            if (!collection2.Contains(obj))
              return false;
          }
        }
        else if (!collection2.AllowsDuplicates)
        {
          foreach (T obj in (IEnumerable<T>) collection2)
          {
            if (!collection1.Contains(obj))
              return false;
          }
        }
        else if (collection1.DuplicatesByCounting && collection2.DuplicatesByCounting)
        {
          foreach (T obj in (IEnumerable<T>) collection2)
          {
            if (collection1.ContainsCount(obj) != collection2.ContainsCount(obj))
              return false;
          }
        }
        else
        {
          HashDictionary<T, int> hashDictionary = new HashDictionary<T, int>(itemequalityComparer);
          foreach (T key in (IEnumerable<T>) collection2)
          {
            int num = 1;
            if (hashDictionary.FindOrAdd(key, ref num))
              hashDictionary[key] = num + 1;
          }
          foreach (T key1 in (IEnumerable<T>) collection1)
          {
            T key2 = key1;
            int num;
            if (!hashDictionary.Find(ref key2, out num) || num <= 0)
              return false;
            hashDictionary[key1] = num - 1;
          }
          return true;
        }
        return true;
      }
    }

    public virtual int GetUnsequencedHashCode()
    {
      if (this.iUnSequencedHashCodeStamp == this.stamp)
        return this.iUnSequencedHashCode;
      this.iUnSequencedHashCode = CollectionBase<T>.ComputeHashCode((ICollectionValue<T>) this, this.itemequalityComparer);
      this.iUnSequencedHashCodeStamp = this.stamp;
      return this.iUnSequencedHashCode;
    }

    public virtual bool UnsequencedEquals(ICollection<T> otherCollection)
    {
      return otherCollection != null && CollectionBase<T>.StaticEquals((ICollection<T>) this, otherCollection, this.itemequalityComparer);
    }

    protected virtual void modifycheck(int thestamp)
    {
      if (this.stamp != thestamp)
        throw new CollectionModifiedException();
    }

    protected virtual void updatecheck()
    {
      if (this.isReadOnlyBase)
        throw new ReadOnlyCollectionException();
      ++this.stamp;
    }

    public virtual bool IsReadOnly => this.isReadOnlyBase;

    public override int Count => this.size;

    public override Speed CountSpeed => Speed.Constant;

    public virtual IEqualityComparer<T> EqualityComparer => this.itemequalityComparer;

    public override bool IsEmpty => this.size == 0;
  }
}
