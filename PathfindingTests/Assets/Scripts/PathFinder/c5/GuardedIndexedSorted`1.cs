// Decompiled with JetBrains decompiler
// Type: C5.GuardedIndexedSorted`1
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
  public class GuardedIndexedSorted<T> : 
    GuardedSorted<T>,
    IIndexedSorted<T>,
    ISorted<T>,
    IIndexed<T>,
    ISequenced<T>,
    ICollection<T>,
    IExtensible<T>,
    System.Collections.Generic.ICollection<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private IIndexedSorted<T> indexedsorted;

    public GuardedIndexedSorted(IIndexedSorted<T> list)
      : base((ISorted<T>) list)
    {
      this.indexedsorted = list;
    }

    public IDirectedCollectionValue<T> RangeFrom(T bot) => this.indexedsorted.RangeFrom(bot);

    public IDirectedCollectionValue<T> RangeFromTo(T bot, T top)
    {
      return this.indexedsorted.RangeFromTo(bot, top);
    }

    public IDirectedCollectionValue<T> RangeTo(T top) => this.indexedsorted.RangeTo(top);

    public int CountFrom(T bot) => this.indexedsorted.CountFrom(bot);

    public int CountFromTo(T bot, T top) => this.indexedsorted.CountFromTo(bot, top);

    public int CountTo(T top) => this.indexedsorted.CountTo(top);

    public IIndexedSorted<T> FindAll(Func<T, bool> f) => this.indexedsorted.FindAll(f);

    public IIndexedSorted<V> Map<V>(Func<T, V> m, IComparer<V> c)
    {
      return this.indexedsorted.Map<V>(m, c);
    }

    public T this[int i] => this.indexedsorted[i];

    public virtual Speed IndexingSpeed => this.indexedsorted.IndexingSpeed;

    public IDirectedCollectionValue<T> this[int start, int end]
    {
      get
      {
        return (IDirectedCollectionValue<T>) new GuardedDirectedCollectionValue<T>(this.indexedsorted[start, end]);
      }
    }

    public int IndexOf(T item) => this.indexedsorted.IndexOf(item);

    public int LastIndexOf(T item) => this.indexedsorted.LastIndexOf(item);

    public T RemoveAt(int i)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public void RemoveInterval(int start, int count)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }
  }
}
