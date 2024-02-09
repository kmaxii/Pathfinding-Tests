// Decompiled with JetBrains decompiler
// Type: C5.HashSet`1
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
  public class HashSet<T> : 
    CollectionBase<T>,
    ICollection<T>,
    IExtensible<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    System.Collections.Generic.ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private static HashSet<T>.Feature features = HashSet<T>.Feature.RefTypeBucket | HashSet<T>.Feature.Chaining | HashSet<T>.Feature.RandomInterHashing;
    private int indexmask;
    private int bits;
    private int bitsc;
    private int origbits;
    private int lastchosen;
    private HashSet<T>.Bucket[] table;
    private double fillfactor = 0.66;
    private int resizethreshhold;
    private static readonly Random Random = new Random();
    private readonly HashSet<T>.HashEnumerator _hashEnumerator;
    private uint _randomhashfactor;

    public static HashSet<T>.Feature Features => HashSet<T>.features;

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    private bool equals(T i1, T i2) => this.itemequalityComparer.Equals(i1, i2);

    private int gethashcode(T item) => this.itemequalityComparer.GetHashCode(item);

    private int hv2i(int hashval) => (int) ((uint) hashval * this._randomhashfactor >> this.bitsc);

    private void expand()
    {
      Logger.Log(string.Format(string.Format("Expand to {0} bits", (object) (this.bits + 1))));
      this.resize(this.bits + 1);
    }

    private void shrink()
    {
      if (this.bits <= 3)
        return;
      Logger.Log(string.Format(string.Format("Shrink to {0} bits", (object) (this.bits - 1))));
      this.resize(this.bits - 1);
    }

    private void resize(int bits)
    {
      Logger.Log(string.Format(string.Format("Resize to {0} bits", (object) bits)));
      this.bits = bits;
      this.bitsc = 32 - bits;
      this.indexmask = (1 << bits) - 1;
      HashSet<T>.Bucket[] bucketArray = new HashSet<T>.Bucket[this.indexmask + 1];
      int index1 = 0;
      for (int length = this.table.Length; index1 < length; ++index1)
      {
        for (HashSet<T>.Bucket overflow = this.table[index1]; overflow != null; overflow = overflow.overflow)
        {
          int index2 = this.hv2i(overflow.hashval);
          bucketArray[index2] = new HashSet<T>.Bucket(overflow.item, overflow.hashval, bucketArray[index2]);
        }
      }
      this.table = bucketArray;
      this.resizethreshhold = (int) ((double) this.table.Length * this.fillfactor);
      Logger.Log(string.Format(string.Format("Resize to {0} bits done", (object) bits)));
    }

    private bool searchoradd(ref T item, bool add, bool update, bool raise)
    {
      int hashval = this.gethashcode(item);
      int index = this.hv2i(hashval);
      HashSet<T>.Bucket overflow = this.table[index];
      HashSet<T>.Bucket bucket = (HashSet<T>.Bucket) null;
      if (overflow != null)
      {
        for (; overflow != null; overflow = overflow.overflow)
        {
          T obj = overflow.item;
          if (this.equals(obj, item))
          {
            if (update)
              overflow.item = item;
            if (raise && update)
              this.raiseForUpdate(item, obj);
            item = obj;
            return true;
          }
          bucket = overflow;
        }
        if (add)
          bucket.overflow = new HashSet<T>.Bucket(item, hashval, (HashSet<T>.Bucket) null);
        else
          goto label_15;
      }
      else if (add)
        this.table[index] = new HashSet<T>.Bucket(item, hashval, (HashSet<T>.Bucket) null);
      else
        goto label_15;
      ++this.size;
      if (this.size > this.resizethreshhold)
        this.expand();
label_15:
      if (raise && add)
        this.raiseForAdd(item);
      if (update)
        item = default (T);
      return false;
    }

    private bool remove(ref T item)
    {
      if (this.size == 0)
        return false;
      int index = this.hv2i(this.gethashcode(item));
      HashSet<T>.Bucket bucket1 = this.table[index];
      if (bucket1 == null)
        return false;
      if (this.equals(item, bucket1.item))
      {
        item = bucket1.item;
        this.table[index] = bucket1.overflow;
      }
      else
      {
        HashSet<T>.Bucket bucket2 = bucket1;
        HashSet<T>.Bucket overflow;
        for (overflow = bucket1.overflow; overflow != null && !this.equals(item, overflow.item); overflow = overflow.overflow)
          bucket2 = overflow;
        if (overflow == null)
          return false;
        item = overflow.item;
        bucket2.overflow = overflow.overflow;
      }
      --this.size;
      return true;
    }

    private void clear()
    {
      this.bits = this.origbits;
      this.bitsc = 32 - this.bits;
      this.indexmask = (1 << this.bits) - 1;
      this.size = 0;
      this.table = new HashSet<T>.Bucket[this.indexmask + 1];
      this.resizethreshhold = (int) ((double) this.table.Length * this.fillfactor);
    }

    public HashSet()
      : this(MemoryType.Normal)
    {
    }

    public HashSet(MemoryType memoryType = MemoryType.Normal)
      : this(C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    public HashSet(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : this(16, itemequalityComparer, memoryType)
    {
    }

    public HashSet(int capacity, IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : this(capacity, 0.66, itemequalityComparer, memoryType)
    {
    }

    public HashSet(
      int capacity,
      double fill,
      IEqualityComparer<T> itemequalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(itemequalityComparer, memoryType)
    {
      this._randomhashfactor = Debug.UseDeterministicHashing ? 1529784659U : (uint) ((2 * HashSet<T>.Random.Next() + 1) * 1529784659);
      if (fill < 0.1 || fill > 0.9)
        throw new ArgumentException("Fill outside valid range [0.1, 0.9]");
      if (capacity <= 0)
        throw new ArgumentException("Capacity must be non-negative");
      this.origbits = 4;
      while (capacity - 1 >> this.origbits > 0)
        ++this.origbits;
      this.clear();
      this._hashEnumerator = new HashSet<T>.HashEnumerator(memoryType);
    }

    public virtual Speed ContainsSpeed => Speed.Constant;

    public virtual bool Contains(T item) => this.searchoradd(ref item, false, false, false);

    public virtual bool Find(ref T item) => this.searchoradd(ref item, false, false, false);

    public virtual bool Update(T item)
    {
      this.updatecheck();
      return this.searchoradd(ref item, false, true, true);
    }

    public virtual bool Update(T item, out T olditem)
    {
      this.updatecheck();
      olditem = item;
      return this.searchoradd(ref olditem, false, true, true);
    }

    public virtual bool FindOrAdd(ref T item)
    {
      this.updatecheck();
      return this.searchoradd(ref item, true, false, true);
    }

    public virtual bool UpdateOrAdd(T item)
    {
      this.updatecheck();
      return this.searchoradd(ref item, true, true, true);
    }

    public virtual bool UpdateOrAdd(T item, out T olditem)
    {
      this.updatecheck();
      olditem = item;
      return this.searchoradd(ref olditem, true, true, true);
    }

    public virtual bool Remove(T item)
    {
      this.updatecheck();
      if (!this.remove(ref item))
        return false;
      this.raiseForRemove(item);
      return true;
    }

    public virtual bool Remove(T item, out T removeditem)
    {
      this.updatecheck();
      removeditem = item;
      if (!this.remove(ref removeditem))
        return false;
      this.raiseForRemove(removeditem);
      return true;
    }

    public virtual void RemoveAll(IEnumerable<T> items)
    {
      this.updatecheck();
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) this);
      bool mustFire = removeAllHandler.MustFire;
      foreach (T obj in items)
      {
        var item = obj;
        if (this.remove(ref item) && mustFire)
          removeAllHandler.Remove(item);
      }
      if (!mustFire)
        return;
      removeAllHandler.Raise();
    }

    public virtual void Clear()
    {
      this.updatecheck();
      int size = this.size;
      this.clear();
      if (this.ActiveEvents == EventTypeEnum.None || size <= 0)
        return;
      this.raiseCollectionCleared(true, size);
      this.raiseCollectionChanged();
    }

    public virtual void RetainAll(IEnumerable<T> items)
    {
      this.updatecheck();
      HashSet<T> hashSet = new HashSet<T>(this.EqualityComparer);
      foreach (T obj1 in items)
      {
        if (this.Contains(obj1))
        {
          T obj2 = obj1;
          hashSet.searchoradd(ref obj2, true, false, false);
        }
      }
      if (this.size == hashSet.size)
        return;
      CircularQueue<T> wasRemoved = (CircularQueue<T>) null;
      if ((this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None)
      {
        wasRemoved = new CircularQueue<T>();
        foreach (T obj in (EnumerableBase<T>) this)
        {
          if (!hashSet.Contains(obj))
            wasRemoved.Enqueue(obj);
        }
      }
      this.table = hashSet.table;
      this.size = hashSet.size;
      this.indexmask = hashSet.indexmask;
      this.resizethreshhold = hashSet.resizethreshhold;
      this.bits = hashSet.bits;
      this.bitsc = hashSet.bitsc;
      this._randomhashfactor = hashSet._randomhashfactor;
      if ((this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None)
      {
        this.raiseForRemoveAll((ICollectionValue<T>) wasRemoved);
      }
      else
      {
        if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
          return;
        this.raiseCollectionChanged();
      }
    }

    public virtual bool ContainsAll(IEnumerable<T> items)
    {
      foreach (T obj in items)
      {
        if (!this.Contains(obj))
          return false;
      }
      return true;
    }

    public override T[] ToArray()
    {
      T[] array = new T[this.size];
      int num = 0;
      for (int index = 0; index < this.table.Length; ++index)
      {
        for (HashSet<T>.Bucket overflow = this.table[index]; overflow != null; overflow = overflow.overflow)
          array[num++] = overflow.item;
      }
      return array;
    }

    public virtual int ContainsCount(T item) => !this.Contains(item) ? 0 : 1;

    public virtual ICollectionValue<T> UniqueItems() => (ICollectionValue<T>) this;

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      return (ICollectionValue<KeyValuePair<T, int>>) new MultiplicityOne<T>((ICollectionValue<T>) this);
    }

    public virtual void RemoveAllCopies(T item) => this.Remove(item);

    public override T Choose()
    {
      int length = this.table.Length;
      if (this.size == 0)
        throw new NoSuchItemException();
      do
      {
        if (++this.lastchosen >= length)
          this.lastchosen = 0;
      }
      while (this.table[this.lastchosen] == null);
      return this.table[this.lastchosen].item;
    }

    public override IEnumerator<T> GetEnumerator()
    {
      HashSet<T>.HashEnumerator enumerator = (HashSet<T>.HashEnumerator) this._hashEnumerator.GetEnumerator();
      enumerator.UpdateReference(this, this.stamp);
      return (IEnumerator<T>) enumerator;
    }

    public virtual bool AllowsDuplicates => false;

    public virtual bool DuplicatesByCounting => true;

    public virtual bool Add(T item)
    {
      this.updatecheck();
      return !this.searchoradd(ref item, true, false, true);
    }

    void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

    public virtual void AddAll(IEnumerable<T> items)
    {
      this.updatecheck();
      bool flag1 = false;
      bool flag2 = (this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None;
      CircularQueue<T> circularQueue = flag2 ? new CircularQueue<T>() : (CircularQueue<T>) null;
      foreach (T obj1 in items)
      {
        T obj2 = obj1;
        if (!this.searchoradd(ref obj2, true, false, false))
        {
          flag1 = true;
          if (flag2)
            circularQueue.Enqueue(obj1);
        }
      }
      if (flag2 & flag1)
      {
        foreach (T obj in (EnumerableBase<T>) circularQueue)
          this.raiseItemsAdded(obj, 1);
      }
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None || !flag1)
        return;
      this.raiseCollectionChanged();
    }

    public virtual bool Check()
    {
      int num1 = 0;
      bool flag = true;
      if (this.bitsc != 32 - this.bits)
      {
        Logger.Log(string.Format("bitsc != 32 - bits ({0}, {1})", (object) this.bitsc, (object) this.bits));
        flag = false;
      }
      if (this.indexmask != (1 << this.bits) - 1)
      {
        Logger.Log(string.Format("indexmask != (1 << bits) - 1 ({0}, {1})", (object) this.indexmask, (object) this.bits));
        flag = false;
      }
      if (this.table.Length != this.indexmask + 1)
      {
        Logger.Log(string.Format("table.Length != indexmask + 1 ({0}, {1})", (object) this.table.Length, (object) this.indexmask));
        flag = false;
      }
      if (this.bitsc != 32 - this.bits)
      {
        Logger.Log(string.Format("resizethreshhold != (int)(table.Length * fillfactor) ({0}, {1}, {2})", (object) this.resizethreshhold, (object) this.table.Length, (object) this.fillfactor));
        flag = false;
      }
      int index = 0;
      for (int length = this.table.Length; index < length; ++index)
      {
        int num2 = 0;
        for (HashSet<T>.Bucket overflow = this.table[index]; overflow != null; overflow = overflow.overflow)
        {
          if (index != this.hv2i(overflow.hashval))
          {
            Logger.Log(string.Format("Bad cell item={0}, hashval={1}, index={2}, level={3}", (object) overflow.item, (object) overflow.hashval, (object) index, (object) num2));
            flag = false;
          }
          ++num1;
          ++num2;
        }
      }
      if (num1 != this.size)
      {
        Logger.Log(string.Format("size({0}) != count({1})", (object) this.size, (object) num1));
        flag = false;
      }
      return flag;
    }

    public ISortedDictionary<int, int> BucketCostDistribution()
    {
      TreeDictionary<int, int> treeDictionary = new TreeDictionary<int, int>();
      int index = 0;
      for (int length = this.table.Length; index < length; ++index)
      {
        int key1 = 0;
        for (HashSet<T>.Bucket overflow = this.table[index]; overflow != null; overflow = overflow.overflow)
          ++key1;
        if (treeDictionary.Contains(key1))
        {
          DictionaryBase<int, int> dictionaryBase;
          int key2;
          (dictionaryBase = (DictionaryBase<int, int>) treeDictionary)[key2 = key1] = dictionaryBase[key2] + 1;
        }
        else
          treeDictionary[key1] = 1;
      }
      return (ISortedDictionary<int, int>) treeDictionary;
    }

    [Flags]
    public enum Feature : short
    {
      Dummy = 0,
      RefTypeBucket = 1,
      ValueTypeBucket = 2,
      LinearProbing = 4,
      ShrinkTable = 8,
      Chaining = 16, // 0x0010
      InterHashing = 32, // 0x0020
      RandomInterHashing = 64, // 0x0040
    }

    [Serializable]
    private class Bucket
    {
      internal T item;
      internal int hashval;
      internal HashSet<T>.Bucket overflow;

      internal Bucket(T item, int hashval, HashSet<T>.Bucket overflow)
      {
        this.item = item;
        this.hashval = hashval;
        this.overflow = overflow;
      }
    }

    [Serializable]
    private class HashEnumerator : MemorySafeEnumerator<T>
    {
      private HashSet<T> _hashSet;
      private int _stamp;
      private int _index;
      private HashSet<T>.Bucket b;

      public HashEnumerator(MemoryType memoryType)
        : base(memoryType)
      {
        this._index = -1;
        this.Current = default (T);
      }

      internal void UpdateReference(HashSet<T> hashSet, int theStamp)
      {
        this._hashSet = hashSet;
        this._stamp = theStamp;
        this.Current = default (T);
        this._index = -1;
      }

      public override void Dispose()
      {
        base.Dispose();
        this._index = -1;
        this.b = (HashSet<T>.Bucket) null;
      }

      protected override MemorySafeEnumerator<T> Clone()
      {
        HashSet<T>.HashEnumerator hashEnumerator = new HashSet<T>.HashEnumerator(this.MemoryType);
        hashEnumerator.Current = default (T);
        hashEnumerator._hashSet = this._hashSet;
        return (MemorySafeEnumerator<T>) hashEnumerator;
      }

      public override bool MoveNext()
      {
        int length = this._hashSet.table.Length;
        if (this._stamp != this._hashSet.stamp)
          throw new CollectionModifiedException();
        if (this.b == null || this.b.overflow == null)
        {
          while (++this._index < length)
          {
            if (this._hashSet.table[this._index] != null)
            {
              this.b = this._hashSet.table[this._index];
              this.Current = this.b.item;
              return true;
            }
          }
          return false;
        }
        this.b = this.b.overflow;
        this.Current = this.b.item;
        return true;
      }

      public override void Reset() => throw new NotImplementedException();
    }
  }
}
