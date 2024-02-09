// Decompiled with JetBrains decompiler
// Type: C5.HashDictionary`2
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
  public class HashDictionary<K, V> : 
    DictionaryBase<K, V>,
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    public HashDictionary(MemoryType memoryType = MemoryType.Normal)
      : this(C5.EqualityComparer<K>.Default, memoryType)
    {
    }

    public HashDictionary(IEqualityComparer<K> keyequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : base(keyequalityComparer, memoryType)
    {
      this.pairs = (ICollection<KeyValuePair<K, V>>) new HashSet<KeyValuePair<K, V>>((IEqualityComparer<KeyValuePair<K, V>>) new KeyValuePairEqualityComparer<K, V>(keyequalityComparer), memoryType);
    }

    public HashDictionary(
      int capacity,
      double fill,
      IEqualityComparer<K> keyequalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(keyequalityComparer, memoryType)
    {
      this.pairs = (ICollection<KeyValuePair<K, V>>) new HashSet<KeyValuePair<K, V>>(capacity, fill, (IEqualityComparer<KeyValuePair<K, V>>) new KeyValuePairEqualityComparer<K, V>(keyequalityComparer), memoryType);
    }
  }
}
