// Decompiled with JetBrains decompiler
// Type: C5.IDictionary`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface IDictionary<K, V> : 
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    IEqualityComparer<K> EqualityComparer { get; }

    V this[K key] { get; set; }

    bool IsReadOnly { get; }

    ICollectionValue<K> Keys { get; }

    ICollectionValue<V> Values { get; }

    System.Func<K, V> Func { get; }

    void Add(K key, V val);

    void AddAll<U, W>(IEnumerable<KeyValuePair<U, W>> entries)
      where U : K
      where W : V;

    Speed ContainsSpeed { get; }

    bool ContainsAll<H>(IEnumerable<H> items) where H : K;

    bool Remove(K key);

    bool Remove(K key, out V val);

    void Clear();

    bool Contains(K key);

    bool Find(ref K key, out V val);

    bool Update(K key, V val);

    bool Update(K key, V val, out V oldval);

    bool FindOrAdd(K key, ref V val);

    bool UpdateOrAdd(K key, V val);

    bool UpdateOrAdd(K key, V val, out V oldval);

    bool Check();
  }
}
