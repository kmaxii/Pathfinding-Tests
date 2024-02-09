// Decompiled with JetBrains decompiler
// Type: C5.ICollection`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface ICollection<T> : 
    IExtensible<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    System.Collections.Generic.ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    Speed ContainsSpeed { get; }

    new int Count { get; }

    new bool IsReadOnly { get; }

    new bool Add(T item);

    new void CopyTo(T[] array, int index);

    int GetUnsequencedHashCode();

    bool UnsequencedEquals(ICollection<T> otherCollection);

    new bool Contains(T item);

    int ContainsCount(T item);

    ICollectionValue<T> UniqueItems();

    ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities();

    bool ContainsAll(IEnumerable<T> items);

    bool Find(ref T item);

    bool FindOrAdd(ref T item);

    bool Update(T item);

    bool Update(T item, out T olditem);

    bool UpdateOrAdd(T item);

    bool UpdateOrAdd(T item, out T olditem);

    new bool Remove(T item);

    bool Remove(T item, out T removeditem);

    void RemoveAllCopies(T item);

    void RemoveAll(IEnumerable<T> items);

    new void Clear();

    void RetainAll(IEnumerable<T> items);
  }
}
