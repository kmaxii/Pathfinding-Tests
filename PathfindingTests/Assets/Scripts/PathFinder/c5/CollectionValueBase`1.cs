// Decompiled with JetBrains decompiler
// Type: C5.CollectionValueBase`1
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
  public abstract class CollectionValueBase<T> : 
    EnumerableBase<T>,
    ICollectionValue<T>,
    IEnumerable<T>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    private EventBlock<T> eventBlock;

    public virtual EventTypeEnum ListenableEvents => EventTypeEnum.None;

    public virtual EventTypeEnum ActiveEvents
    {
      get => this.eventBlock != null ? this.eventBlock.events : EventTypeEnum.None;
    }

    private void checkWillListen(EventTypeEnum eventType)
    {
      if ((this.ListenableEvents & eventType) == EventTypeEnum.None)
        throw new UnlistenableEventException();
    }

    public virtual event CollectionChangedHandler<T> CollectionChanged
    {
      add
      {
        this.checkWillListen(EventTypeEnum.Changed);
        (this.eventBlock ?? (this.eventBlock = new EventBlock<T>())).CollectionChanged += value;
      }
      remove
      {
        this.checkWillListen(EventTypeEnum.Changed);
        if (this.eventBlock == null)
          return;
        this.eventBlock.CollectionChanged -= value;
        if (this.eventBlock.events != EventTypeEnum.None)
          return;
        this.eventBlock = (EventBlock<T>) null;
      }
    }

    protected virtual void raiseCollectionChanged()
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseCollectionChanged((object) this);
    }

    public virtual event CollectionClearedHandler<T> CollectionCleared
    {
      add
      {
        this.checkWillListen(EventTypeEnum.Cleared);
        (this.eventBlock ?? (this.eventBlock = new EventBlock<T>())).CollectionCleared += value;
      }
      remove
      {
        this.checkWillListen(EventTypeEnum.Cleared);
        if (this.eventBlock == null)
          return;
        this.eventBlock.CollectionCleared -= value;
        if (this.eventBlock.events != EventTypeEnum.None)
          return;
        this.eventBlock = (EventBlock<T>) null;
      }
    }

    protected virtual void raiseCollectionCleared(bool full, int count)
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseCollectionCleared((object) this, full, count);
    }

    protected virtual void raiseCollectionCleared(bool full, int count, int? offset)
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseCollectionCleared((object) this, full, count, offset);
    }

    public virtual event ItemsAddedHandler<T> ItemsAdded
    {
      add
      {
        this.checkWillListen(EventTypeEnum.Added);
        (this.eventBlock ?? (this.eventBlock = new EventBlock<T>())).ItemsAdded += value;
      }
      remove
      {
        this.checkWillListen(EventTypeEnum.Added);
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemsAdded -= value;
        if (this.eventBlock.events != EventTypeEnum.None)
          return;
        this.eventBlock = (EventBlock<T>) null;
      }
    }

    protected virtual void raiseItemsAdded(T item, int count)
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseItemsAdded((object) this, item, count);
    }

    public virtual event ItemsRemovedHandler<T> ItemsRemoved
    {
      add
      {
        this.checkWillListen(EventTypeEnum.Removed);
        (this.eventBlock ?? (this.eventBlock = new EventBlock<T>())).ItemsRemoved += value;
      }
      remove
      {
        this.checkWillListen(EventTypeEnum.Removed);
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemsRemoved -= value;
        if (this.eventBlock.events != EventTypeEnum.None)
          return;
        this.eventBlock = (EventBlock<T>) null;
      }
    }

    protected virtual void raiseItemsRemoved(T item, int count)
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseItemsRemoved((object) this, item, count);
    }

    public virtual event ItemInsertedHandler<T> ItemInserted
    {
      add
      {
        this.checkWillListen(EventTypeEnum.Inserted);
        (this.eventBlock ?? (this.eventBlock = new EventBlock<T>())).ItemInserted += value;
      }
      remove
      {
        this.checkWillListen(EventTypeEnum.Inserted);
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemInserted -= value;
        if (this.eventBlock.events != EventTypeEnum.None)
          return;
        this.eventBlock = (EventBlock<T>) null;
      }
    }

    protected virtual void raiseItemInserted(T item, int index)
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseItemInserted((object) this, item, index);
    }

    public virtual event ItemRemovedAtHandler<T> ItemRemovedAt
    {
      add
      {
        this.checkWillListen(EventTypeEnum.RemovedAt);
        (this.eventBlock ?? (this.eventBlock = new EventBlock<T>())).ItemRemovedAt += value;
      }
      remove
      {
        this.checkWillListen(EventTypeEnum.RemovedAt);
        if (this.eventBlock == null)
          return;
        this.eventBlock.ItemRemovedAt -= value;
        if (this.eventBlock.events != EventTypeEnum.None)
          return;
        this.eventBlock = (EventBlock<T>) null;
      }
    }

    protected virtual void raiseItemRemovedAt(T item, int index)
    {
      if (this.eventBlock == null)
        return;
      this.eventBlock.raiseItemRemovedAt((object) this, item, index);
    }

    protected virtual void raiseForSetThis(int index, T value, T item)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemsRemoved(item, 1);
      this.raiseItemRemovedAt(item, index);
      this.raiseItemsAdded(value, 1);
      this.raiseItemInserted(value, index);
      this.raiseCollectionChanged();
    }

    protected virtual void raiseForInsert(int i, T item)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemInserted(item, i);
      this.raiseItemsAdded(item, 1);
      this.raiseCollectionChanged();
    }

    protected void raiseForRemove(T item)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemsRemoved(item, 1);
      this.raiseCollectionChanged();
    }

    protected void raiseForRemove(T item, int count)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemsRemoved(item, count);
      this.raiseCollectionChanged();
    }

    protected void raiseForRemoveAt(int index, T item)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemRemovedAt(item, index);
      this.raiseItemsRemoved(item, 1);
      this.raiseCollectionChanged();
    }

    protected virtual void raiseForUpdate(T newitem, T olditem)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemsRemoved(olditem, 1);
      this.raiseItemsAdded(newitem, 1);
      this.raiseCollectionChanged();
    }

    protected virtual void raiseForUpdate(T newitem, T olditem, int count)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemsRemoved(olditem, count);
      this.raiseItemsAdded(newitem, count);
      this.raiseCollectionChanged();
    }

    protected virtual void raiseForAdd(T item)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseItemsAdded(item, 1);
      this.raiseCollectionChanged();
    }

    protected virtual void raiseForRemoveAll(ICollectionValue<T> wasRemoved)
    {
      if ((this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None)
      {
        foreach (T obj in (IEnumerable<T>) wasRemoved)
          this.raiseItemsRemoved(obj, 1);
      }
      if (wasRemoved == null || wasRemoved.Count <= 0)
        return;
      this.raiseCollectionChanged();
    }

    internal MemoryType MemoryType { get; set; }

    public abstract bool IsEmpty { get; }

    public abstract int Count { get; }

    public abstract Speed CountSpeed { get; }

    public virtual void CopyTo(T[] array, int index)
    {
      if (index < 0 || index + this.Count > array.Length)
        throw new ArgumentOutOfRangeException();
      foreach (T obj in (EnumerableBase<T>) this)
        array[index++] = obj;
    }

    public virtual T[] ToArray()
    {
      T[] array = new T[this.Count];
      int num = 0;
      foreach (T obj in (EnumerableBase<T>) this)
        array[num++] = obj;
      return array;
    }

    public virtual void Apply(Action<T> action)
    {
      foreach (T obj in (EnumerableBase<T>) this)
        action(obj);
    }

    public virtual bool Exists(Func<T, bool> predicate)
    {
      foreach (T obj in (EnumerableBase<T>) this)
      {
        if (predicate(obj))
          return true;
      }
      return false;
    }

    public virtual bool Find(Func<T, bool> predicate, out T item)
    {
      foreach (T obj in (EnumerableBase<T>) this)
      {
        if (predicate(obj))
        {
          item = obj;
          return true;
        }
      }
      item = default (T);
      return false;
    }

    public virtual bool All(Func<T, bool> predicate)
    {
      foreach (T obj in (EnumerableBase<T>) this)
      {
        if (!predicate(obj))
          return false;
      }
      return true;
    }

    public virtual IEnumerable<T> Filter(Func<T, bool> predicate)
    {
      if (this.MemoryType == MemoryType.Strict)
        throw new Exception("This is not a memory safe function and cannot be used in MemoryType.Strict");
      foreach (T item in (EnumerableBase<T>) this)
      {
        if (predicate(item))
          yield return item;
      }
    }

    public abstract T Choose();

    public virtual bool Show(
      StringBuilder stringbuilder,
      ref int rest,
      IFormatProvider formatProvider)
    {
      return Showing.ShowCollectionValue<T>((ICollectionValue<T>) this, stringbuilder, ref rest, formatProvider);
    }

    public virtual string ToString(string format, IFormatProvider formatProvider)
    {
      return Showing.ShowString((IShowable) this, format, formatProvider);
    }

    public override string ToString() => this.ToString((string) null, (IFormatProvider) null);

    [Serializable]
    protected class RaiseForRemoveAllHandler
    {
      private CollectionValueBase<T> collection;
      private CircularQueue<T> wasRemoved;
      private bool wasChanged;
      private bool mustFireRemoved;
      public readonly bool MustFire;

      public RaiseForRemoveAllHandler(CollectionValueBase<T> collection)
      {
        this.collection = collection;
        this.mustFireRemoved = (collection.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None;
        this.MustFire = (collection.ActiveEvents & (EventTypeEnum.Changed | EventTypeEnum.Removed)) != EventTypeEnum.None;
      }

      public void Remove(T item)
      {
        if (this.mustFireRemoved)
        {
          if (this.wasRemoved == null)
            this.wasRemoved = new CircularQueue<T>();
          this.wasRemoved.Enqueue(item);
        }
        if (this.wasChanged)
          return;
        this.wasChanged = true;
      }

      public void Raise()
      {
        if (this.wasRemoved != null)
        {
          foreach (T obj in (EnumerableBase<T>) this.wasRemoved)
            this.collection.raiseItemsRemoved(obj, 1);
        }
        if (!this.wasChanged)
          return;
        this.collection.raiseCollectionChanged();
      }
    }
  }
}
