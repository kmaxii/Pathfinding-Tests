// Decompiled with JetBrains decompiler
// Type: C5.HashBag`1
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
  public class HashBag<T> : 
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
    private HashSet<KeyValuePair<T, int>> dict;
    private T _item;

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    public HashBag(MemoryType memoryType = MemoryType.Normal)
      : this(C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    public HashBag(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : base(itemequalityComparer, memoryType)
    {
      this.dict = memoryType == MemoryType.Normal ? new HashSet<KeyValuePair<T, int>>((IEqualityComparer<KeyValuePair<T, int>>) new KeyValuePairEqualityComparer<T, int>(itemequalityComparer), memoryType) : throw new Exception("HashBag doesn't still support Safe and Strict memory type.");
    }

    public HashBag(int capacity, IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : base(itemequalityComparer, memoryType)
    {
      if (memoryType != MemoryType.Normal)
        throw new Exception("HashBag doesn't still support Safe and Strict memory type.");
      this.dict = new HashSet<KeyValuePair<T, int>>(capacity, (IEqualityComparer<KeyValuePair<T, int>>) new KeyValuePairEqualityComparer<T, int>(itemequalityComparer), memoryType);
    }

    public HashBag(
      int capacity,
      double fill,
      IEqualityComparer<T> itemequalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(itemequalityComparer, memoryType)
    {
      if (memoryType != MemoryType.Normal)
        throw new Exception("HashBag doesn't still support Safe and Strict memory type.");
      this.dict = new HashSet<KeyValuePair<T, int>>(capacity, fill, (IEqualityComparer<KeyValuePair<T, int>>) new KeyValuePairEqualityComparer<T, int>(itemequalityComparer), memoryType);
    }

    public virtual Speed ContainsSpeed => Speed.Constant;

    public virtual bool Contains(T item) => this.dict.Contains(new KeyValuePair<T, int>(item, 0));

    public virtual bool Find(ref T item)
    {
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 0);
      if (!this.dict.Find(ref keyValuePair))
        return false;
      item = keyValuePair.Key;
      return true;
    }

    public virtual bool Update(T item)
    {
      T olditem = default (T);
      return this.Update(item, out olditem);
    }

    public virtual bool Update(T item, out T olditem)
    {
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 0);
      this.updatecheck();
      if (this.dict.Find(ref keyValuePair))
      {
        olditem = keyValuePair.Key;
        keyValuePair.Key = item;
        this.dict.Update(keyValuePair);
        if (this.ActiveEvents != EventTypeEnum.None)
          this.raiseForUpdate(item, olditem, keyValuePair.Value);
        return true;
      }
      olditem = default (T);
      return false;
    }

    public virtual bool FindOrAdd(ref T item)
    {
      this.updatecheck();
      if (this.Find(ref item))
        return true;
      this.Add(item);
      return false;
    }

    public virtual bool UpdateOrAdd(T item)
    {
      this.updatecheck();
      if (this.Update(item))
        return true;
      this.Add(item);
      return false;
    }

    public virtual bool UpdateOrAdd(T item, out T olditem)
    {
      this.updatecheck();
      if (this.Update(item, out olditem))
        return true;
      this.Add(item);
      return false;
    }

    public virtual bool Remove(T item)
    {
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 0);
      this.updatecheck();
      if (!this.dict.Find(ref keyValuePair))
        return false;
      --this.size;
      if (keyValuePair.Value == 1)
      {
        this.dict.Remove(keyValuePair);
      }
      else
      {
        --keyValuePair.Value;
        this.dict.Update(keyValuePair);
      }
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForRemove(keyValuePair.Key);
      return true;
    }

    public virtual bool Remove(T item, out T removeditem)
    {
      this.updatecheck();
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 0);
      if (this.dict.Find(ref keyValuePair))
      {
        removeditem = keyValuePair.Key;
        --this.size;
        if (keyValuePair.Value == 1)
        {
          this.dict.Remove(keyValuePair);
        }
        else
        {
          --keyValuePair.Value;
          this.dict.Update(keyValuePair);
        }
        if (this.ActiveEvents != EventTypeEnum.None)
          this.raiseForRemove(removeditem);
        return true;
      }
      removeditem = default (T);
      return false;
    }

    public virtual void RemoveAll(IEnumerable<T> items)
    {
      this.updatecheck();
      bool flag = (this.ActiveEvents & (EventTypeEnum.Changed | EventTypeEnum.Removed)) != EventTypeEnum.None;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = flag ? new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) this) : (CollectionValueBase<T>.RaiseForRemoveAllHandler) null;
      foreach (T key in items)
      {
        KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(key, 0);
        if (this.dict.Find(ref keyValuePair))
        {
          --this.size;
          if (keyValuePair.Value == 1)
          {
            this.dict.Remove(keyValuePair);
          }
          else
          {
            --keyValuePair.Value;
            this.dict.Update(keyValuePair);
          }
          if (flag)
            removeAllHandler.Remove(keyValuePair.Key);
        }
      }
      if (!flag)
        return;
      removeAllHandler.Raise();
    }

    public virtual void Clear()
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      this.dict.Clear();
      int size = this.size;
      this.size = 0;
      if ((this.ActiveEvents & EventTypeEnum.Cleared) != EventTypeEnum.None)
        this.raiseCollectionCleared(true, size);
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public virtual void RetainAll(IEnumerable<T> items)
    {
      this.updatecheck();
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      foreach (T key in items)
      {
        KeyValuePair<T, int> keyValuePair1 = new KeyValuePair<T, int>(key);
        if (this.dict.Find(ref keyValuePair1))
        {
          KeyValuePair<T, int> keyValuePair2 = keyValuePair1;
          if (hashBag.dict.Find(ref keyValuePair2))
          {
            if (keyValuePair2.Value < keyValuePair1.Value)
            {
              ++keyValuePair2.Value;
              hashBag.dict.Update(keyValuePair2);
              ++hashBag.size;
            }
          }
          else
          {
            keyValuePair2.Value = 1;
            hashBag.dict.Add(keyValuePair2);
            ++hashBag.size;
          }
        }
      }
      if (this.size == hashBag.size)
        return;
      CircularQueue<T> wasRemoved = (CircularQueue<T>) null;
      if ((this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None)
      {
        wasRemoved = new CircularQueue<T>();
        foreach (KeyValuePair<T, int> keyValuePair in (EnumerableBase<KeyValuePair<T, int>>) this.dict)
        {
          int num = keyValuePair.Value - hashBag.ContainsCount(keyValuePair.Key);
          if (num > 0)
          {
            for (int index = 0; index < num; ++index)
              wasRemoved.Enqueue(keyValuePair.Key);
          }
        }
      }
      this.dict = hashBag.dict;
      this.size = hashBag.size;
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
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      foreach (T obj in items)
      {
        if (hashBag.ContainsCount(obj) >= this.ContainsCount(obj))
          return false;
        hashBag.Add(obj);
      }
      return true;
    }

    public override T[] ToArray()
    {
      T[] array = new T[this.size];
      int num = 0;
      foreach (KeyValuePair<T, int> keyValuePair in (EnumerableBase<KeyValuePair<T, int>>) this.dict)
      {
        for (int index = 0; index < keyValuePair.Value; ++index)
          array[num++] = keyValuePair.Key;
      }
      return array;
    }

    public virtual int ContainsCount(T item)
    {
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 0);
      return this.dict.Find(ref keyValuePair) ? keyValuePair.Value : 0;
    }

    public virtual ICollectionValue<T> UniqueItems()
    {
      return (ICollectionValue<T>) new DropMultiplicity<T>((ICollectionValue<KeyValuePair<T, int>>) this.dict);
    }

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      return (ICollectionValue<KeyValuePair<T, int>>) new GuardedCollectionValue<KeyValuePair<T, int>>((ICollectionValue<KeyValuePair<T, int>>) this.dict);
    }

    public virtual void RemoveAllCopies(T item)
    {
      this.updatecheck();
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 0);
      if (!this.dict.Find(ref keyValuePair))
        return;
      this.size -= keyValuePair.Value;
      this.dict.Remove(keyValuePair);
      if ((this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None)
        this.raiseItemsRemoved(keyValuePair.Key, keyValuePair.Value);
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public override void CopyTo(T[] array, int index)
    {
      if (index < 0 || index + this.Count > array.Length)
        throw new ArgumentOutOfRangeException();
      foreach (KeyValuePair<T, int> keyValuePair in (EnumerableBase<KeyValuePair<T, int>>) this.dict)
      {
        for (int index1 = 0; index1 < keyValuePair.Value; ++index1)
          array[index++] = keyValuePair.Key;
      }
    }

    public virtual bool AllowsDuplicates => true;

    public virtual bool DuplicatesByCounting => true;

    public virtual bool Add(T item)
    {
      this.updatecheck();
      this.add(ref item);
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForAdd(item);
      return true;
    }

    void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

    private void add(ref T item)
    {
      KeyValuePair<T, int> keyValuePair = new KeyValuePair<T, int>(item, 1);
      if (this.dict.Find(ref keyValuePair))
      {
        ++keyValuePair.Value;
        this.dict.Update(keyValuePair);
        item = keyValuePair.Key;
      }
      else
        this.dict.Add(keyValuePair);
      ++this.size;
    }

    public virtual void AddAll(IEnumerable<T> items)
    {
      this.updatecheck();
      bool flag1 = (this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None;
      CircularQueue<T> circularQueue = flag1 ? new CircularQueue<T>() : (CircularQueue<T>) null;
      bool flag2 = false;
      foreach (T obj in items)
      {
        _item = obj;
        this.add(ref _item);
        flag2 = true;
        if (flag1)
          circularQueue.Enqueue(_item);
      }
      if (!flag2)
        return;
      if (flag1)
      {
        foreach (T obj in (EnumerableBase<T>) circularQueue)
          this.raiseItemsAdded(obj, 1);
      }
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public override T Choose() => this.dict.Choose().Key;

    public override IEnumerator<T> GetEnumerator()
    {
      int mystamp = this.stamp;
      foreach (KeyValuePair<T, int> p in (EnumerableBase<KeyValuePair<T, int>>) this.dict)
      {
        int left = p.Value;
        while (left > 0)
        {
          if (mystamp != this.stamp)
            throw new CollectionModifiedException();
          --left;
          yield return p.Key;
        }
      }
    }

    public virtual bool Check()
    {
      bool flag = this.dict.Check();
      int num = 0;
      foreach (KeyValuePair<T, int> keyValuePair in (EnumerableBase<KeyValuePair<T, int>>) this.dict)
        num += keyValuePair.Value;
      if (num != this.size)
      {
        Logger.Log(string.Format("count({0}) != size({1})", (object) num, (object) this.size));
        flag = false;
      }
      return flag;
    }
  }
}
