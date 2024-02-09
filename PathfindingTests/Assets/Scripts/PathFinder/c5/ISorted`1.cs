// Decompiled with JetBrains decompiler
// Type: C5.ISorted`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface ISorted<T> : 
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
    T FindMin();

    T DeleteMin();

    T FindMax();

    T DeleteMax();

    IComparer<T> Comparer { get; }

    bool TryPredecessor(T item, out T res);

    bool TrySuccessor(T item, out T res);

    bool TryWeakPredecessor(T item, out T res);

    bool TryWeakSuccessor(T item, out T res);

    T Predecessor(T item);

    T Successor(T item);

    T WeakPredecessor(T item);

    T WeakSuccessor(T item);

    bool Cut(
      IComparable<T> cutFunction,
      out T low,
      out bool lowIsValid,
      out T high,
      out bool highIsValid);

    IDirectedEnumerable<T> RangeFrom(T bot);

    IDirectedEnumerable<T> RangeFromTo(T bot, T top);

    IDirectedEnumerable<T> RangeTo(T top);

    IDirectedCollectionValue<T> RangeAll();

    void AddSorted(IEnumerable<T> items);

    void RemoveRangeFrom(T low);

    void RemoveRangeFromTo(T low, T hi);

    void RemoveRangeTo(T hi);
  }
}
