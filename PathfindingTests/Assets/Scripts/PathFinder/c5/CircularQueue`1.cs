// Decompiled with JetBrains decompiler
// Type: C5.CircularQueue`1
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
  public class CircularQueue<T> : 
    SequencedBase<T>,
    IQueue<T>,
    IStack<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private int front;
    private int back;
    private T[] array;
    private bool forwards = true;
    private bool original = true;

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    private void expand()
    {
      T[] destinationArray = new T[2 * this.array.Length];
      if (this.front <= this.back)
      {
        Array.Copy((Array) this.array, this.front, (Array) destinationArray, 0, this.size);
      }
      else
      {
        int num = this.array.Length - this.front;
        Array.Copy((Array) this.array, this.front, (Array) destinationArray, 0, num);
        Array.Copy((Array) this.array, 0, (Array) destinationArray, num, this.size - num);
      }
      this.front = 0;
      this.back = this.size;
      this.array = destinationArray;
    }

    public CircularQueue(MemoryType memoryType = MemoryType.Normal)
      : this(8, memoryType)
    {
    }

    public CircularQueue(int capacity, MemoryType memoryType = MemoryType.Normal)
      : base(C5.EqualityComparer<T>.Default, memoryType)
    {
      int length = 8;
      while (length < capacity)
        length *= 2;
      this.array = new T[length];
    }

    public virtual bool AllowsDuplicates => true;

    public virtual T this[int i]
    {
      get
      {
        if (i < 0 || i >= this.size)
          throw new IndexOutOfRangeException();
        i += this.front;
        return this.array[i >= this.array.Length ? i - this.array.Length : i];
      }
    }

    public virtual void Enqueue(T item)
    {
      if (!this.original)
        throw new ReadOnlyCollectionException();
      ++this.stamp;
      if (this.size == this.array.Length - 1)
        this.expand();
      ++this.size;
      int index = this.back++;
      if (this.back == this.array.Length)
        this.back = 0;
      this.array[index] = item;
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseForAdd(item);
    }

    public virtual T Dequeue()
    {
      if (!this.original)
        throw new ReadOnlyCollectionException("Object is a non-updatable clone");
      ++this.stamp;
      if (this.size == 0)
        throw new NoSuchItemException();
      --this.size;
      int index = this.front++;
      if (this.front == this.array.Length)
        this.front = 0;
      T obj = this.array[index];
      this.array[index] = default (T);
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForRemove(obj);
      return obj;
    }

    public void Push(T item)
    {
      if (!this.original)
        throw new ReadOnlyCollectionException();
      ++this.stamp;
      if (this.size == this.array.Length - 1)
        this.expand();
      ++this.size;
      int index = this.back++;
      if (this.back == this.array.Length)
        this.back = 0;
      this.array[index] = item;
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseForAdd(item);
    }

    public T Pop()
    {
      if (!this.original)
        throw new ReadOnlyCollectionException("Object is a non-updatable clone");
      ++this.stamp;
      if (this.size == 0)
        throw new NoSuchItemException();
      --this.size;
      --this.back;
      if (this.back == -1)
        this.back = this.array.Length - 1;
      T obj = this.array[this.back];
      this.array[this.back] = default (T);
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForRemove(obj);
      return obj;
    }

    public override T Choose()
    {
      if (this.size == 0)
        throw new NoSuchItemException();
      return this.array[this.front];
    }

    public override IEnumerator<T> GetEnumerator()
    {
      int stamp = this.stamp;
      if (this.forwards)
      {
        int position = this.front;
        int end = this.front <= this.back ? this.back : this.array.Length;
        while (position < end)
        {
          if (stamp != this.stamp)
            throw new CollectionModifiedException();
          yield return this.array[position++];
        }
        if (this.front > this.back)
        {
          position = 0;
          while (position < this.back)
          {
            if (stamp != this.stamp)
              throw new CollectionModifiedException();
            yield return this.array[position++];
          }
        }
      }
      else
      {
        int position = this.back - 1;
        int end = this.front <= this.back ? this.front : 0;
        while (position >= end)
        {
          if (stamp != this.stamp)
            throw new CollectionModifiedException();
          yield return this.array[position--];
        }
        if (this.front > this.back)
        {
          position = this.array.Length - 1;
          while (position >= this.front)
          {
            if (stamp != this.stamp)
              throw new CollectionModifiedException();
            yield return this.array[position--];
          }
        }
      }
    }

    public override IDirectedCollectionValue<T> Backwards()
    {
      CircularQueue<T> circularQueue = (CircularQueue<T>) this.MemberwiseClone();
      circularQueue.original = false;
      circularQueue.forwards = !this.forwards;
      return (IDirectedCollectionValue<T>) circularQueue;
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public virtual bool Check()
    {
      if (this.front >= 0 && this.front < this.array.Length && this.back >= 0 && this.back < this.array.Length && (this.front > this.back || this.size == this.back - this.front) && (this.front <= this.back || this.size == this.array.Length + this.back - this.front))
        return true;
      Logger.Log(string.Format("Bad combination of (front,back,size,array.Length): ({0},{1},{2},{3})", (object) this.front, (object) this.back, (object) this.size, (object) this.array.Length));
      return false;
    }
  }
}
