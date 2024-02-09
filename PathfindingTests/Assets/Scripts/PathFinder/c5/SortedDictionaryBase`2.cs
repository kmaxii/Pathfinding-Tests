// Decompiled with JetBrains decompiler
// Type: C5.SortedDictionaryBase`2
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
  public abstract class SortedDictionaryBase<K, V> : 
    DictionaryBase<K, V>,
    ISortedDictionary<K, V>,
    IDictionary<K, V>,
    ICollectionValue<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    protected ISorted<KeyValuePair<K, V>> sortedpairs;
    private IComparer<K> keycomparer;

    protected SortedDictionaryBase(
      IComparer<K> keycomparer,
      IEqualityComparer<K> keyequalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(keyequalityComparer, memoryType)
    {
      this.keycomparer = keycomparer;
      this.MemoryType = memoryType;
    }

    public IComparer<K> Comparer => this.keycomparer;

    public ISorted<K> Keys
    {
      get
      {
        return (ISorted<K>) new SortedDictionaryBase<K, V>.SortedKeysCollection((ISortedDictionary<K, V>) this, this.sortedpairs, this.keycomparer, this.EqualityComparer, this.MemoryType);
      }
    }

    public bool TryPredecessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sortedpairs.TryPredecessor(new KeyValuePair<K, V>(key), out res);
    }

    public bool TrySuccessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sortedpairs.TrySuccessor(new KeyValuePair<K, V>(key), out res);
    }

    public bool TryWeakPredecessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sortedpairs.TryWeakPredecessor(new KeyValuePair<K, V>(key), out res);
    }

    public bool TryWeakSuccessor(K key, out KeyValuePair<K, V> res)
    {
      return this.sortedpairs.TryWeakSuccessor(new KeyValuePair<K, V>(key), out res);
    }

    public KeyValuePair<K, V> Predecessor(K key)
    {
      return this.sortedpairs.Predecessor(new KeyValuePair<K, V>(key));
    }

    public KeyValuePair<K, V> Successor(K key)
    {
      return this.sortedpairs.Successor(new KeyValuePair<K, V>(key));
    }

    public KeyValuePair<K, V> WeakPredecessor(K key)
    {
      return this.sortedpairs.WeakPredecessor(new KeyValuePair<K, V>(key));
    }

    public KeyValuePair<K, V> WeakSuccessor(K key)
    {
      return this.sortedpairs.WeakSuccessor(new KeyValuePair<K, V>(key));
    }

    public KeyValuePair<K, V> FindMin() => this.sortedpairs.FindMin();

    public KeyValuePair<K, V> DeleteMin() => this.sortedpairs.DeleteMin();

    public KeyValuePair<K, V> FindMax() => this.sortedpairs.FindMax();

    public KeyValuePair<K, V> DeleteMax() => this.sortedpairs.DeleteMax();

    public bool Cut(
      IComparable<K> cutter,
      out KeyValuePair<K, V> lowEntry,
      out bool lowIsValid,
      out KeyValuePair<K, V> highEntry,
      out bool highIsValid)
    {
      return this.sortedpairs.Cut((IComparable<KeyValuePair<K, V>>) new SortedDictionaryBase<K, V>.KeyValuePairComparable(cutter), out lowEntry, out lowIsValid, out highEntry, out highIsValid);
    }

    public IDirectedEnumerable<KeyValuePair<K, V>> RangeFrom(K bot)
    {
      return this.sortedpairs.RangeFrom(new KeyValuePair<K, V>(bot));
    }

    public IDirectedEnumerable<KeyValuePair<K, V>> RangeFromTo(K bot, K top)
    {
      return this.sortedpairs.RangeFromTo(new KeyValuePair<K, V>(bot), new KeyValuePair<K, V>(top));
    }

    public IDirectedEnumerable<KeyValuePair<K, V>> RangeTo(K top)
    {
      return this.sortedpairs.RangeTo(new KeyValuePair<K, V>(top));
    }

    public IDirectedCollectionValue<KeyValuePair<K, V>> RangeAll() => this.sortedpairs.RangeAll();

    public void AddSorted(IEnumerable<KeyValuePair<K, V>> items)
    {
      this.sortedpairs.AddSorted(items);
    }

    public void RemoveRangeFrom(K lowKey)
    {
      this.sortedpairs.RemoveRangeFrom(new KeyValuePair<K, V>(lowKey));
    }

    public void RemoveRangeFromTo(K lowKey, K highKey)
    {
      this.sortedpairs.RemoveRangeFromTo(new KeyValuePair<K, V>(lowKey), new KeyValuePair<K, V>(highKey));
    }

    public void RemoveRangeTo(K highKey)
    {
      this.sortedpairs.RemoveRangeTo(new KeyValuePair<K, V>(highKey));
    }

    public override bool Show(
      StringBuilder stringbuilder,
      ref int rest,
      IFormatProvider formatProvider)
    {
      return Showing.ShowDictionary<K, V>((IDictionary<K, V>) this, stringbuilder, ref rest, formatProvider);
    }

    [Serializable]
    private class KeyValuePairComparable : IComparable<KeyValuePair<K, V>>
    {
      private IComparable<K> cutter;

      internal KeyValuePairComparable(IComparable<K> cutter) => this.cutter = cutter;

      public int CompareTo(KeyValuePair<K, V> other) => this.cutter.CompareTo(other.Key);

      public bool Equals(KeyValuePair<K, V> other) => this.cutter.Equals((object) other.Key);
    }

    [Serializable]
    private class ProjectedDirectedEnumerable : MappedDirectedEnumerable<KeyValuePair<K, V>, K>
    {
      public ProjectedDirectedEnumerable(
        IDirectedEnumerable<KeyValuePair<K, V>> directedpairs)
        : base(directedpairs)
      {
      }

      public override K Map(KeyValuePair<K, V> pair) => pair.Key;
    }

    [Serializable]
    private class ProjectedDirectedCollectionValue : 
      MappedDirectedCollectionValue<KeyValuePair<K, V>, K>
    {
      public ProjectedDirectedCollectionValue(
        IDirectedCollectionValue<KeyValuePair<K, V>> directedpairs)
        : base(directedpairs)
      {
      }

      public override K Map(KeyValuePair<K, V> pair) => pair.Key;
    }

    [Serializable]
    private sealed class SortedKeysCollection : 
      SequencedBase<K>,
      ISorted<K>,
      ISequenced<K>,
      ICollection<K>,
      IExtensible<K>,
      System.Collections.Generic.ICollection<K>,
      IDirectedCollectionValue<K>,
      ICollectionValue<K>,
      IShowable,
      IFormattable,
      IDirectedEnumerable<K>,
      IEnumerable<K>,
      IEnumerable
    {
      private readonly SortedDictionaryBase<K, V>.SortedKeysCollection.KeyEnumerator _internalEnumerator;
      private ISortedDictionary<K, V> sorteddict;
      private ISorted<KeyValuePair<K, V>> sortedpairs;
      private IComparer<K> comparer;

      internal SortedKeysCollection(
        ISortedDictionary<K, V> sorteddict,
        ISorted<KeyValuePair<K, V>> sortedpairs,
        IComparer<K> comparer,
        IEqualityComparer<K> itemequalityComparer,
        MemoryType memoryType)
        : base(itemequalityComparer, memoryType)
      {
        this.sorteddict = sorteddict;
        this.sortedpairs = sortedpairs;
        this.comparer = comparer;
        this._internalEnumerator = new SortedDictionaryBase<K, V>.SortedKeysCollection.KeyEnumerator((ICollection<KeyValuePair<K, V>>) sortedpairs, memoryType);
      }

      public override K Choose() => this.sorteddict.Choose().Key;

      public override IEnumerator<K> GetEnumerator()
      {
        this._internalEnumerator.UpdateReference((ICollection<KeyValuePair<K, V>>) this.sortedpairs);
        return this._internalEnumerator.GetEnumerator();
      }

      public override bool IsEmpty => this.sorteddict.IsEmpty;

      public override int Count => this.sorteddict.Count;

      public override Speed CountSpeed => this.sorteddict.CountSpeed;

      public K FindMin() => this.sorteddict.FindMin().Key;

      public K DeleteMin() => throw new ReadOnlyCollectionException();

      public K FindMax() => this.sorteddict.FindMax().Key;

      public K DeleteMax() => throw new ReadOnlyCollectionException();

      public IComparer<K> Comparer => this.comparer;

      public bool TryPredecessor(K item, out K res)
      {
        KeyValuePair<K, V> res1;
        bool flag = this.sorteddict.TryPredecessor(item, out res1);
        res = res1.Key;
        return flag;
      }

      public bool TrySuccessor(K item, out K res)
      {
        KeyValuePair<K, V> res1;
        bool flag = this.sorteddict.TrySuccessor(item, out res1);
        res = res1.Key;
        return flag;
      }

      public bool TryWeakPredecessor(K item, out K res)
      {
        KeyValuePair<K, V> res1;
        bool flag = this.sorteddict.TryWeakPredecessor(item, out res1);
        res = res1.Key;
        return flag;
      }

      public bool TryWeakSuccessor(K item, out K res)
      {
        KeyValuePair<K, V> res1;
        bool flag = this.sorteddict.TryWeakSuccessor(item, out res1);
        res = res1.Key;
        return flag;
      }

      public K Predecessor(K item) => this.sorteddict.Predecessor(item).Key;

      public K Successor(K item) => this.sorteddict.Successor(item).Key;

      public K WeakPredecessor(K item) => this.sorteddict.WeakPredecessor(item).Key;

      public K WeakSuccessor(K item) => this.sorteddict.WeakSuccessor(item).Key;

      public bool Cut(
        IComparable<K> c,
        out K low,
        out bool lowIsValid,
        out K high,
        out bool highIsValid)
      {
        KeyValuePair<K, V> lowEntry;
        KeyValuePair<K, V> highEntry;
        bool flag = this.sorteddict.Cut(c, out lowEntry, out lowIsValid, out highEntry, out highIsValid);
        low = lowEntry.Key;
        high = highEntry.Key;
        return flag;
      }

      public IDirectedEnumerable<K> RangeFrom(K bot)
      {
        return (IDirectedEnumerable<K>) new SortedDictionaryBase<K, V>.ProjectedDirectedEnumerable(this.sorteddict.RangeFrom(bot));
      }

      public IDirectedEnumerable<K> RangeFromTo(K bot, K top)
      {
        return (IDirectedEnumerable<K>) new SortedDictionaryBase<K, V>.ProjectedDirectedEnumerable(this.sorteddict.RangeFromTo(bot, top));
      }

      public IDirectedEnumerable<K> RangeTo(K top)
      {
        return (IDirectedEnumerable<K>) new SortedDictionaryBase<K, V>.ProjectedDirectedEnumerable(this.sorteddict.RangeTo(top));
      }

      public IDirectedCollectionValue<K> RangeAll()
      {
        return (IDirectedCollectionValue<K>) new SortedDictionaryBase<K, V>.ProjectedDirectedCollectionValue(this.sorteddict.RangeAll());
      }

      public void AddSorted(IEnumerable<K> items) => throw new ReadOnlyCollectionException();

      public void RemoveRangeFrom(K low) => throw new ReadOnlyCollectionException();

      public void RemoveRangeFromTo(K low, K hi) => throw new ReadOnlyCollectionException();

      public void RemoveRangeTo(K hi) => throw new ReadOnlyCollectionException();

      public Speed ContainsSpeed => this.sorteddict.ContainsSpeed;

      public bool Contains(K key) => this.sorteddict.Contains(key);

      public int ContainsCount(K item) => !this.sorteddict.Contains(item) ? 0 : 1;

      public ICollectionValue<K> UniqueItems() => (ICollectionValue<K>) this;

      public ICollectionValue<KeyValuePair<K, int>> ItemMultiplicities()
      {
        return (ICollectionValue<KeyValuePair<K, int>>) new MultiplicityOne<K>((ICollectionValue<K>) this);
      }

      public bool ContainsAll(IEnumerable<K> items)
      {
        foreach (K key in items)
        {
          if (!this.sorteddict.Contains(key))
            return false;
        }
        return true;
      }

      public bool Find(ref K item)
      {
        KeyValuePair<K, V> keyValuePair = new KeyValuePair<K, V>(item);
        bool flag = this.sortedpairs.Find(ref keyValuePair);
        item = keyValuePair.Key;
        return flag;
      }

      public bool FindOrAdd(ref K item) => throw new ReadOnlyCollectionException();

      public bool Update(K item) => throw new ReadOnlyCollectionException();

      public bool Update(K item, out K olditem) => throw new ReadOnlyCollectionException();

      public bool UpdateOrAdd(K item) => throw new ReadOnlyCollectionException();

      public bool UpdateOrAdd(K item, out K olditem) => throw new ReadOnlyCollectionException();

      public bool Remove(K item) => throw new ReadOnlyCollectionException();

      public bool Remove(K item, out K removeditem) => throw new ReadOnlyCollectionException();

      public void RemoveAllCopies(K item) => throw new ReadOnlyCollectionException();

      public void RemoveAll(IEnumerable<K> items) => throw new ReadOnlyCollectionException();

      public void Clear() => throw new ReadOnlyCollectionException();

      public void RetainAll(IEnumerable<K> items) => throw new ReadOnlyCollectionException();

      public override bool IsReadOnly => true;

      public bool AllowsDuplicates => false;

      public bool DuplicatesByCounting => true;

      public bool Add(K item) => throw new ReadOnlyCollectionException();

      void System.Collections.Generic.ICollection<K>.Add(K item)
      {
        throw new ReadOnlyCollectionException();
      }

      public void AddAll(IEnumerable<K> items) => throw new ReadOnlyCollectionException();

      public bool Check() => this.sorteddict.Check();

      public override IDirectedCollectionValue<K> Backwards() => this.RangeAll().Backwards();

      IDirectedEnumerable<K> IDirectedEnumerable<K>.Backwards()
      {
        return (IDirectedEnumerable<K>) this.Backwards();
      }

      [Serializable]
      private class KeyEnumerator : MemorySafeEnumerator<K>
      {
        private ICollection<KeyValuePair<K, V>> _internalList;
        private IEnumerator<KeyValuePair<K, V>> _internalEnumerator;

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

        public override void Dispose()
        {
          this._internalEnumerator.Dispose();
          this._internalEnumerator = (IEnumerator<KeyValuePair<K, V>>) null;
        }

        public override bool MoveNext()
        {
          ICollection<KeyValuePair<K, V>> internalList = this._internalList;
          if (this.IteratorState == -1 || this.IteratorState == 0)
            this._internalEnumerator = internalList.GetEnumerator();
          this.IteratorState = 1;
          if (this._internalEnumerator.MoveNext())
          {
            this.Current = this._internalEnumerator.Current.Key;
            return true;
          }
          this.IteratorState = 0;
          return false;
        }

        public override void Reset()
        {
          try
          {
            this._internalEnumerator.Reset();
          }
          catch (Exception ex)
          {
          }
          finally
          {
            this.Current = default (K);
          }
        }

        protected override MemorySafeEnumerator<K> Clone()
        {
          SortedDictionaryBase<K, V>.SortedKeysCollection.KeyEnumerator keyEnumerator = new SortedDictionaryBase<K, V>.SortedKeysCollection.KeyEnumerator(this._internalList, this.MemoryType);
          keyEnumerator.Current = default (K);
          return (MemorySafeEnumerator<K>) keyEnumerator;
        }
      }
    }
  }
}
