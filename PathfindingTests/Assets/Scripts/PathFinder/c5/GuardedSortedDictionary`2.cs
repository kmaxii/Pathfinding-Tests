// Decompiled with JetBrains decompiler
// Type: C5.GuardedSortedDictionary`2
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
  public class GuardedSortedDictionary<K, V> : 
    GuardedDictionary<K, V>,
    ISortedDictionary<K, V>,
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    private ISortedDictionary<K, V> sorteddict;

    public GuardedSortedDictionary(ISortedDictionary<K, V> sorteddict)
      : base((IDictionary<K, V>) sorteddict)
    {
      this.sorteddict = sorteddict;
    }

    public IComparer<K> Comparer => this.sorteddict.Comparer;

    public ISorted<K> Keys => (ISorted<K>) null;

    public bool TryPredecessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sorteddict.TryPredecessor(key, out res);
    }

    public bool TrySuccessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sorteddict.TrySuccessor(key, out res);
    }

    public bool TryWeakPredecessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sorteddict.TryWeakPredecessor(key, out res);
    }

    public bool TryWeakSuccessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sorteddict.TryWeakSuccessor(key, out res);
    }

    public KeyValuePair<K, V> Predecessor(K key) => this.sorteddict.Predecessor(key);

    public KeyValuePair<K, V> Successor(K key) => this.sorteddict.Successor(key);

    public KeyValuePair<K, V> WeakPredecessor(K key) => this.sorteddict.WeakPredecessor(key);

    public KeyValuePair<K, V> WeakSuccessor(K key) => this.sorteddict.WeakSuccessor(key);

    public KeyValuePair<K, V> FindMin() => this.sorteddict.FindMin();

    public KeyValuePair<K, V> DeleteMin() => throw new ReadOnlyCollectionException();

    public KeyValuePair<K, V> FindMax() => this.sorteddict.FindMax();

    public KeyValuePair<K, V> DeleteMax() => throw new ReadOnlyCollectionException();

    public bool Cut(
      IComparable<K> c,
      out KeyValuePair<K, V> lowEntry,
      out bool lowIsValid,
      out KeyValuePair<K, V> highEntry,
      out bool highIsValid)
    {
      return this.sorteddict.Cut(c, out lowEntry, out lowIsValid, out highEntry, out highIsValid);
    }

    public IDirectedEnumerable<KeyValuePair<K, V>> RangeFrom(K bot)
    {
      return (IDirectedEnumerable<KeyValuePair<K, V>>) new GuardedDirectedEnumerable<KeyValuePair<K, V>>(this.sorteddict.RangeFrom(bot));
    }

    public IDirectedEnumerable<KeyValuePair<K, V>> RangeFromTo(K bot, K top)
    {
      return (IDirectedEnumerable<KeyValuePair<K, V>>) new GuardedDirectedEnumerable<KeyValuePair<K, V>>(this.sorteddict.RangeFromTo(bot, top));
    }

    public IDirectedEnumerable<KeyValuePair<K, V>> RangeTo(K top)
    {
      return (IDirectedEnumerable<KeyValuePair<K, V>>) new GuardedDirectedEnumerable<KeyValuePair<K, V>>(this.sorteddict.RangeTo(top));
    }

    public IDirectedCollectionValue<KeyValuePair<K, V>> RangeAll()
    {
      return (IDirectedCollectionValue<KeyValuePair<K, V>>) new GuardedDirectedCollectionValue<KeyValuePair<K, V>>(this.sorteddict.RangeAll());
    }

    public void AddSorted(IEnumerable<KeyValuePair<K, V>> items)
    {
      throw new ReadOnlyCollectionException();
    }

    public void RemoveRangeFrom(K low) => throw new ReadOnlyCollectionException();

    public void RemoveRangeFromTo(K low, K hi) => throw new ReadOnlyCollectionException();

    public void RemoveRangeTo(K hi) => throw new ReadOnlyCollectionException();
  }
}
