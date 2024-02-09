// Decompiled with JetBrains decompiler
// Type: C5.ISortedDictionary`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface ISortedDictionary<K, V> : 
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    ISorted<K> Keys { get; }

    KeyValuePair<K, V> FindMin();

    KeyValuePair<K, V> DeleteMin();

    KeyValuePair<K, V> FindMax();

    KeyValuePair<K, V> DeleteMax();

    IComparer<K> Comparer { get; }

    bool TryPredecessor(K key, out KeyValuePair<K, V> res);

    bool TrySuccessor(K key, out KeyValuePair<K, V> res);

    bool TryWeakPredecessor(K key, out KeyValuePair<K, V> res);

    bool TryWeakSuccessor(K key, out KeyValuePair<K, V> res);

    KeyValuePair<K, V> Predecessor(K key);

    KeyValuePair<K, V> Successor(K key);

    KeyValuePair<K, V> WeakPredecessor(K key);

    KeyValuePair<K, V> WeakSuccessor(K key);

    bool Cut(
      IComparable<K> cutFunction,
      out KeyValuePair<K, V> lowEntry,
      out bool lowIsValid,
      out KeyValuePair<K, V> highEntry,
      out bool highIsValid);

    IDirectedEnumerable<KeyValuePair<K, V>> RangeFrom(K bot);

    IDirectedEnumerable<KeyValuePair<K, V>> RangeFromTo(K lowerBound, K upperBound);

    IDirectedEnumerable<KeyValuePair<K, V>> RangeTo(K top);

    IDirectedCollectionValue<KeyValuePair<K, V>> RangeAll();

    void AddSorted(IEnumerable<KeyValuePair<K, V>> items);

    void RemoveRangeFrom(K low);

    void RemoveRangeFromTo(K low, K hi);

    void RemoveRangeTo(K hi);
  }
}
