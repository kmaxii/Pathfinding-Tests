// Decompiled with JetBrains decompiler
// Type: C5.DictionaryBase`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace C5
{
  [Serializable]
  public abstract class DictionaryBase<K, V> : 
    CollectionValueBase<KeyValuePair<K, V>>,
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    protected ICollection<KeyValuePair<K, V>> pairs;
    private IEqualityComparer<K> keyequalityComparer;
    private readonly DictionaryBase<K, V>.KeysCollection _keyCollection;
    private readonly DictionaryBase<K, V>.ValuesCollection _valueCollection;
    private ProxyEventBlock<KeyValuePair<K, V>> eventBlock;

    public override event CollectionChangedHandler<KeyValuePair<K, V>> CollectionChanged
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>((ICollectionValue<KeyValuePair<K, V>>) this, (ICollectionValue<KeyValuePair<K, V>>) this.pairs))).CollectionChanged += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.CollectionChanged -= value;
      }
    }

    public override event CollectionClearedHandler<KeyValuePair<K, V>> CollectionCleared
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>((ICollectionValue<KeyValuePair<K, V>>) this, (ICollectionValue<KeyValuePair<K, V>>) this.pairs))).CollectionCleared += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.CollectionCleared -= value;
      }
    }

    public override event ItemsAddedHandler<KeyValuePair<K, V>> ItemsAdded
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>((ICollectionValue<KeyValuePair<K, V>>) this, (ICollectionValue<KeyValuePair<K, V>>) this.pairs))).ItemsAdded += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemsAdded -= value;
      }
    }

    public override event ItemsRemovedHandler<KeyValuePair<K, V>> ItemsRemoved
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>((ICollectionValue<KeyValuePair<K, V>>) this, (ICollectionValue<KeyValuePair<K, V>>) this.pairs))).ItemsRemoved += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemsRemoved -= value;
      }
    }

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    public override EventTypeEnum ActiveEvents => this.pairs.ActiveEvents;

    protected DictionaryBase(IEqualityComparer<K> keyequalityComparer, MemoryType memoryType)
    {
      this.keyequalityComparer = keyequalityComparer != null ? keyequalityComparer : throw new NullReferenceException("Key equality comparer cannot be null");
      this.MemoryType = memoryType;
      this._keyCollection = new DictionaryBase<K, V>.KeysCollection(this.pairs, memoryType);
      this._valueCollection = new DictionaryBase<K, V>.ValuesCollection(this.pairs, memoryType);
    }

    public virtual IEqualityComparer<K> EqualityComparer => this.keyequalityComparer;

    public virtual void Add(K key, V value)
    {
      if (!this.pairs.Add(new KeyValuePair<K, V>(key, value)))
        throw new DuplicateNotAllowedException("Key being added: '" + (object) key + "'");
    }

    public virtual void AddAll<L, W>(IEnumerable<KeyValuePair<L, W>> entries)
      where L : K
      where W : V
    {
      foreach (KeyValuePair<L, W> entry in entries)
      {
        if (!this.pairs.Add(new KeyValuePair<K, V>((K) entry.Key, (V) entry.Value)))
          throw new DuplicateNotAllowedException("Key being added: '" + (object) entry.Key + "'");
      }
    }

    public virtual bool Remove(K key) => this.pairs.Remove(new KeyValuePair<K, V>(key));

    public virtual bool Remove(K key, out V value)
    {
      KeyValuePair<K, V> removeditem = new KeyValuePair<K, V>(key);
      if (this.pairs.Remove(removeditem, out removeditem))
      {
        value = removeditem.Value;
        return true;
      }
      value = default (V);
      return false;
    }

    public virtual void Clear() => this.pairs.Clear();

    public virtual Speed ContainsSpeed => this.pairs.ContainsSpeed;

    public virtual bool Contains(K key) => this.pairs.Contains(new KeyValuePair<K, V>(key));

    public virtual bool ContainsAll<H>(IEnumerable<H> keys) where H : K
    {
      if (this.MemoryType == MemoryType.Strict)
        throw new Exception("The use of ContainsAll generates garbage as it still uses a non-memory safe enumerator");
      return this.pairs.ContainsAll((IEnumerable<KeyValuePair<K, V>>) new DictionaryBase<K, V>.LiftedEnumerable<H>(keys));
    }

    public virtual bool Find(ref K key, out V value)
    {
      KeyValuePair<K, V> keyValuePair = new KeyValuePair<K, V>(key);
      if (this.pairs.Find(ref keyValuePair))
      {
        key = keyValuePair.Key;
        value = keyValuePair.Value;
        return true;
      }
      value = default (V);
      return false;
    }

    public virtual bool Update(K key, V value)
    {
      return this.pairs.Update(new KeyValuePair<K, V>(key, value));
    }

    public virtual bool Update(K key, V value, out V oldvalue)
    {
      KeyValuePair<K, V> olditem = new KeyValuePair<K, V>(key, value);
      bool flag = this.pairs.Update(olditem, out olditem);
      oldvalue = olditem.Value;
      return flag;
    }

    public virtual bool FindOrAdd(K key, ref V value)
    {
      KeyValuePair<K, V> keyValuePair = new KeyValuePair<K, V>(key, value);
      if (!this.pairs.FindOrAdd(ref keyValuePair))
        return false;
      value = keyValuePair.Value;
      return true;
    }

    public virtual bool UpdateOrAdd(K key, V value)
    {
      return this.pairs.UpdateOrAdd(new KeyValuePair<K, V>(key, value));
    }

    public virtual bool UpdateOrAdd(K key, V value, out V oldvalue)
    {
      KeyValuePair<K, V> olditem = new KeyValuePair<K, V>(key, value);
      bool flag = this.pairs.UpdateOrAdd(olditem, out olditem);
      oldvalue = olditem.Value;
      return flag;
    }

    public virtual ICollectionValue<K> Keys
    {
      get
      {
        this._keyCollection.Update(this.pairs);
        return (ICollectionValue<K>) this._keyCollection;
      }
    }

    public virtual ICollectionValue<V> Values
    {
      get
      {
        this._valueCollection.Update(this.pairs);
        return (ICollectionValue<V>) this._valueCollection;
      }
    }

    public virtual System.Func<K, V> Func => (System.Func<K, V>) (k => this[k]);

    public virtual V this[K key]
    {
      get
      {
        KeyValuePair<K, V> keyValuePair = new KeyValuePair<K, V>(key);
        return this.pairs.Find(ref keyValuePair) ? keyValuePair.Value : throw new NoSuchItemException("Key '" + key.ToString() + "' not present in Dictionary");
      }
      set => this.pairs.UpdateOrAdd(new KeyValuePair<K, V>(key, value));
    }

    public virtual bool IsReadOnly => this.pairs.IsReadOnly;

    public virtual bool Check() => this.pairs.Check();

    public override bool IsEmpty => this.pairs.IsEmpty;

    public override int Count => this.pairs.Count;

    public override Speed CountSpeed => this.pairs.CountSpeed;

    public override KeyValuePair<K, V> Choose() => this.pairs.Choose();

    public override IEnumerator<KeyValuePair<K, V>> GetEnumerator() => this.pairs.GetEnumerator();

    public override bool Show(
      StringBuilder stringbuilder,
      ref int rest,
      IFormatProvider formatProvider)
    {
      return Showing.ShowDictionary<K, V>((IDictionary<K, V>) this, stringbuilder, ref rest, formatProvider);
    }

    [Serializable]
    private class LiftedEnumerable<H> : IEnumerable<KeyValuePair<K, V>>, IEnumerable where H : K
    {
      private IEnumerable<H> keys;

      public LiftedEnumerable(IEnumerable<H> keys) => this.keys = keys;

      public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
      {
        foreach (H key in this.keys)
          yield return new KeyValuePair<K, V>((K) key);
      }

      IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

    [Serializable]
    internal class ValuesCollection : 
      CollectionValueBase<V>,
      ICollectionValue<V>,
      IEnumerable<V>,
      IEnumerable,
      IShowable,
      IFormattable
    {
      private ICollection<KeyValuePair<K, V>> _pairs;
      private readonly DictionaryBase<K, V>.ValuesCollection.ValueEnumerator _valueEnumerator;

      internal ValuesCollection(
        ICollection<KeyValuePair<K, V>> keyValuePairs,
        MemoryType memoryType)
      {
        this._pairs = keyValuePairs;
        this._valueEnumerator = new DictionaryBase<K, V>.ValuesCollection.ValueEnumerator(keyValuePairs, memoryType);
      }

      public override V Choose() => this._pairs.Choose().Value;

      public override IEnumerator<V> GetEnumerator()
      {
        DictionaryBase<K, V>.ValuesCollection.ValueEnumerator enumerator = (DictionaryBase<K, V>.ValuesCollection.ValueEnumerator) this._valueEnumerator.GetEnumerator();
        enumerator.UpdateReference(this._pairs);
        return (IEnumerator<V>) enumerator;
      }

      public override bool IsEmpty => this._pairs.IsEmpty;

      public override int Count => this._pairs.Count;

      public override Speed CountSpeed => Speed.Constant;

      public void Update(ICollection<KeyValuePair<K, V>> keyValuePairs)
      {
        this._pairs = keyValuePairs;
      }

      [Serializable]
      private class ValueEnumerator : MemorySafeEnumerator<V>
      {
        private ICollection<KeyValuePair<K, V>> _keyValuePairs;
        private IEnumerator<KeyValuePair<K, V>> _keyValuePairEnumerator;

        public ValueEnumerator(ICollection<KeyValuePair<K, V>> keyValuePairs, MemoryType memoryType)
          : base(memoryType)
        {
          this._keyValuePairs = keyValuePairs;
        }

        internal void UpdateReference(ICollection<KeyValuePair<K, V>> keyValuePairs)
        {
          this._keyValuePairs = keyValuePairs;
          this.Current = default (V);
        }

        public override bool MoveNext()
        {
          ICollection<KeyValuePair<K, V>> keyValuePairs = this._keyValuePairs;
          if (this._keyValuePairEnumerator == null)
            this._keyValuePairEnumerator = keyValuePairs.GetEnumerator();
          if (this._keyValuePairEnumerator.MoveNext())
          {
            this.Current = this._keyValuePairEnumerator.Current.Value;
            return true;
          }
          this._keyValuePairEnumerator.Dispose();
          this.Current = default (V);
          return false;
        }

        public override void Reset() => this.Current = default (V);

        protected override MemorySafeEnumerator<V> Clone()
        {
          DictionaryBase<K, V>.ValuesCollection.ValueEnumerator valueEnumerator = new DictionaryBase<K, V>.ValuesCollection.ValueEnumerator(this._keyValuePairs, this.MemoryType);
          valueEnumerator.Current = default (V);
          return (MemorySafeEnumerator<V>) valueEnumerator;
        }
      }
    }

    [Serializable]
    internal class KeysCollection : 
      CollectionValueBase<K>,
      ICollectionValue<K>,
      IEnumerable<K>,
      IEnumerable,
      IShowable,
      IFormattable
    {
      private ICollection<KeyValuePair<K, V>> _pairs;
      private readonly DictionaryBase<K, V>.KeysCollection.KeyEnumerator _keyEnumerator;

      internal KeysCollection(ICollection<KeyValuePair<K, V>> pairs, MemoryType memoryType)
      {
        this._pairs = pairs;
        this._keyEnumerator = new DictionaryBase<K, V>.KeysCollection.KeyEnumerator(pairs, memoryType);
      }

      public void Update(ICollection<KeyValuePair<K, V>> pairs) => this._pairs = pairs;

      public override K Choose() => this._pairs.Choose().Key;

      public override IEnumerator<K> GetEnumerator()
      {
        DictionaryBase<K, V>.KeysCollection.KeyEnumerator enumerator = (DictionaryBase<K, V>.KeysCollection.KeyEnumerator) this._keyEnumerator.GetEnumerator();
        enumerator.UpdateReference(this._pairs);
        return (IEnumerator<K>) enumerator;
      }

      public override bool IsEmpty => this._pairs.IsEmpty;

      public override int Count => this._pairs.Count;

      public override Speed CountSpeed => this._pairs.CountSpeed;

      [Serializable]
      private class KeyEnumerator : MemorySafeEnumerator<K>
      {
        private ICollection<KeyValuePair<K, V>> _internalList;
        private IEnumerator<KeyValuePair<K, V>> _keyValuePairEnumerator;

        public KeyEnumerator(ICollection<KeyValuePair<K, V>> list, MemoryType memoryType)
          : base(memoryType)
        {
          this._internalList = list;
        }

        internal void UpdateReference(ICollection<KeyValuePair<K, V>> list)
        {
          this._internalList = list;
          this.Current = default (K);
        }

        public override bool MoveNext()
        {
          ICollection<KeyValuePair<K, V>> internalList = this._internalList;
          if (this._keyValuePairEnumerator == null)
            this._keyValuePairEnumerator = internalList.GetEnumerator();
          if (this._keyValuePairEnumerator.MoveNext())
          {
            this.Current = this._keyValuePairEnumerator.Current.Key;
            return true;
          }
          this._keyValuePairEnumerator.Dispose();
          this.Current = default (K);
          return false;
        }

        public override void Reset() => this.Current = default (K);

        protected override MemorySafeEnumerator<K> Clone()
        {
          DictionaryBase<K, V>.KeysCollection.KeyEnumerator keyEnumerator = new DictionaryBase<K, V>.KeysCollection.KeyEnumerator(this._internalList, this.MemoryType);
          keyEnumerator.Current = default (K);
          return (MemorySafeEnumerator<K>) keyEnumerator;
        }
      }
    }
  }
}
