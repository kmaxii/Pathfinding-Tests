// Decompiled with JetBrains decompiler
// Type: C5.WrappedArray`1
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
  public class WrappedArray<T> : 
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
    private ArrayList<T> innerlist;
    private WrappedArray<T> underlying;

    public WrappedArray(T[] wrappedarray, MemoryType memoryType = MemoryType.Normal)
    {
      this.innerlist = (ArrayList<T>) new WrappedArray<T>.InnerList(wrappedarray, memoryType);
    }

    private WrappedArray(ArrayList<T> arraylist, WrappedArray<T> underlying)
    {
      this.innerlist = arraylist;
      this.underlying = underlying;
    }

    public T First => this.innerlist.First;

    public T Last => this.innerlist.Last;

    public T this[int index]
    {
      get => this.innerlist[index];
      set => this.innerlist[index] = value;
    }

    public IList<T> FindAll(Func<T, bool> filter) => this.innerlist.FindAll(filter);

    public IList<V> Map<V>(Func<T, V> mapper) => this.innerlist.Map<V>(mapper);

    public IList<V> Map<V>(Func<T, V> mapper, IEqualityComparer<V> equalityComparer)
    {
      return this.innerlist.Map<V>(mapper, equalityComparer);
    }

    public bool FIFO
    {
      get => throw new FixedSizeCollectionException();
      set => throw new FixedSizeCollectionException();
    }

    public virtual bool IsFixedSize => true;

    public void Insert(int index, T item) => throw new FixedSizeCollectionException();

    public void Insert(IList<T> pointer, T item) => throw new FixedSizeCollectionException();

    public void InsertFirst(T item) => throw new FixedSizeCollectionException();

    public void InsertLast(T item) => throw new FixedSizeCollectionException();

    public void InsertAll(int i, IEnumerable<T> items) => throw new FixedSizeCollectionException();

    public T Remove() => throw new FixedSizeCollectionException();

    public T RemoveFirst() => throw new FixedSizeCollectionException();

    public T RemoveLast() => throw new FixedSizeCollectionException();

    public IList<T> View(int start, int count)
    {
      return (IList<T>) new WrappedArray<T>((ArrayList<T>) this.innerlist.View(start, count), this.underlying ?? this);
    }

    public IList<T> ViewOf(T item)
    {
      return (IList<T>) new WrappedArray<T>((ArrayList<T>) this.innerlist.ViewOf(item), this.underlying ?? this);
    }

    public IList<T> LastViewOf(T item)
    {
      return (IList<T>) new WrappedArray<T>((ArrayList<T>) this.innerlist.LastViewOf(item), this.underlying ?? this);
    }

    public IList<T> Underlying => (IList<T>) this.underlying;

    public int Offset => this.innerlist.Offset;

    public bool IsValid => this.innerlist.IsValid;

    public IList<T> Slide(int offset) => this.innerlist.Slide(offset);

    public IList<T> Slide(int offset, int size) => this.innerlist.Slide(offset, size);

    public bool TrySlide(int offset) => this.innerlist.TrySlide(offset);

    public bool TrySlide(int offset, int size) => this.innerlist.TrySlide(offset, size);

    public IList<T> Span(IList<T> otherView)
    {
      return this.innerlist.Span((IList<T>) ((WrappedArray<T>) otherView).innerlist);
    }

    public void Reverse() => this.innerlist.Reverse();

    public bool IsSorted() => this.innerlist.IsSorted();

    public bool IsSorted(IComparer<T> comparer) => this.innerlist.IsSorted(comparer);

    public void Sort() => this.innerlist.Sort();

    public void Sort(IComparer<T> comparer) => this.innerlist.Sort(comparer);

    public void Shuffle() => this.innerlist.Shuffle();

    public void Shuffle(Random rnd) => this.innerlist.Shuffle(rnd);

    public Speed IndexingSpeed => Speed.Constant;

    public IDirectedCollectionValue<T> this[int start, int count] => this.innerlist[start, count];

    public int IndexOf(T item) => this.innerlist.IndexOf(item);

    public int LastIndexOf(T item) => this.innerlist.LastIndexOf(item);

    public int FindIndex(Func<T, bool> predicate) => this.innerlist.FindIndex(predicate);

    public int FindLastIndex(Func<T, bool> predicate) => this.innerlist.FindLastIndex(predicate);

    public T RemoveAt(int i) => throw new FixedSizeCollectionException();

    public void RemoveInterval(int start, int count) => throw new FixedSizeCollectionException();

    public int GetSequencedHashCode() => this.innerlist.GetSequencedHashCode();

    public bool SequencedEquals(ISequenced<T> that) => this.innerlist.SequencedEquals(that);

    public Speed ContainsSpeed => Speed.Linear;

    public int GetUnsequencedHashCode() => this.innerlist.GetUnsequencedHashCode();

    public bool UnsequencedEquals(ICollection<T> that) => this.innerlist.UnsequencedEquals(that);

    public bool Contains(T item) => this.innerlist.Contains(item);

    public int ContainsCount(T item) => this.innerlist.ContainsCount(item);

    public ICollectionValue<T> UniqueItems() => this.innerlist.UniqueItems();

    public ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      return this.innerlist.ItemMultiplicities();
    }

    public bool ContainsAll(IEnumerable<T> items) => this.innerlist.ContainsAll(items);

    public bool Find(ref T item) => this.innerlist.Find(ref item);

    public bool FindOrAdd(ref T item) => throw new FixedSizeCollectionException();

    public bool Update(T item) => throw new FixedSizeCollectionException();

    public bool Update(T item, out T olditem) => throw new FixedSizeCollectionException();

    public bool UpdateOrAdd(T item) => throw new FixedSizeCollectionException();

    public bool UpdateOrAdd(T item, out T olditem) => throw new FixedSizeCollectionException();

    public bool Remove(T item) => throw new FixedSizeCollectionException();

    public bool Remove(T item, out T removeditem) => throw new FixedSizeCollectionException();

    public void RemoveAllCopies(T item) => throw new FixedSizeCollectionException();

    public void RemoveAll(IEnumerable<T> items) => throw new FixedSizeCollectionException();

    public void Clear() => throw new FixedSizeCollectionException();

    public void RetainAll(IEnumerable<T> items) => throw new FixedSizeCollectionException();

    public bool IsReadOnly => true;

    public bool AllowsDuplicates => true;

    public IEqualityComparer<T> EqualityComparer => this.innerlist.EqualityComparer;

    public bool DuplicatesByCounting => false;

    public bool Add(T item) => throw new FixedSizeCollectionException();

    public void AddAll(IEnumerable<T> items) => throw new FixedSizeCollectionException();

    public bool Check()
    {
      if (!this.innerlist.Check())
        return false;
      return this.underlying == null || this.underlying.innerlist == this.innerlist.Underlying;
    }

    public virtual EventTypeEnum ListenableEvents => EventTypeEnum.None;

    public virtual EventTypeEnum ActiveEvents => EventTypeEnum.None;

    public event CollectionChangedHandler<T> CollectionChanged
    {
      add => throw new UnlistenableEventException();
      remove => throw new UnlistenableEventException();
    }

    public event CollectionClearedHandler<T> CollectionCleared
    {
      add => throw new UnlistenableEventException();
      remove => throw new UnlistenableEventException();
    }

    public event ItemsAddedHandler<T> ItemsAdded
    {
      add => throw new UnlistenableEventException();
      remove => throw new UnlistenableEventException();
    }

    public event ItemInsertedHandler<T> ItemInserted
    {
      add => throw new UnlistenableEventException();
      remove => throw new UnlistenableEventException();
    }

    public event ItemsRemovedHandler<T> ItemsRemoved
    {
      add => throw new UnlistenableEventException();
      remove => throw new UnlistenableEventException();
    }

    public event ItemRemovedAtHandler<T> ItemRemovedAt
    {
      add => throw new UnlistenableEventException();
      remove => throw new UnlistenableEventException();
    }

    public bool IsEmpty => this.innerlist.IsEmpty;

    public int Count => this.innerlist.Count;

    public Speed CountSpeed => this.innerlist.CountSpeed;

    public void CopyTo(T[] array, int index) => this.innerlist.CopyTo(array, index);

    public T[] ToArray() => this.innerlist.ToArray();

    public void Apply(Action<T> action) => this.innerlist.Apply(action);

    public bool Exists(Func<T, bool> predicate) => this.innerlist.Exists(predicate);

    public bool Find(Func<T, bool> predicate, out T item)
    {
      return this.innerlist.Find(predicate, out item);
    }

    public bool All(Func<T, bool> predicate) => this.innerlist.All(predicate);

    public T Choose() => this.innerlist.Choose();

    public IEnumerable<T> Filter(Func<T, bool> filter) => this.innerlist.Filter(filter);

    public IEnumerator<T> GetEnumerator() => this.innerlist.GetEnumerator();

    public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
    {
      return this.innerlist.Show(stringbuilder, ref rest, formatProvider);
    }

    public override string ToString() => this.innerlist.ToString();

    public virtual string ToString(string format, IFormatProvider formatProvider)
    {
      return this.innerlist.ToString(format, formatProvider);
    }

    public IDirectedCollectionValue<T> Backwards() => this.innerlist.Backwards();

    public bool FindLast(Func<T, bool> predicate, out T item)
    {
      return this.innerlist.FindLast(predicate, out item);
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public EnumerationDirection Direction => EnumerationDirection.Forwards;

    public void Dispose()
    {
      if (this.underlying == null)
        throw new FixedSizeCollectionException();
      this.innerlist.Dispose();
    }

    void System.Collections.Generic.IList<T>.RemoveAt(int index)
    {
      throw new FixedSizeCollectionException();
    }

    void System.Collections.Generic.ICollection<T>.Add(T item)
    {
      throw new FixedSizeCollectionException();
    }

    bool ICollection.IsSynchronized => false;

    [Obsolete]
    object ICollection.SyncRoot => ((ICollection) this.innerlist).SyncRoot;

    void ICollection.CopyTo(Array arr, int index)
    {
      if (index < 0 || index + this.Count > arr.Length)
        throw new ArgumentOutOfRangeException();
      foreach (T obj in this)
        arr.SetValue((object) obj, index++);
    }

    object IList.this[int index]
    {
      get => (object) this[index];
      set => this[index] = (T) value;
    }

    int IList.Add(object o) => !this.Add((T) o) ? -1 : this.Count - 1;

    bool IList.Contains(object o) => this.Contains((T) o);

    int IList.IndexOf(object o) => Math.Max(-1, this.IndexOf((T) o));

    void IList.Insert(int index, object o) => this.Insert(index, (T) o);

    void IList.Remove(object o) => this.Remove((T) o);

    void IList.RemoveAt(int index) => this.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    [Serializable]
    private class InnerList : ArrayList<T>
    {
      internal InnerList(T[] array, MemoryType memoryType)
        : base(memoryType)
      {
        this.array = array;
        this.size = array.Length;
      }
    }
  }
}
