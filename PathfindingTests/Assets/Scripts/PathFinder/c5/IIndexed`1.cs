// Decompiled with JetBrains decompiler
// Type: C5.IIndexed`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface IIndexed<T> : 
    ISequenced<T>,
    ICollection<T>,
    IExtensible<T>,
    System.Collections.Generic.ICollection<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    Speed IndexingSpeed { get; }

    IDirectedCollectionValue<T> this[int start, int count] { get; }

    int IndexOf(T item);

    int LastIndexOf(T item);

    int FindIndex(Func<T, bool> predicate);

    int FindLastIndex(Func<T, bool> predicate);

    T RemoveAt(int index);

    void RemoveInterval(int start, int count);
  }
}
