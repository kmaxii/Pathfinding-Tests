// Decompiled with JetBrains decompiler
// Type: C5.TreeDictionary`2
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
  public class TreeDictionary<K, V> : 
    SortedDictionaryBase<K, V>,
    ISortedDictionary<K, V>,
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    public TreeDictionary(MemoryType memoryType = MemoryType.Normal)
      : this((IComparer<K>) System.Collections.Generic.Comparer<K>.Default, C5.EqualityComparer<K>.Default, memoryType)
    {
    }

    public TreeDictionary(IComparer<K> comparer, MemoryType memoryType = MemoryType.Normal)
      : this(comparer, (IEqualityComparer<K>) new ComparerZeroHashCodeEqualityComparer<K>(comparer))
    {
    }

    private TreeDictionary(
      IComparer<K> comparer,
      IEqualityComparer<K> equalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(comparer, equalityComparer, memoryType)
    {
      this.pairs = (ICollection<KeyValuePair<K, V>>) (this.sortedpairs = (ISorted<KeyValuePair<K, V>>) new TreeSet<KeyValuePair<K, V>>((IComparer<KeyValuePair<K, V>>) new KeyValuePairComparer<K, V>(comparer)));
      if (memoryType != MemoryType.Normal)
        throw new Exception("TreeDictionary doesn't support MemoryType Strict or Safe");
    }

    public IEnumerable<KeyValuePair<K, V>> Snapshot()
    {
      TreeDictionary<K, V> treeDictionary = (TreeDictionary<K, V>) this.MemberwiseClone();
      treeDictionary.pairs = (ICollection<KeyValuePair<K, V>>) ((TreeSet<KeyValuePair<K, V>>) this.sortedpairs).Snapshot();
      return (IEnumerable<KeyValuePair<K, V>>) treeDictionary;
    }
  }
}
