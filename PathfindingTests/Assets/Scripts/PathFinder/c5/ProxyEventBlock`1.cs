// Decompiled with JetBrains decompiler
// Type: C5.ProxyEventBlock`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  internal sealed class ProxyEventBlock<T>
  {
    private ICollectionValue<T> proxy;
    private ICollectionValue<T> real;
    private CollectionChangedHandler<T> collectionChangedProxy;
    private CollectionClearedHandler<T> collectionClearedProxy;
    private ItemsAddedHandler<T> itemsAddedProxy;
    private ItemInsertedHandler<T> itemInsertedProxy;
    private ItemsRemovedHandler<T> itemsRemovedProxy;
    private ItemRemovedAtHandler<T> itemRemovedAtProxy;

    internal ProxyEventBlock(ICollectionValue<T> proxy, ICollectionValue<T> real)
    {
      this.proxy = proxy;
      this.real = real;
    }

    private event CollectionChangedHandler<T> collectionChanged;

    internal event CollectionChangedHandler<T> CollectionChanged
    {
      add
      {
        if (this.collectionChanged == null)
        {
          if (this.collectionChangedProxy == null)
            this.collectionChangedProxy = (CollectionChangedHandler<T>) (sender => this.collectionChanged((object) this.proxy));
          this.real.CollectionChanged += this.collectionChangedProxy;
        }
        this.collectionChanged += value;
      }
      remove
      {
        this.collectionChanged -= value;
        if (this.collectionChanged != null)
          return;
        this.real.CollectionChanged -= this.collectionChangedProxy;
      }
    }

    private event CollectionClearedHandler<T> collectionCleared;

    internal event CollectionClearedHandler<T> CollectionCleared
    {
      add
      {
        if (this.collectionCleared == null)
        {
          if (this.collectionClearedProxy == null)
            this.collectionClearedProxy = (CollectionClearedHandler<T>) ((sender, e) => this.collectionCleared((object) this.proxy, e));
          this.real.CollectionCleared += this.collectionClearedProxy;
        }
        this.collectionCleared += value;
      }
      remove
      {
        this.collectionCleared -= value;
        if (this.collectionCleared != null)
          return;
        this.real.CollectionCleared -= this.collectionClearedProxy;
      }
    }

    private event ItemsAddedHandler<T> itemsAdded;

    internal event ItemsAddedHandler<T> ItemsAdded
    {
      add
      {
        if (this.itemsAdded == null)
        {
          if (this.itemsAddedProxy == null)
            this.itemsAddedProxy = (ItemsAddedHandler<T>) ((sender, e) => this.itemsAdded((object) this.proxy, e));
          this.real.ItemsAdded += this.itemsAddedProxy;
        }
        this.itemsAdded += value;
      }
      remove
      {
        this.itemsAdded -= value;
        if (this.itemsAdded != null)
          return;
        this.real.ItemsAdded -= this.itemsAddedProxy;
      }
    }

    private event ItemInsertedHandler<T> itemInserted;

    internal event ItemInsertedHandler<T> ItemInserted
    {
      add
      {
        if (this.itemInserted == null)
        {
          if (this.itemInsertedProxy == null)
            this.itemInsertedProxy = (ItemInsertedHandler<T>) ((sender, e) => this.itemInserted((object) this.proxy, e));
          this.real.ItemInserted += this.itemInsertedProxy;
        }
        this.itemInserted += value;
      }
      remove
      {
        this.itemInserted -= value;
        if (this.itemInserted != null)
          return;
        this.real.ItemInserted -= this.itemInsertedProxy;
      }
    }

    private event ItemsRemovedHandler<T> itemsRemoved;

    internal event ItemsRemovedHandler<T> ItemsRemoved
    {
      add
      {
        if (this.itemsRemoved == null)
        {
          if (this.itemsRemovedProxy == null)
            this.itemsRemovedProxy = (ItemsRemovedHandler<T>) ((sender, e) => this.itemsRemoved((object) this.proxy, e));
          this.real.ItemsRemoved += this.itemsRemovedProxy;
        }
        this.itemsRemoved += value;
      }
      remove
      {
        this.itemsRemoved -= value;
        if (this.itemsRemoved != null)
          return;
        this.real.ItemsRemoved -= this.itemsRemovedProxy;
      }
    }

    private event ItemRemovedAtHandler<T> itemRemovedAt;

    internal event ItemRemovedAtHandler<T> ItemRemovedAt
    {
      add
      {
        if (this.itemRemovedAt == null)
        {
          if (this.itemRemovedAtProxy == null)
            this.itemRemovedAtProxy = (ItemRemovedAtHandler<T>) ((sender, e) => this.itemRemovedAt((object) this.proxy, e));
          this.real.ItemRemovedAt += this.itemRemovedAtProxy;
        }
        this.itemRemovedAt += value;
      }
      remove
      {
        this.itemRemovedAt -= value;
        if (this.itemRemovedAt != null)
          return;
        this.real.ItemRemovedAt -= this.itemRemovedAtProxy;
      }
    }
  }
}
