// Decompiled with JetBrains decompiler
// Type: C5.IList`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface IList<T> : 
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
    System.Collections.Generic.IList<T>,
    System.Collections.Generic.ICollection<T>,
    IEnumerable<T>,
    IList,
    ICollection,
    IEnumerable
  {
    T First { get; }

    T Last { get; }

    bool FIFO { get; set; }

    new T this[int index] { get; set; }

    new int Count { get; }

    new bool IsReadOnly { get; }

    new bool Add(T item);

    new void Clear();

    new bool Contains(T item);

    new void CopyTo(T[] array, int index);

    new bool Remove(T item);

    new int IndexOf(T item);

    new T RemoveAt(int index);

    void Insert(IList<T> pointer, T item);

    void InsertFirst(T item);

    void InsertLast(T item);

    void InsertAll(int index, IEnumerable<T> items);

    IList<T> FindAll(Func<T, bool> filter);

    IList<V> Map<V>(Func<T, V> mapper);

    IList<V> Map<V>(Func<T, V> mapper, IEqualityComparer<V> equalityComparer);

    T Remove();

    T RemoveFirst();

    T RemoveLast();

    IList<T> View(int start, int count);

    IList<T> ViewOf(T item);

    IList<T> LastViewOf(T item);

    IList<T> Underlying { get; }

    int Offset { get; }

    bool IsValid { get; }

    IList<T> Slide(int offset);

    IList<T> Slide(int offset, int size);

    bool TrySlide(int offset);

    bool TrySlide(int offset, int size);

    IList<T> Span(IList<T> otherView);

    void Reverse();

    bool IsSorted();

    bool IsSorted(IComparer<T> comparer);

    void Sort();

    void Sort(IComparer<T> comparer);

    void Shuffle();

    void Shuffle(Random rnd);
  }
}
