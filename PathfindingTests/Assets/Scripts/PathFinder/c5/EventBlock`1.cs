// Decompiled with JetBrains decompiler
// Type: C5.EventBlock`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  internal sealed class EventBlock<T>
  {
    internal EventTypeEnum events;

    private event CollectionChangedHandler<T> collectionChanged;

    internal event CollectionChangedHandler<T> CollectionChanged
    {
      add
      {
        this.collectionChanged += value;
        this.events |= EventTypeEnum.Changed;
      }
      remove
      {
        this.collectionChanged -= value;
        if (this.collectionChanged != null)
          return;
        this.events &= ~EventTypeEnum.Changed;
      }
    }

    internal void raiseCollectionChanged(object sender)
    {
      if (this.collectionChanged == null)
        return;
      this.collectionChanged(sender);
    }

    private event CollectionClearedHandler<T> collectionCleared;

    internal event CollectionClearedHandler<T> CollectionCleared
    {
      add
      {
        this.collectionCleared += value;
        this.events |= EventTypeEnum.Cleared;
      }
      remove
      {
        this.collectionCleared -= value;
        if (this.collectionCleared != null)
          return;
        this.events &= ~EventTypeEnum.Cleared;
      }
    }

    internal void raiseCollectionCleared(object sender, bool full, int count)
    {
      if (this.collectionCleared == null)
        return;
      this.collectionCleared(sender, new ClearedEventArgs(full, count));
    }

    internal void raiseCollectionCleared(object sender, bool full, int count, int? start)
    {
      if (this.collectionCleared == null)
        return;
      this.collectionCleared(sender, (ClearedEventArgs) new ClearedRangeEventArgs(full, count, start));
    }

    private event ItemsAddedHandler<T> itemsAdded;

    internal event ItemsAddedHandler<T> ItemsAdded
    {
      add
      {
        this.itemsAdded += value;
        this.events |= EventTypeEnum.Added;
      }
      remove
      {
        this.itemsAdded -= value;
        if (this.itemsAdded != null)
          return;
        this.events &= ~EventTypeEnum.Added;
      }
    }

    internal void raiseItemsAdded(object sender, T item, int count)
    {
      if (this.itemsAdded == null)
        return;
      this.itemsAdded(sender, new ItemCountEventArgs<T>(item, count));
    }

    private event ItemsRemovedHandler<T> itemsRemoved;

    internal event ItemsRemovedHandler<T> ItemsRemoved
    {
      add
      {
        this.itemsRemoved += value;
        this.events |= EventTypeEnum.Removed;
      }
      remove
      {
        this.itemsRemoved -= value;
        if (this.itemsRemoved != null)
          return;
        this.events &= ~EventTypeEnum.Removed;
      }
    }

    internal void raiseItemsRemoved(object sender, T item, int count)
    {
      if (this.itemsRemoved == null)
        return;
      this.itemsRemoved(sender, new ItemCountEventArgs<T>(item, count));
    }

    private event ItemInsertedHandler<T> itemInserted;

    internal event ItemInsertedHandler<T> ItemInserted
    {
      add
      {
        this.itemInserted += value;
        this.events |= EventTypeEnum.Inserted;
      }
      remove
      {
        this.itemInserted -= value;
        if (this.itemInserted != null)
          return;
        this.events &= ~EventTypeEnum.Inserted;
      }
    }

    internal void raiseItemInserted(object sender, T item, int index)
    {
      if (this.itemInserted == null)
        return;
      this.itemInserted(sender, new ItemAtEventArgs<T>(item, index));
    }

    private event ItemRemovedAtHandler<T> itemRemovedAt;

    internal event ItemRemovedAtHandler<T> ItemRemovedAt
    {
      add
      {
        this.itemRemovedAt += value;
        this.events |= EventTypeEnum.RemovedAt;
      }
      remove
      {
        this.itemRemovedAt -= value;
        if (this.itemRemovedAt != null)
          return;
        this.events &= ~EventTypeEnum.RemovedAt;
      }
    }

    internal void raiseItemRemovedAt(object sender, T item, int index)
    {
      if (this.itemRemovedAt == null)
        return;
      this.itemRemovedAt(sender, new ItemAtEventArgs<T>(item, index));
    }
  }
}
