// Decompiled with JetBrains decompiler
// Type: C5.GuardedCollection`1
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
  public class GuardedCollection<T> : 
    GuardedCollectionValue<T>,
    ICollection<T>,
    IExtensible<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    System.Collections.Generic.ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private ICollection<T> collection;

    public GuardedCollection(ICollection<T> collection)
      : base((ICollectionValue<T>) collection)
    {
      this.collection = collection;
    }

    public virtual bool IsReadOnly => true;

    public virtual Speed ContainsSpeed => this.collection.ContainsSpeed;

    public virtual int GetUnsequencedHashCode() => this.collection.GetUnsequencedHashCode();

    public virtual bool UnsequencedEquals(ICollection<T> that)
    {
      return this.collection.UnsequencedEquals(that);
    }

    public virtual bool Contains(T item) => this.collection.Contains(item);

    public virtual int ContainsCount(T item) => this.collection.ContainsCount(item);

    public virtual ICollectionValue<T> UniqueItems()
    {
      return (ICollectionValue<T>) new GuardedCollectionValue<T>(this.collection.UniqueItems());
    }

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      return (ICollectionValue<KeyValuePair<T, int>>) new GuardedCollectionValue<KeyValuePair<T, int>>(this.collection.ItemMultiplicities());
    }

    public virtual bool ContainsAll(IEnumerable<T> items) => this.collection.ContainsAll(items);

    public virtual bool Find(ref T item) => this.collection.Find(ref item);

    public virtual bool FindOrAdd(ref T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool Update(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool Update(T item, out T olditem)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool UpdateOrAdd(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool UpdateOrAdd(T item, out T olditem)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool Remove(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool Remove(T item, out T removeditem)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual void RemoveAllCopies(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual void RemoveAll(IEnumerable<T> items)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual void Clear()
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual void RetainAll(IEnumerable<T> items)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public virtual bool Check() => this.collection.Check();

    public virtual bool AllowsDuplicates => this.collection.AllowsDuplicates;

    public virtual IEqualityComparer<T> EqualityComparer => this.collection.EqualityComparer;

    public virtual bool DuplicatesByCounting => this.collection.DuplicatesByCounting;

    public override bool IsEmpty => this.collection.IsEmpty;

    public virtual bool Add(T item) => throw new ReadOnlyCollectionException();

    void System.Collections.Generic.ICollection<T>.Add(T item)
    {
      throw new ReadOnlyCollectionException();
    }

    public virtual void AddAll(IEnumerable<T> items) => throw new ReadOnlyCollectionException();
  }
}
