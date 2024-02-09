// Decompiled with JetBrains decompiler
// Type: C5.GuardedQueue`1
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
  public class GuardedQueue<T> : 
    GuardedDirectedCollectionValue<T>,
    IQueue<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private IQueue<T> queue;

    public GuardedQueue(IQueue<T> queue)
      : base((IDirectedCollectionValue<T>) queue)
    {
      this.queue = queue;
    }

    public bool AllowsDuplicates => this.queue.AllowsDuplicates;

    public T this[int i] => this.queue[i];

    public void Enqueue(T item)
    {
      throw new ReadOnlyCollectionException("Queue cannot be modified through this guard object");
    }

    public T Dequeue()
    {
      throw new ReadOnlyCollectionException("Queue cannot be modified through this guard object");
    }
  }
}
