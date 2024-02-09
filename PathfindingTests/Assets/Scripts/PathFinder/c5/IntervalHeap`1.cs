// Decompiled with JetBrains decompiler
// Type: C5.IntervalHeap`1
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
  public class IntervalHeap<T> : 
    CollectionValueBase<T>,
    IPriorityQueue<T>,
    IExtensible<T>,
    ICollectionValue<T>,
    IEnumerable<T>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    private int stamp;
    private IComparer<T> comparer;
    private IEqualityComparer<T> itemequalityComparer;
    private IntervalHeap<T>.Interval[] heap;
    private int size;

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    private void swapFirstWithLast(int cell1, int cell2)
    {
      T first = this.heap[cell1].first;
      IntervalHeap<T>.Handle firsthandle = this.heap[cell1].firsthandle;
      this.updateFirst(cell1, this.heap[cell2].last, this.heap[cell2].lasthandle);
      this.updateLast(cell2, first, firsthandle);
    }

    private void swapLastWithLast(int cell1, int cell2)
    {
      T last = this.heap[cell2].last;
      IntervalHeap<T>.Handle lasthandle = this.heap[cell2].lasthandle;
      this.updateLast(cell2, this.heap[cell1].last, this.heap[cell1].lasthandle);
      this.updateLast(cell1, last, lasthandle);
    }

    private void swapFirstWithFirst(int cell1, int cell2)
    {
      T first = this.heap[cell2].first;
      IntervalHeap<T>.Handle firsthandle = this.heap[cell2].firsthandle;
      this.updateFirst(cell2, this.heap[cell1].first, this.heap[cell1].firsthandle);
      this.updateFirst(cell1, first, firsthandle);
    }

    private bool heapifyMin(int cell)
    {
      bool flag = false;
      if (2 * cell + 1 < this.size && this.comparer.Compare(this.heap[cell].first, this.heap[cell].last) > 0)
      {
        flag = true;
        this.swapFirstWithLast(cell, cell);
      }
      int index1 = cell;
      int index2 = 2 * cell + 1;
      int index3 = index2 + 1;
      if (2 * index2 < this.size && this.comparer.Compare(this.heap[index2].first, this.heap[index1].first) < 0)
        index1 = index2;
      if (2 * index3 < this.size && this.comparer.Compare(this.heap[index3].first, this.heap[index1].first) < 0)
        index1 = index3;
      if (index1 != cell)
      {
        this.swapFirstWithFirst(index1, cell);
        this.heapifyMin(index1);
      }
      return flag;
    }

    private bool heapifyMax(int cell)
    {
      bool flag1 = false;
      if (2 * cell + 1 < this.size && this.comparer.Compare(this.heap[cell].last, this.heap[cell].first) < 0)
      {
        flag1 = true;
        this.swapFirstWithLast(cell, cell);
      }
      int index1 = cell;
      int index2 = 2 * cell + 1;
      int index3 = index2 + 1;
      bool flag2 = false;
      if (2 * index2 + 1 < this.size)
      {
        if (this.comparer.Compare(this.heap[index2].last, this.heap[index1].last) > 0)
          index1 = index2;
      }
      else if (2 * index2 + 1 == this.size && this.comparer.Compare(this.heap[index2].first, this.heap[index1].last) > 0)
      {
        index1 = index2;
        flag2 = true;
      }
      if (2 * index3 + 1 < this.size)
      {
        if (this.comparer.Compare(this.heap[index3].last, this.heap[index1].last) > 0)
          index1 = index3;
      }
      else if (2 * index3 + 1 == this.size && this.comparer.Compare(this.heap[index3].first, this.heap[index1].last) > 0)
      {
        index1 = index3;
        flag2 = true;
      }
      if (index1 != cell)
      {
        if (flag2)
          this.swapFirstWithLast(index1, cell);
        else
          this.swapLastWithLast(index1, cell);
        this.heapifyMax(index1);
      }
      return flag1;
    }

    private void bubbleUpMin(int i)
    {
      if (i <= 0)
        return;
      T first1 = this.heap[i].first;
      IntervalHeap<T>.Handle firsthandle = this.heap[i].firsthandle;
      int num = (i + 1) / 2 - 1;
      int index;
      T first2;
      for (; i > 0 && this.comparer.Compare(first1, first2 = this.heap[index = (i + 1) / 2 - 1].first) < 0; i = index)
        this.updateFirst(i, first2, this.heap[index].firsthandle);
      this.updateFirst(i, first1, firsthandle);
    }

    private void bubbleUpMax(int i)
    {
      if (i <= 0)
        return;
      T last1 = this.heap[i].last;
      IntervalHeap<T>.Handle lasthandle = this.heap[i].lasthandle;
      int num = (i + 1) / 2 - 1;
      int index;
      T last2;
      for (; i > 0 && this.comparer.Compare(last1, last2 = this.heap[index = (i + 1) / 2 - 1].last) > 0; i = index)
        this.updateLast(i, last2, this.heap[index].lasthandle);
      this.updateLast(i, last1, lasthandle);
    }

    public IntervalHeap(MemoryType memoryType = MemoryType.Normal)
      : this(16, memoryType)
    {
    }

    public IntervalHeap(IComparer<T> comparer, MemoryType memoryType = MemoryType.Normal)
      : this(16, comparer, memoryType)
    {
    }

    public IntervalHeap(int capacity, MemoryType memoryType = MemoryType.Normal)
      : this(capacity, (IComparer<T>) System.Collections.Generic.Comparer<T>.Default, C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    public IntervalHeap(int capacity, IComparer<T> comparer, MemoryType memoryType = MemoryType.Normal)
      : this(capacity, comparer, (IEqualityComparer<T>) new ComparerZeroHashCodeEqualityComparer<T>(comparer), memoryType)
    {
    }

    private IntervalHeap(
      int capacity,
      IComparer<T> comparer,
      IEqualityComparer<T> itemequalityComparer,
      MemoryType memoryType = MemoryType.Normal)
    {
      if (comparer == null)
        throw new NullReferenceException("Item comparer cannot be null");
      if (itemequalityComparer == null)
        throw new NullReferenceException("Item equality comparer cannot be null");
      if (memoryType != MemoryType.Normal)
        throw new Exception("IntervalHeap still doesn't support MemoryType Strict or Safe");
      this.comparer = comparer;
      this.itemequalityComparer = itemequalityComparer;
      int length = 1;
      while (length < capacity)
        length <<= 1;
      this.heap = new IntervalHeap<T>.Interval[length];
    }

    public T FindMin()
    {
      if (this.size == 0)
        throw new NoSuchItemException();
      return this.heap[0].first;
    }

    public T DeleteMin()
    {
      IPriorityQueueHandle<T> handle = (IPriorityQueueHandle<T>) null;
      return this.DeleteMin(out handle);
    }

    public T FindMax()
    {
      if (this.size == 0)
        throw new NoSuchItemException("Heap is empty");
      return this.size == 1 ? this.heap[0].first : this.heap[0].last;
    }

    public T DeleteMax()
    {
      IPriorityQueueHandle<T> handle = (IPriorityQueueHandle<T>) null;
      return this.DeleteMax(out handle);
    }

    public IComparer<T> Comparer => this.comparer;

    public bool IsReadOnly => false;

    public bool AllowsDuplicates => true;

    public virtual IEqualityComparer<T> EqualityComparer => this.itemequalityComparer;

    public virtual bool DuplicatesByCounting => false;

    public bool Add(T item)
    {
      ++this.stamp;
      if (!this.add((IntervalHeap<T>.Handle) null, item))
        return false;
      this.raiseItemsAdded(item, 1);
      this.raiseCollectionChanged();
      return true;
    }

    private bool add(IntervalHeap<T>.Handle itemhandle, T item)
    {
      if (this.size == 0)
      {
        this.size = 1;
        this.updateFirst(0, item, itemhandle);
        return true;
      }
      if (this.size == 2 * this.heap.Length)
      {
        IntervalHeap<T>.Interval[] destinationArray = new IntervalHeap<T>.Interval[2 * this.heap.Length];
        Array.Copy((Array) this.heap, (Array) destinationArray, this.heap.Length);
        this.heap = destinationArray;
      }
      if (this.size % 2 == 0)
      {
        int num = this.size / 2;
        int index = (num + 1) / 2 - 1;
        T last = this.heap[index].last;
        if (this.comparer.Compare(item, last) > 0)
        {
          this.updateFirst(num, last, this.heap[index].lasthandle);
          this.updateLast(index, item, itemhandle);
          this.bubbleUpMax(index);
        }
        else
        {
          this.updateFirst(num, item, itemhandle);
          if (this.comparer.Compare(item, this.heap[index].first) < 0)
            this.bubbleUpMin(num);
        }
      }
      else
      {
        int index = this.size / 2;
        T first = this.heap[index].first;
        if (this.comparer.Compare(item, first) < 0)
        {
          this.updateLast(index, first, this.heap[index].firsthandle);
          this.updateFirst(index, item, itemhandle);
          this.bubbleUpMin(index);
        }
        else
        {
          this.updateLast(index, item, itemhandle);
          this.bubbleUpMax(index);
        }
      }
      ++this.size;
      return true;
    }

    private void updateLast(int cell, T item, IntervalHeap<T>.Handle handle)
    {
      this.heap[cell].last = item;
      if (handle != null)
        handle.index = 2 * cell + 1;
      this.heap[cell].lasthandle = handle;
    }

    private void updateFirst(int cell, T item, IntervalHeap<T>.Handle handle)
    {
      this.heap[cell].first = item;
      if (handle != null)
        handle.index = 2 * cell;
      this.heap[cell].firsthandle = handle;
    }

    public void AddAll(IEnumerable<T> items)
    {
      ++this.stamp;
      int size = this.size;
      foreach (T obj in items)
        this.add((IntervalHeap<T>.Handle) null, obj);
      if (this.size == size)
        return;
      if ((this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None)
      {
        foreach (T obj in items)
          this.raiseItemsAdded(obj, 1);
      }
      this.raiseCollectionChanged();
    }

    public override bool IsEmpty => this.size == 0;

    public override int Count => this.size;

    public override Speed CountSpeed => Speed.Constant;

    public override T Choose()
    {
      if (this.size == 0)
        throw new NoSuchItemException("Collection is empty");
      return this.heap[0].first;
    }

    public override IEnumerator<T> GetEnumerator()
    {
      int mystamp = this.stamp;
      for (int i = 0; i < this.size; ++i)
      {
        if (mystamp != this.stamp)
          throw new CollectionModifiedException();
        yield return i % 2 == 0 ? this.heap[i >> 1].first : this.heap[i >> 1].last;
      }
    }

    private bool check(int i, T min, T max)
    {
      bool flag = true;
      IntervalHeap<T>.Interval interval = this.heap[i];
      T first = interval.first;
      T last = interval.last;
      if (2 * i + 1 == this.size)
      {
        if (this.comparer.Compare(min, first) > 0)
        {
          Logger.Log(string.Format("Cell {0}: parent.first({1}) > first({2})  [size={3}]", (object) i, (object) min, (object) first, (object) this.size));
          flag = false;
        }
        if (this.comparer.Compare(first, max) > 0)
        {
          Logger.Log(string.Format("Cell {0}: first({1}) > parent.last({2})  [size={3}]", (object) i, (object) first, (object) max, (object) this.size));
          flag = false;
        }
        if (interval.firsthandle != null && interval.firsthandle.index != 2 * i)
        {
          Logger.Log(string.Format("Cell {0}: firsthandle.index({1}) != 2*cell({2})  [size={3}]", (object) i, (object) interval.firsthandle.index, (object) (2 * i), (object) this.size));
          flag = false;
        }
        return flag;
      }
      if (this.comparer.Compare(min, first) > 0)
      {
        Logger.Log(string.Format("Cell {0}: parent.first({1}) > first({2})  [size={3}]", (object) i, (object) min, (object) first, (object) this.size));
        flag = false;
      }
      if (this.comparer.Compare(first, last) > 0)
      {
        Logger.Log(string.Format("Cell {0}: first({1}) > last({2})  [size={3}]", (object) i, (object) first, (object) last, (object) this.size));
        flag = false;
      }
      if (this.comparer.Compare(last, max) > 0)
      {
        Logger.Log(string.Format("Cell {0}: last({1}) > parent.last({2})  [size={3}]", (object) i, (object) last, (object) max, (object) this.size));
        flag = false;
      }
      if (interval.firsthandle != null && interval.firsthandle.index != 2 * i)
      {
        Logger.Log(string.Format("Cell {0}: firsthandle.index({1}) != 2*cell({2})  [size={3}]", (object) i, (object) interval.firsthandle.index, (object) (2 * i), (object) this.size));
        flag = false;
      }
      if (interval.lasthandle != null && interval.lasthandle.index != 2 * i + 1)
      {
        Logger.Log(string.Format("Cell {0}: lasthandle.index({1}) != 2*cell+1({2})  [size={3}]", (object) i, (object) interval.lasthandle.index, (object) (2 * i + 1), (object) this.size));
        flag = false;
      }
      int i1 = 2 * i + 1;
      int i2 = i1 + 1;
      if (2 * i1 < this.size)
        flag = flag && this.check(i1, first, last);
      if (2 * i2 < this.size)
        flag = flag && this.check(i2, first, last);
      return flag;
    }

    public bool Check()
    {
      if (this.size == 0)
        return true;
      return this.size == 1 ? (object) this.heap[0].first != null : this.check(0, this.heap[0].first, this.heap[0].last);
    }

    public T this[IPriorityQueueHandle<T> handle]
    {
      get
      {
        int cell;
        bool isfirst;
        this.checkHandle(handle, out cell, out isfirst);
        return !isfirst ? this.heap[cell].last : this.heap[cell].first;
      }
      set => this.Replace(handle, value);
    }

    public bool Find(IPriorityQueueHandle<T> handle, out T item)
    {
      if (!(handle is IntervalHeap<T>.Handle handle1))
      {
        item = default (T);
        return false;
      }
      int index1 = handle1.index;
      int index2 = index1 / 2;
      bool flag = index1 % 2 == 0;
      if (index1 == -1 || index1 >= this.size)
      {
        item = default (T);
        return false;
      }
      if ((flag ? this.heap[index2].firsthandle : this.heap[index2].lasthandle) != handle1)
      {
        item = default (T);
        return false;
      }
      item = flag ? this.heap[index2].first : this.heap[index2].last;
      return true;
    }

    public bool Add(ref IPriorityQueueHandle<T> handle, T item)
    {
      ++this.stamp;
      IntervalHeap<T>.Handle itemhandle = (IntervalHeap<T>.Handle) handle;
      if (itemhandle == null)
        handle = (IPriorityQueueHandle<T>) (itemhandle = new IntervalHeap<T>.Handle());
      else if (itemhandle.index != -1)
        throw new InvalidPriorityQueueHandleException("Handle not valid for reuse");
      if (!this.add(itemhandle, item))
        return false;
      this.raiseItemsAdded(item, 1);
      this.raiseCollectionChanged();
      return true;
    }

    public T Delete(IPriorityQueueHandle<T> handle)
    {
      ++this.stamp;
      int cell;
      bool isfirst;
      this.checkHandle(handle, out cell, out isfirst).index = -1;
      int index = (this.size - 1) / 2;
      T obj;
      if (cell == index)
      {
        if (isfirst)
        {
          obj = this.heap[cell].first;
          if (this.size % 2 == 0)
          {
            this.updateFirst(cell, this.heap[cell].last, this.heap[cell].lasthandle);
            this.heap[cell].last = default (T);
            this.heap[cell].lasthandle = (IntervalHeap<T>.Handle) null;
          }
          else
          {
            this.heap[cell].first = default (T);
            this.heap[cell].firsthandle = (IntervalHeap<T>.Handle) null;
          }
        }
        else
        {
          obj = this.heap[cell].last;
          this.heap[cell].last = default (T);
          this.heap[cell].lasthandle = (IntervalHeap<T>.Handle) null;
        }
        --this.size;
      }
      else if (isfirst)
      {
        obj = this.heap[cell].first;
        if (this.size % 2 == 0)
        {
          this.updateFirst(cell, this.heap[index].last, this.heap[index].lasthandle);
          this.heap[index].last = default (T);
          this.heap[index].lasthandle = (IntervalHeap<T>.Handle) null;
        }
        else
        {
          this.updateFirst(cell, this.heap[index].first, this.heap[index].firsthandle);
          this.heap[index].first = default (T);
          this.heap[index].firsthandle = (IntervalHeap<T>.Handle) null;
        }
        --this.size;
        if (this.heapifyMin(cell))
          this.bubbleUpMax(cell);
        else
          this.bubbleUpMin(cell);
      }
      else
      {
        obj = this.heap[cell].last;
        if (this.size % 2 == 0)
        {
          this.updateLast(cell, this.heap[index].last, this.heap[index].lasthandle);
          this.heap[index].last = default (T);
          this.heap[index].lasthandle = (IntervalHeap<T>.Handle) null;
        }
        else
        {
          this.updateLast(cell, this.heap[index].first, this.heap[index].firsthandle);
          this.heap[index].first = default (T);
          this.heap[index].firsthandle = (IntervalHeap<T>.Handle) null;
        }
        --this.size;
        if (this.heapifyMax(cell))
          this.bubbleUpMin(cell);
        else
          this.bubbleUpMax(cell);
      }
      this.raiseItemsRemoved(obj, 1);
      this.raiseCollectionChanged();
      return obj;
    }

    private IntervalHeap<T>.Handle checkHandle(
      IPriorityQueueHandle<T> handle,
      out int cell,
      out bool isfirst)
    {
      IntervalHeap<T>.Handle handle1 = (IntervalHeap<T>.Handle) handle;
      int index = handle1.index;
      cell = index / 2;
      isfirst = index % 2 == 0;
      if (index == -1 || index >= this.size)
        throw new InvalidPriorityQueueHandleException("Invalid handle, index out of range");
      if ((isfirst ? this.heap[cell].firsthandle : this.heap[cell].lasthandle) != handle1)
        throw new InvalidPriorityQueueHandleException("Invalid handle, doesn't match queue");
      return handle1;
    }

    public T Replace(IPriorityQueueHandle<T> handle, T item)
    {
      ++this.stamp;
      int cell;
      bool isfirst;
      this.checkHandle(handle, out cell, out isfirst);
      if (this.size == 0)
        throw new NoSuchItemException();
      T obj;
      if (isfirst)
      {
        obj = this.heap[cell].first;
        this.heap[cell].first = item;
        if (this.size != 1)
        {
          if (this.size == 2 * cell + 1)
          {
            int index = (cell + 1) / 2 - 1;
            if (this.comparer.Compare(item, this.heap[index].last) > 0)
            {
              IntervalHeap<T>.Handle firsthandle = this.heap[cell].firsthandle;
              this.updateFirst(cell, this.heap[index].last, this.heap[index].lasthandle);
              this.updateLast(index, item, firsthandle);
              this.bubbleUpMax(index);
            }
            else
              this.bubbleUpMin(cell);
          }
          else if (this.heapifyMin(cell))
            this.bubbleUpMax(cell);
          else
            this.bubbleUpMin(cell);
        }
      }
      else
      {
        obj = this.heap[cell].last;
        this.heap[cell].last = item;
        if (this.heapifyMax(cell))
          this.bubbleUpMin(cell);
        else
          this.bubbleUpMax(cell);
      }
      this.raiseItemsRemoved(obj, 1);
      this.raiseItemsAdded(item, 1);
      this.raiseCollectionChanged();
      return obj;
    }

    public T FindMin(out IPriorityQueueHandle<T> handle)
    {
      if (this.size == 0)
        throw new NoSuchItemException();
      handle = (IPriorityQueueHandle<T>) this.heap[0].firsthandle;
      return this.heap[0].first;
    }

    public T FindMax(out IPriorityQueueHandle<T> handle)
    {
      if (this.size == 0)
        throw new NoSuchItemException();
      if (this.size == 1)
      {
        handle = (IPriorityQueueHandle<T>) this.heap[0].firsthandle;
        return this.heap[0].first;
      }
      handle = (IPriorityQueueHandle<T>) this.heap[0].lasthandle;
      return this.heap[0].last;
    }

    public T DeleteMin(out IPriorityQueueHandle<T> handle)
    {
      ++this.stamp;
      if (this.size == 0)
        throw new NoSuchItemException();
      T first = this.heap[0].first;
      IntervalHeap<T>.Handle firsthandle = this.heap[0].firsthandle;
      handle = (IPriorityQueueHandle<T>) firsthandle;
      if (firsthandle != null)
        firsthandle.index = -1;
      if (this.size == 1)
      {
        this.size = 0;
        this.heap[0].first = default (T);
        this.heap[0].firsthandle = (IntervalHeap<T>.Handle) null;
      }
      else
      {
        int index = (this.size - 1) / 2;
        if (this.size % 2 == 0)
        {
          this.updateFirst(0, this.heap[index].last, this.heap[index].lasthandle);
          this.heap[index].last = default (T);
          this.heap[index].lasthandle = (IntervalHeap<T>.Handle) null;
        }
        else
        {
          this.updateFirst(0, this.heap[index].first, this.heap[index].firsthandle);
          this.heap[index].first = default (T);
          this.heap[index].firsthandle = (IntervalHeap<T>.Handle) null;
        }
        --this.size;
        this.heapifyMin(0);
      }
      this.raiseItemsRemoved(first, 1);
      this.raiseCollectionChanged();
      return first;
    }

    public T DeleteMax(out IPriorityQueueHandle<T> handle)
    {
      ++this.stamp;
      if (this.size == 0)
        throw new NoSuchItemException();
      T obj;
      IntervalHeap<T>.Handle handle1;
      if (this.size == 1)
      {
        this.size = 0;
        obj = this.heap[0].first;
        handle1 = this.heap[0].firsthandle;
        if (handle1 != null)
          handle1.index = -1;
        this.heap[0].first = default (T);
        this.heap[0].firsthandle = (IntervalHeap<T>.Handle) null;
      }
      else
      {
        obj = this.heap[0].last;
        handle1 = this.heap[0].lasthandle;
        if (handle1 != null)
          handle1.index = -1;
        int index = (this.size - 1) / 2;
        if (this.size % 2 == 0)
        {
          this.updateLast(0, this.heap[index].last, this.heap[index].lasthandle);
          this.heap[index].last = default (T);
          this.heap[index].lasthandle = (IntervalHeap<T>.Handle) null;
        }
        else
        {
          this.updateLast(0, this.heap[index].first, this.heap[index].firsthandle);
          this.heap[index].first = default (T);
          this.heap[index].firsthandle = (IntervalHeap<T>.Handle) null;
        }
        --this.size;
        this.heapifyMax(0);
      }
      this.raiseItemsRemoved(obj, 1);
      this.raiseCollectionChanged();
      handle = (IPriorityQueueHandle<T>) handle1;
      return obj;
    }

    private struct Interval
    {
      internal T first;
      internal T last;
      internal IntervalHeap<T>.Handle firsthandle;
      internal IntervalHeap<T>.Handle lasthandle;

      public override string ToString()
      {
        return string.Format("[{0}; {1}]", (object) this.first, (object) this.last);
      }
    }

    [Serializable]
    private class Handle : IPriorityQueueHandle<T>
    {
      internal int index = -1;

      public override string ToString() => string.Format("[{0}]", (object) this.index);
    }
  }
}
