// Decompiled with JetBrains decompiler
// Type: C5.GuardedCollectionValue`1
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
  public class GuardedCollectionValue<T> : 
    GuardedEnumerable<T>,
    ICollectionValue<T>,
    IEnumerable<T>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    private ProxyEventBlock<T> eventBlock;
    private ICollectionValue<T> collectionvalue;

    public virtual EventTypeEnum ListenableEvents => this.collectionvalue.ListenableEvents;

    public virtual EventTypeEnum ActiveEvents => this.collectionvalue.ActiveEvents;

    public event CollectionChangedHandler<T> CollectionChanged
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<T>((ICollectionValue<T>) this, this.collectionvalue))).CollectionChanged += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.CollectionChanged -= value;
      }
    }

    public event CollectionClearedHandler<T> CollectionCleared
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<T>((ICollectionValue<T>) this, this.collectionvalue))).CollectionCleared += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.CollectionCleared -= value;
      }
    }

    public event ItemsAddedHandler<T> ItemsAdded
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<T>((ICollectionValue<T>) this, this.collectionvalue))).ItemsAdded += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemsAdded -= value;
      }
    }

    public event ItemInsertedHandler<T> ItemInserted
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<T>((ICollectionValue<T>) this, this.collectionvalue))).ItemInserted += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemInserted -= value;
      }
    }

    public event ItemsRemovedHandler<T> ItemsRemoved
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<T>((ICollectionValue<T>) this, this.collectionvalue))).ItemsRemoved += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemsRemoved -= value;
      }
    }

    public event ItemRemovedAtHandler<T> ItemRemovedAt
    {
      add
      {
        (this.eventBlock ?? (this.eventBlock = new ProxyEventBlock<T>((ICollectionValue<T>) this, this.collectionvalue))).ItemRemovedAt += value;
      }
      remove
      {
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemRemovedAt -= value;
      }
    }

    public GuardedCollectionValue(ICollectionValue<T> collectionvalue)
      : base((IEnumerable<T>) collectionvalue)
    {
      this.collectionvalue = collectionvalue;
    }

    public virtual bool IsEmpty => this.collectionvalue.IsEmpty;

    public virtual int Count => this.collectionvalue.Count;

    public virtual Speed CountSpeed => this.collectionvalue.CountSpeed;

    public virtual void CopyTo(T[] a, int i) => this.collectionvalue.CopyTo(a, i);

    public virtual T[] ToArray() => this.collectionvalue.ToArray();

    public virtual void Apply(Action<T> a) => this.collectionvalue.Apply(a);

    public virtual bool Exists(Func<T, bool> filter) => this.collectionvalue.Exists(filter);

    public virtual bool Find(Func<T, bool> filter, out T item)
    {
      return this.collectionvalue.Find(filter, out item);
    }

    public virtual bool All(Func<T, bool> filter) => this.collectionvalue.All(filter);

    public virtual IEnumerable<T> Filter(Func<T, bool> filter)
    {
      return this.collectionvalue.Filter(filter);
    }

    public virtual T Choose() => this.collectionvalue.Choose();

    public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
    {
      return this.collectionvalue.Show(stringbuilder, ref rest, formatProvider);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return this.collectionvalue.ToString(format, formatProvider);
    }
  }
}
