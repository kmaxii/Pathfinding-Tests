// Decompiled with JetBrains decompiler
// Type: C5.SortedArrayDictionary`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  internal class SortedArrayDictionary<K, V> : SortedDictionaryBase<K, V>
  {
    public SortedArrayDictionary()
      : this((IComparer<K>) System.Collections.Generic.Comparer<K>.Default, C5.EqualityComparer<K>.Default)
    {
    }

    public SortedArrayDictionary(IComparer<K> comparer)
      : this(comparer, (IEqualityComparer<K>) new ComparerZeroHashCodeEqualityComparer<K>(comparer))
    {
    }

    public SortedArrayDictionary(IComparer<K> comparer, IEqualityComparer<K> equalityComparer)
      : base(comparer, equalityComparer)
    {
      this.pairs = (ICollection<KeyValuePair<K, V>>) (this.sortedpairs = (ISorted<KeyValuePair<K, V>>) new SortedArray<KeyValuePair<K, V>>((IComparer<KeyValuePair<K, V>>) new KeyValuePairComparer<K, V>(comparer)));
    }

    public SortedArrayDictionary(
      int capacity,
      IComparer<K> comparer,
      IEqualityComparer<K> equalityComparer)
      : base(comparer, equalityComparer)
    {
      this.pairs = (ICollection<KeyValuePair<K, V>>) (this.sortedpairs = (ISorted<KeyValuePair<K, V>>) new SortedArray<KeyValuePair<K, V>>(capacity, (IComparer<KeyValuePair<K, V>>) new KeyValuePairComparer<K, V>(comparer)));
    }
  }
}
