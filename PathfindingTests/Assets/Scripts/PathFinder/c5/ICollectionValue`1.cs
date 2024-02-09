// Decompiled with JetBrains decompiler
// Type: C5.ICollectionValue`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface ICollectionValue<T> : IEnumerable<T>, IEnumerable, IShowable, IFormattable
  {
    EventTypeEnum ListenableEvents { get; }

    EventTypeEnum ActiveEvents { get; }

    event CollectionChangedHandler<T> CollectionChanged;

    event CollectionClearedHandler<T> CollectionCleared;

    event ItemsAddedHandler<T> ItemsAdded;

    event ItemInsertedHandler<T> ItemInserted;

    event ItemsRemovedHandler<T> ItemsRemoved;

    event ItemRemovedAtHandler<T> ItemRemovedAt;

    bool IsEmpty { get; }

    int Count { get; }

    Speed CountSpeed { get; }

    void CopyTo(T[] array, int index);

    T[] ToArray();

    void Apply(Action<T> action);

    bool Exists(Func<T, bool> predicate);

    bool Find(Func<T, bool> predicate, out T item);

    bool All(Func<T, bool> predicate);

    T Choose();

    IEnumerable<T> Filter(Func<T, bool> filter);
  }
}
