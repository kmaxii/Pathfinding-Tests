// Decompiled with JetBrains decompiler
// Type: C5.GuardedDictionary`2
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
  public class GuardedDictionary<K, V> : 
    GuardedCollectionValue<KeyValuePair<K, V>>,
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    private IDictionary<K, V> dict;

    public GuardedDictionary(IDictionary<K, V> dict)
      : base((ICollectionValue<KeyValuePair<K, V>>) dict)
    {
      this.dict = dict;
    }

    public IEqualityComparer<K> EqualityComparer => this.dict.EqualityComparer;

    public V this[K key]
    {
      get => this.dict[key];
      set => throw new ReadOnlyCollectionException();
    }

    public bool IsReadOnly => true;

    public ICollectionValue<K> Keys => this.dict.Keys;

    public ICollectionValue<V> Values => this.dict.Values;

    public virtual System.Func<K, V> Func => (System.Func<K, V>) (k => this[k]);

    public void Add(K key, V val) => throw new ReadOnlyCollectionException();

    public void AddAll<L, W>(IEnumerable<KeyValuePair<L, W>> items)
      where L : K
      where W : V
    {
      throw new ReadOnlyCollectionException();
    }

    public bool Remove(K key) => throw new ReadOnlyCollectionException();

    public bool Remove(K key, out V val) => throw new ReadOnlyCollectionException();

    public void Clear() => throw new ReadOnlyCollectionException();

    public Speed ContainsSpeed => this.dict.ContainsSpeed;

    public bool Contains(K key) => this.dict.Contains(key);

    public bool ContainsAll<H>(IEnumerable<H> keys) where H : K => this.dict.ContainsAll<H>(keys);

    public bool Find(ref K key, out V val) => this.dict.Find(ref key, out val);

    public bool Update(K key, V val) => throw new ReadOnlyCollectionException();

    public bool Update(K key, V val, out V oldval) => throw new ReadOnlyCollectionException();

    public bool FindOrAdd(K key, ref V val) => throw new ReadOnlyCollectionException();

    public bool UpdateOrAdd(K key, V val) => throw new ReadOnlyCollectionException();

    public bool UpdateOrAdd(K key, V val, out V oldval) => throw new ReadOnlyCollectionException();

    public bool Check() => this.dict.Check();
  }
}
