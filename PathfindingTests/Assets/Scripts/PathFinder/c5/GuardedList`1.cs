// Decompiled with JetBrains decompiler
// Type: C5.GuardedList`1
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
  public class GuardedList<T> : 
    GuardedSequenced<T>,
    IList<T>,
    IIndexed<T>,
    ISequenced<T>,
    ICollection<T>,
    IExtensible<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IDisposable,
    IList,
    ICollection,
    System.Collections.Generic.IList<T>,
    System.Collections.Generic.ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private IList<T> innerlist;
    private GuardedList<T> underlying;
    private bool slidableView;

    public GuardedList(IList<T> list)
      : base((ISequenced<T>) list)
    {
      this.innerlist = list;
      if (list.Underlying == null)
        return;
      this.underlying = new GuardedList<T>(list.Underlying, (GuardedList<T>) null, false);
    }

    private GuardedList(IList<T> list, GuardedList<T> underlying, bool slidableView)
      : base((ISequenced<T>) list)
    {
      this.innerlist = list;
      this.underlying = underlying;
      this.slidableView = slidableView;
    }

    public T First => this.innerlist.First;

    public T Last => this.innerlist.Last;

    public bool FIFO
    {
      get => this.innerlist.FIFO;
      set => throw new ReadOnlyCollectionException("List is read only");
    }

    public virtual bool IsFixedSize => true;

    public T this[int i]
    {
      get => this.innerlist[i];
      set => throw new ReadOnlyCollectionException("List is read only");
    }

    public virtual Speed IndexingSpeed => this.innerlist.IndexingSpeed;

    public void Insert(int index, T item) => throw new ReadOnlyCollectionException();

    public void Insert(IList<T> pointer, T item) => throw new ReadOnlyCollectionException();

    public void InsertFirst(T item) => throw new ReadOnlyCollectionException("List is read only");

    public void InsertLast(T item) => throw new ReadOnlyCollectionException("List is read only");

    public void InsertBefore(T item, T target)
    {
      throw new ReadOnlyCollectionException("List is read only");
    }

    public void InsertAfter(T item, T target)
    {
      throw new ReadOnlyCollectionException("List is read only");
    }

    public void InsertAll(int i, IEnumerable<T> items)
    {
      throw new ReadOnlyCollectionException("List is read only");
    }

    public IList<T> FindAll(Func<T, bool> filter) => this.innerlist.FindAll(filter);

    public IList<V> Map<V>(Func<T, V> mapper) => this.innerlist.Map<V>(mapper);

    public IList<V> Map<V>(Func<T, V> mapper, IEqualityComparer<V> itemequalityComparer)
    {
      return this.innerlist.Map<V>(mapper, itemequalityComparer);
    }

    public T Remove() => throw new ReadOnlyCollectionException("List is read only");

    public T RemoveFirst() => throw new ReadOnlyCollectionException("List is read only");

    public T RemoveLast() => throw new ReadOnlyCollectionException("List is read only");

    public IList<T> View(int start, int count)
    {
      IList<T> list = this.innerlist.View(start, count);
      return list != null ? (IList<T>) new GuardedList<T>(list, this.underlying ?? this, true) : (IList<T>) null;
    }

    public IList<T> ViewOf(T item)
    {
      IList<T> list = this.innerlist.ViewOf(item);
      return list != null ? (IList<T>) new GuardedList<T>(list, this.underlying ?? this, true) : (IList<T>) null;
    }

    public IList<T> LastViewOf(T item)
    {
      IList<T> list = this.innerlist.LastViewOf(item);
      return list != null ? (IList<T>) new GuardedList<T>(list, this.underlying ?? this, true) : (IList<T>) null;
    }

    public IList<T> Underlying => (IList<T>) this.underlying;

    public int Offset => this.innerlist.Offset;

    public virtual bool IsValid => this.innerlist.IsValid;

    public IList<T> Slide(int offset)
    {
      if (!this.slidableView)
        throw new ReadOnlyCollectionException("List is read only");
      this.innerlist.Slide(offset);
      return (IList<T>) this;
    }

    public IList<T> Slide(int offset, int size)
    {
      if (!this.slidableView)
        throw new ReadOnlyCollectionException("List is read only");
      this.innerlist.Slide(offset, size);
      return (IList<T>) this;
    }

    public bool TrySlide(int offset)
    {
      if (this.slidableView)
        return this.innerlist.TrySlide(offset);
      throw new ReadOnlyCollectionException("List is read only");
    }

    public bool TrySlide(int offset, int size)
    {
      if (this.slidableView)
        return this.innerlist.TrySlide(offset, size);
      throw new ReadOnlyCollectionException("List is read only");
    }

    public IList<T> Span(IList<T> otherView)
    {
      IList<T> list = otherView is GuardedList<T> guardedList ? this.innerlist.Span(guardedList.innerlist) : throw new IncompatibleViewException();
      return list == null ? (IList<T>) null : (IList<T>) new GuardedList<T>(list, this.underlying ?? guardedList.underlying ?? this, true);
    }

    public void Reverse() => throw new ReadOnlyCollectionException("List is read only");

    public void Reverse(int start, int count)
    {
      throw new ReadOnlyCollectionException("List is read only");
    }

    public bool IsSorted() => this.innerlist.IsSorted((IComparer<T>) Comparer<T>.Default);

    public bool IsSorted(IComparer<T> c) => this.innerlist.IsSorted(c);

    public void Sort() => throw new ReadOnlyCollectionException("List is read only");

    public void Sort(IComparer<T> c) => throw new ReadOnlyCollectionException("List is read only");

    public void Shuffle() => throw new ReadOnlyCollectionException("List is read only");

    public void Shuffle(Random rnd) => throw new ReadOnlyCollectionException("List is read only");

    public IDirectedCollectionValue<T> this[int start, int end]
    {
      get
      {
        return (IDirectedCollectionValue<T>) new GuardedDirectedCollectionValue<T>(this.innerlist[start, end]);
      }
    }

    public int IndexOf(T item) => this.innerlist.IndexOf(item);

    public int LastIndexOf(T item) => this.innerlist.LastIndexOf(item);

    public T RemoveAt(int i) => throw new ReadOnlyCollectionException("List is read only");

    public void RemoveInterval(int start, int count)
    {
      throw new ReadOnlyCollectionException("List is read only");
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public void Push(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public T Pop()
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public void Enqueue(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public T Dequeue()
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    public void Dispose()
    {
    }

    void System.Collections.Generic.IList<T>.RemoveAt(int index)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    void System.Collections.Generic.ICollection<T>.Add(T item)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    bool ICollection.IsSynchronized => false;

    [Obsolete]
    object ICollection.SyncRoot => this.innerlist.SyncRoot;

    void ICollection.CopyTo(Array arr, int index)
    {
      if (index < 0 || index + this.Count > arr.Length)
        throw new ArgumentOutOfRangeException();
      foreach (T obj in (GuardedEnumerable<T>) this)
        arr.SetValue((object) obj, index++);
    }

    object IList.this[int index]
    {
      get => (object) this[index];
      set
      {
        throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
      }
    }

    int IList.Add(object o)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    bool IList.Contains(object o) => this.Contains((T) o);

    int IList.IndexOf(object o) => Math.Max(-1, this.IndexOf((T) o));

    void IList.Insert(int index, object o)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    void IList.Remove(object o)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }

    void IList.RemoveAt(int index)
    {
      throw new ReadOnlyCollectionException("Collection cannot be modified through this guard object");
    }
  }
}
