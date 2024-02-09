// Decompiled with JetBrains decompiler
// Type: C5.IPriorityQueue`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface IPriorityQueue<T> : 
    IExtensible<T>,
    ICollectionValue<T>,
    IEnumerable<T>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    T FindMin();

    T DeleteMin();

    T FindMax();

    T DeleteMax();

    IComparer<T> Comparer { get; }

    T this[IPriorityQueueHandle<T> handle] { get; set; }

    bool Find(IPriorityQueueHandle<T> handle, out T item);

    bool Add(ref IPriorityQueueHandle<T> handle, T item);

    T Delete(IPriorityQueueHandle<T> handle);

    T Replace(IPriorityQueueHandle<T> handle, T item);

    T FindMin(out IPriorityQueueHandle<T> handle);

    T FindMax(out IPriorityQueueHandle<T> handle);

    T DeleteMin(out IPriorityQueueHandle<T> handle);

    T DeleteMax(out IPriorityQueueHandle<T> handle);
  }
}
