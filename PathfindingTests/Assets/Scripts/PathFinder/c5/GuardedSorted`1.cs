// Decompiled with JetBrains decompiler
// Type: C5.GuardedSorted`1
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
  public class GuardedSorted<T> : 
    GuardedSequenced<T>,
    ISorted<T>,
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
    private ISorted<T> sorted;

    public GuardedSorted(ISorted<T> sorted)
      : base((ISequenced<T>) sorted)
    {
      this.sorted = sorted;
    }

    public bool TryPredecessor(T item, out T res) => this.sorted.TryPredecessor(item, out res);

    public bool TrySuccessor(T item, out T res) => this.sorted.TrySuccessor(item, out res);

    public bool TryWeakPredecessor(T item, out T res)
    {
      return this.sorted.TryWeakPredecessor(item, out res);
    }

    public bool TryWeakSuccessor(T item, out T res) => this.sorted.TryWeakSuccessor(item, out res);

    public T Predecessor(T item) => this.sorted.Predecessor(item);

    public T Successor(T item) => this.sorted.Successor(item);

    public T WeakPredecessor(T item) => this.sorted.WeakPredecessor(item);

    public T WeakSuccessor(T item) => this.sorted.WeakSuccessor(item);

    public bool Cut(IComparable<T> c, out T low, out bool lval, out T high, out bool hval)
    {
      return this.sorted.Cut(c, out low, out lval, out high, out hval);
    }

    public IDirectedEnumerable<T> RangeFrom(T bot) => this.sorted.RangeFrom(bot);

    public IDirectedEnumerable<T> RangeFromTo(T bot, T top) => this.sorted.RangeFromTo(bot, top);

    public IDirectedEnumerable<T> RangeTo(T top) => this.sorted.RangeTo(top);

    public IDirectedCollectionValue<T> RangeAll() => this.sorted.RangeAll();

    public void AddSorted(IEnumerable<T> items)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public void RemoveRangeFrom(T low)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public void RemoveRangeFromTo(T low, T hi)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public void RemoveRangeTo(T hi)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public T FindMin() => this.sorted.FindMin();

    public T DeleteMin()
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public T FindMax() => this.sorted.FindMax();

    public T DeleteMax()
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public IComparer<T> Comparer => this.sorted.Comparer;

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }
  }
}
