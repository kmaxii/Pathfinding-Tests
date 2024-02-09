// Decompiled with JetBrains decompiler
// Type: C5.LinkedList`1
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
  public class LinkedList<T> : 
    SequencedBase<T>,
    IList<T>,
    IIndexed<T>,
    ISequenced<T>,
    ICollection<T>,
    IExtensible<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IDisposable,
    System.Collections.Generic.IList<T>,
    System.Collections.Generic.ICollection<T>,
    IList,
    ICollection,
    IStack<T>,
    IQueue<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private bool fIFO = true;
    private LinkedList<T>.Node startsentinel;
    private LinkedList<T>.Node endsentinel;
    private int offset;
    private LinkedList<T> underlying;
    private WeakViewList<LinkedList<T>> views;
    private WeakViewList<LinkedList<T>>.Node myWeakReference;
    private bool isValid = true;

    public override EventTypeEnum ListenableEvents
    {
      get => this.underlying != null ? EventTypeEnum.None : EventTypeEnum.All;
    }

    private bool equals(T i1, T i2) => this.itemequalityComparer.Equals(i1, i2);

    protected override void updatecheck()
    {
      this.validitycheck();
      base.updatecheck();
      if (this.underlying == null)
        return;
      ++this.underlying.stamp;
    }

    private void validitycheck()
    {
      if (!this.isValid)
        throw new ViewDisposedException();
    }

    protected override void modifycheck(int stamp)
    {
      this.validitycheck();
      if ((this.underlying != null ? this.underlying.stamp : this.stamp) != stamp)
        throw new CollectionModifiedException();
    }

    private bool contains(T item, out LinkedList<T>.Node node)
    {
      node = this.startsentinel.next;
      while (node != this.endsentinel)
      {
        if (this.equals(item, node.item))
          return true;
        node = node.next;
      }
      return false;
    }

    private bool find(T item, ref LinkedList<T>.Node node, ref int index)
    {
      while (node != this.endsentinel)
      {
        if (this.itemequalityComparer.Equals(item, node.item))
          return true;
        ++index;
        node = node.next;
      }
      return false;
    }

    private bool dnif(T item, ref LinkedList<T>.Node node, ref int index)
    {
      while (node != this.startsentinel)
      {
        if (this.itemequalityComparer.Equals(item, node.item))
          return true;
        --index;
        node = node.prev;
      }
      return false;
    }

    private LinkedList<T>.Node get(int pos)
    {
      if (pos < 0 || pos >= this.size)
        throw new IndexOutOfRangeException();
      if (pos < this.size / 2)
      {
        LinkedList<T>.Node node = this.startsentinel;
        for (int index = 0; index <= pos; ++index)
          node = node.next;
        return node;
      }
      LinkedList<T>.Node node1 = this.endsentinel;
      for (int size = this.size; size > pos; --size)
        node1 = node1.prev;
      return node1;
    }

    private int dist(int pos, out int nearest, int[] positions)
    {
      nearest = -1;
      int num1 = int.MaxValue;
      int num2 = num1;
      for (int index = 0; index < positions.Length; ++index)
      {
        int num3 = positions[index] - pos;
        if (num3 >= 0 && num3 < num1)
        {
          nearest = index;
          num1 = num3;
          num2 = num3;
        }
        if (num3 < 0 && -num3 < num1)
        {
          nearest = index;
          num1 = -num3;
          num2 = num3;
        }
      }
      return num2;
    }

    private LinkedList<T>.Node get(int pos, int[] positions, LinkedList<T>.Node[] nodes)
    {
      int nearest;
      int num = this.dist(pos, out nearest, positions);
      LinkedList<T>.Node node = nodes[nearest];
      if (num > 0)
      {
        for (int index = 0; index < num; ++index)
          node = node.prev;
      }
      else
      {
        for (int index = 0; index > num; --index)
          node = node.next;
      }
      return node;
    }

    private void getPair(
      int p1,
      int p2,
      out LinkedList<T>.Node n1,
      out LinkedList<T>.Node n2,
      int[] positions,
      LinkedList<T>.Node[] nodes)
    {
      int nearest1;
      int num1 = this.dist(p1, out nearest1, positions);
      int num2 = num1 < 0 ? -num1 : num1;
      int nearest2;
      int num3 = this.dist(p2, out nearest2, positions);
      int num4 = num3 < 0 ? -num3 : num3;
      if (num2 < num4)
      {
        n1 = this.get(p1, positions, nodes);
        n2 = this.get(p2, new int[2]
        {
          positions[nearest2],
          p1
        }, new LinkedList<T>.Node[2]{ nodes[nearest2], n1 });
      }
      else
      {
        n2 = this.get(p2, positions, nodes);
        n1 = this.get(p1, new int[2]
        {
          positions[nearest1],
          p2
        }, new LinkedList<T>.Node[2]{ nodes[nearest1], n2 });
      }
    }

    private LinkedList<T>.Node insert(int index, LinkedList<T>.Node succ, T item)
    {
      LinkedList<T>.Node node = new LinkedList<T>.Node(item, succ.prev, succ);
      succ.prev.next = node;
      succ.prev = node;
      ++this.size;
      if (this.underlying != null)
        ++this.underlying.size;
      this.fixViewsAfterInsert(succ, node.prev, 1, this.Offset + index);
      return node;
    }

    private T remove(LinkedList<T>.Node node, int index)
    {
      this.fixViewsBeforeSingleRemove(node, this.Offset + index);
      node.prev.next = node.next;
      node.next.prev = node.prev;
      --this.size;
      if (this.underlying != null)
        --this.underlying.size;
      return node.item;
    }

    private void fixViewsAfterInsert(
      LinkedList<T>.Node succ,
      LinkedList<T>.Node pred,
      int added,
      int realInsertionIndex)
    {
      if (this.views == null)
        return;
      foreach (LinkedList<T> view in this.views)
      {
        if (view != this)
        {
          if (view.Offset == realInsertionIndex && view.size > 0)
            view.startsentinel = succ.prev;
          if (view.Offset + view.size == realInsertionIndex)
            view.endsentinel = pred.next;
          if (view.Offset < realInsertionIndex && view.Offset + view.size > realInsertionIndex)
            view.size += added;
          if (view.Offset > realInsertionIndex || view.Offset == realInsertionIndex && view.size > 0)
            view.offset += added;
        }
      }
    }

    private void fixViewsBeforeSingleRemove(LinkedList<T>.Node node, int realRemovalIndex)
    {
      if (this.views == null)
        return;
      foreach (LinkedList<T> view in this.views)
      {
        if (view != this)
        {
          if (view.offset - 1 == realRemovalIndex)
            view.startsentinel = node.prev;
          if (view.offset + view.size == realRemovalIndex)
            view.endsentinel = node.next;
          if (view.offset <= realRemovalIndex && view.offset + view.size > realRemovalIndex)
            --view.size;
          if (view.offset > realRemovalIndex)
            --view.offset;
        }
      }
    }

    private void fixViewsBeforeRemove(
      int start,
      int count,
      LinkedList<T>.Node first,
      LinkedList<T>.Node last)
    {
      int num1 = start + count - 1;
      if (this.views == null)
        return;
      foreach (LinkedList<T> view in this.views)
      {
        if (view != this)
        {
          int offset = view.Offset;
          int num2 = offset + view.size - 1;
          if (start < offset && offset - 1 <= num1)
            view.startsentinel = first.prev;
          if (start <= num2 + 1 && num2 < num1)
            view.endsentinel = last.next;
          if (start < offset)
          {
            if (num1 < offset)
            {
              view.offset = offset - count;
            }
            else
            {
              view.offset = start;
              view.size = num1 < num2 ? num2 - num1 : 0;
            }
          }
          else if (start <= num2)
            view.size = num1 <= num2 ? view.size - count : start - offset;
        }
      }
    }

    private MutualViewPosition viewPosition(LinkedList<T> otherView)
    {
      int num1 = this.offset + this.size;
      int offset = otherView.offset;
      int size = otherView.size;
      int num2 = offset + size;
      if (offset >= num1 || num2 <= this.offset)
        return MutualViewPosition.NonOverlapping;
      if (this.size == 0 || offset <= this.offset && num1 <= num2)
        return MutualViewPosition.Contains;
      return size == 0 || this.offset <= offset && num2 <= num1 ? MutualViewPosition.ContainedIn : MutualViewPosition.Overlapping;
    }

    private void disposeOverlappingViews(bool reverse)
    {
      if (this.views == null)
        return;
      foreach (LinkedList<T> view in this.views)
      {
        if (view != this)
        {
          switch (this.viewPosition(view))
          {
            case MutualViewPosition.ContainedIn:
              if (!reverse)
              {
                view.Dispose();
                continue;
              }
              continue;
            case MutualViewPosition.Overlapping:
              view.Dispose();
              continue;
            default:
              continue;
          }
        }
      }
    }

    public LinkedList(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : base(itemequalityComparer, memoryType)
    {
      if (memoryType != MemoryType.Normal)
        throw new Exception("LinkedList doesn't support MemoryType Strict or Safe");
      this.offset = 0;
      this.size = this.stamp = 0;
      this.startsentinel = new LinkedList<T>.Node(default (T));
      this.endsentinel = new LinkedList<T>.Node(default (T));
      this.startsentinel.next = this.endsentinel;
      this.endsentinel.prev = this.startsentinel;
    }

    public LinkedList()
      : this(C5.EqualityComparer<T>.Default)
    {
    }

    public virtual void Dispose() => this.Dispose(false);

    private void Dispose(bool disposingUnderlying)
    {
      if (!this.isValid)
        return;
      if (this.underlying != null)
      {
        this.isValid = false;
        if (!disposingUnderlying && this.views != null)
          this.views.Remove(this.myWeakReference);
        this.endsentinel = (LinkedList<T>.Node) null;
        this.startsentinel = (LinkedList<T>.Node) null;
        this.underlying = (LinkedList<T>) null;
        this.views = (WeakViewList<LinkedList<T>>) null;
        this.myWeakReference = (WeakViewList<LinkedList<T>>.Node) null;
      }
      else
      {
        if (this.views != null)
        {
          foreach (LinkedList<T> view in this.views)
            view.Dispose(true);
        }
        this.Clear();
      }
    }

    public virtual T First
    {
      get
      {
        this.validitycheck();
        if (this.size == 0)
          throw new NoSuchItemException();
        return this.startsentinel.next.item;
      }
    }

    public virtual T Last
    {
      get
      {
        this.validitycheck();
        if (this.size == 0)
          throw new NoSuchItemException();
        return this.endsentinel.prev.item;
      }
    }

    public virtual bool FIFO
    {
      get
      {
        this.validitycheck();
        return this.fIFO;
      }
      set
      {
        this.updatecheck();
        this.fIFO = value;
      }
    }

    public virtual bool IsFixedSize
    {
      get
      {
        this.validitycheck();
        return false;
      }
    }

    public virtual T this[int index]
    {
      get
      {
        this.validitycheck();
        return this.get(index).item;
      }
      set
      {
        this.updatecheck();
        LinkedList<T>.Node node = this.get(index);
        T obj = node.item;
        node.item = value;
        (this.underlying ?? this).raiseForSetThis(index, value, obj);
      }
    }

    public virtual Speed IndexingSpeed => Speed.Linear;

    public virtual void Insert(int i, T item)
    {
      this.updatecheck();
      this.insert(i, i == this.size ? this.endsentinel : this.get(i), item);
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      (this.underlying ?? this).raiseForInsert(i + this.Offset, item);
    }

    public void Insert(IList<T> pointer, T item)
    {
      this.updatecheck();
      if (pointer == null || (pointer.Underlying ?? pointer) != (this.underlying ?? this))
        throw new IncompatibleViewException();
      this.Insert(pointer.Offset + pointer.Count - this.Offset, item);
    }

    public virtual void InsertAll(int i, IEnumerable<T> items) => this.InsertAll(i, items, true);

    private void InsertAll(int i, IEnumerable<T> items, bool insertion)
    {
      this.updatecheck();
      int added = 0;
      LinkedList<T>.Node succ = i == this.size ? this.endsentinel : this.get(i);
      LinkedList<T>.Node prev;
      LinkedList<T>.Node node1 = prev = succ.prev;
      foreach (T obj in items)
      {
        LinkedList<T>.Node node2 = new LinkedList<T>.Node(obj, prev, (LinkedList<T>.Node) null);
        prev.next = node2;
        ++added;
        prev = node2;
      }
      if (added == 0)
        return;
      succ.prev = prev;
      prev.next = succ;
      this.size += added;
      if (this.underlying != null)
        this.underlying.size += added;
      if (added <= 0)
        return;
      this.fixViewsAfterInsert(succ, node1, added, this.offset + i);
      this.raiseForInsertAll(node1, i, added, insertion);
    }

    private void raiseForInsertAll(LinkedList<T>.Node node, int i, int added, bool insertion)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      int num = this.Offset + i;
      if ((this.ActiveEvents & (EventTypeEnum.Added | EventTypeEnum.Inserted)) != EventTypeEnum.None)
      {
        for (int index = num; index < num + added; ++index)
        {
          node = node.next;
          T obj = node.item;
          if (insertion)
            this.raiseItemInserted(obj, index);
          this.raiseItemsAdded(obj, 1);
        }
      }
      this.raiseCollectionChanged();
    }

    public virtual void InsertFirst(T item)
    {
      this.updatecheck();
      this.insert(0, this.startsentinel.next, item);
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      (this.underlying ?? this).raiseForInsert(this.Offset, item);
    }

    public virtual void InsertLast(T item)
    {
      this.updatecheck();
      this.insert(this.size, this.endsentinel, item);
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      (this.underlying ?? this).raiseForInsert(this.size - 1 + this.Offset, item);
    }

    public IList<V> Map<V>(Func<T, V> mapper)
    {
      this.validitycheck();
      LinkedList<V> retval = new LinkedList<V>();
      return this.map<V>(mapper, retval);
    }

    public IList<V> Map<V>(Func<T, V> mapper, IEqualityComparer<V> equalityComparer)
    {
      this.validitycheck();
      LinkedList<V> retval = new LinkedList<V>(equalityComparer);
      return this.map<V>(mapper, retval);
    }

    private IList<V> map<V>(Func<T, V> mapper, LinkedList<V> retval)
    {
      if (this.size == 0)
        return (IList<V>) retval;
      int stamp = this.stamp;
      LinkedList<T>.Node next = this.startsentinel.next;
      LinkedList<V>.Node prev = retval.startsentinel;
      while (next != this.endsentinel)
      {
        V v = mapper(next.item);
        this.modifycheck(stamp);
        prev.next = new LinkedList<V>.Node(v, prev, (LinkedList<V>.Node) null);
        next = next.next;
        prev = prev.next;
      }
      retval.endsentinel.prev = prev;
      prev.next = retval.endsentinel;
      retval.size = this.size;
      return (IList<V>) retval;
    }

    public virtual T Remove()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException("List is empty");
      T obj = this.fIFO ? this.remove(this.startsentinel.next, 0) : this.remove(this.endsentinel.prev, this.size - 1);
      (this.underlying ?? this).raiseForRemove(obj);
      return obj;
    }

    public virtual T RemoveFirst()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException("List is empty");
      T obj = this.remove(this.startsentinel.next, 0);
      if (this.ActiveEvents != EventTypeEnum.None)
        (this.underlying ?? this).raiseForRemoveAt(this.Offset, obj);
      return obj;
    }

    public virtual T RemoveLast()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException("List is empty");
      T obj = this.remove(this.endsentinel.prev, this.size - 1);
      if (this.ActiveEvents != EventTypeEnum.None)
        (this.underlying ?? this).raiseForRemoveAt(this.size + this.Offset, obj);
      return obj;
    }

    public virtual IList<T> View(int start, int count)
    {
      this.checkRange(start, count);
      this.validitycheck();
      if (this.views == null)
        this.views = new WeakViewList<LinkedList<T>>();
      LinkedList<T> view = (LinkedList<T>) this.MemberwiseClone();
      view.underlying = this.underlying != null ? this.underlying : this;
      view.offset = this.offset + start;
      view.size = count;
      this.getPair(start - 1, start + count, out view.startsentinel, out view.endsentinel, new int[2]
      {
        -1,
        this.size
      }, new LinkedList<T>.Node[2]
      {
        this.startsentinel,
        this.endsentinel
      });
      view.myWeakReference = this.views.Add(view);
      return (IList<T>) view;
    }

    public virtual IList<T> ViewOf(T item)
    {
      int index = 0;
      LinkedList<T>.Node next = this.startsentinel.next;
      return !this.find(item, ref next, ref index) ? (IList<T>) null : this.View(index, 1);
    }

    public virtual IList<T> LastViewOf(T item)
    {
      int index = this.size - 1;
      LinkedList<T>.Node prev = this.endsentinel.prev;
      return !this.dnif(item, ref prev, ref index) ? (IList<T>) null : this.View(index, 1);
    }

    public virtual IList<T> Underlying
    {
      get
      {
        this.validitycheck();
        return (IList<T>) this.underlying;
      }
    }

    public virtual bool IsValid => this.isValid;

    public virtual int Offset
    {
      get
      {
        this.validitycheck();
        return this.offset;
      }
    }

    public IList<T> Slide(int offset)
    {
      if (!this.TrySlide(offset, this.size))
        throw new ArgumentOutOfRangeException();
      return (IList<T>) this;
    }

    public IList<T> Slide(int offset, int size)
    {
      if (!this.TrySlide(offset, size))
        throw new ArgumentOutOfRangeException();
      return (IList<T>) this;
    }

    public virtual bool TrySlide(int offset) => this.TrySlide(offset, this.size);

    public virtual bool TrySlide(int offset, int size)
    {
      this.updatecheck();
      if (this.underlying == null)
        throw new NotAViewException("List not a view");
      if (offset + this.offset < 0 || offset + this.offset + size > this.underlying.size)
        return false;
      int offset1 = this.offset;
      this.getPair(offset - 1, offset + size, out this.startsentinel, out this.endsentinel, new int[4]
      {
        -offset1 - 1,
        -1,
        this.size,
        this.underlying.size - offset1
      }, new LinkedList<T>.Node[4]
      {
        this.underlying.startsentinel,
        this.startsentinel,
        this.endsentinel,
        this.underlying.endsentinel
      });
      this.size = size;
      this.offset += offset;
      return true;
    }

    public virtual IList<T> Span(IList<T> otherView)
    {
      if (otherView == null || (otherView.Underlying ?? otherView) != (this.underlying ?? this))
        throw new IncompatibleViewException();
      return otherView.Offset + otherView.Count - this.Offset < 0 ? (IList<T>) null : (this.underlying ?? this).View(this.Offset, otherView.Offset + otherView.Count - this.Offset);
    }

    public virtual void Reverse()
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      LinkedList<T>.Position[] positionArray = (LinkedList<T>.Position[]) null;
      int poslow = 0;
      int poshigh = 0;
      if (this.views != null)
      {
        CircularQueue<LinkedList<T>.Position> circularQueue = (CircularQueue<LinkedList<T>.Position>) null;
        foreach (LinkedList<T> view in this.views)
        {
          if (view != this)
          {
            switch (this.viewPosition(view))
            {
              case MutualViewPosition.ContainedIn:
                (circularQueue ?? (circularQueue = new CircularQueue<LinkedList<T>.Position>())).Enqueue(new LinkedList<T>.Position(view, true));
                circularQueue.Enqueue(new LinkedList<T>.Position(view, false));
                continue;
              case MutualViewPosition.Overlapping:
                view.Dispose();
                continue;
              default:
                continue;
            }
          }
        }
        if (circularQueue != null)
        {
          positionArray = circularQueue.ToArray();
          Sorting.IntroSort<LinkedList<T>.Position>(positionArray, 0, positionArray.Length, (IComparer<LinkedList<T>.Position>) LinkedList<T>.PositionComparer.Default);
          poshigh = positionArray.Length - 1;
        }
      }
      LinkedList<T>.Node next = this.get(0);
      LinkedList<T>.Node prev = this.get(this.size - 1);
      for (int i = 0; i < this.size / 2; ++i)
      {
        T obj = next.item;
        next.item = prev.item;
        prev.item = obj;
        if (positionArray != null)
          this.mirrorViewSentinelsForReverse(positionArray, ref poslow, ref poshigh, next, prev, i);
        next = next.next;
        prev = prev.prev;
      }
      if (positionArray != null && this.size % 2 != 0)
        this.mirrorViewSentinelsForReverse(positionArray, ref poslow, ref poshigh, next, prev, this.size / 2);
      (this.underlying ?? this).raiseCollectionChanged();
    }

    private void mirrorViewSentinelsForReverse(
      LinkedList<T>.Position[] positions,
      ref int poslow,
      ref int poshigh,
      LinkedList<T>.Node a,
      LinkedList<T>.Node b,
      int i)
    {
      int num1 = this.offset + i;
      int num2 = this.offset + this.size - 1 - i;
      LinkedList<T>.Position position;
      while (poslow <= poshigh && (position = positions[poslow]).Index == num1)
      {
        if (position.Left)
        {
          position.View.endsentinel = b.next;
        }
        else
        {
          position.View.startsentinel = b.prev;
          position.View.offset = num2;
        }
        ++poslow;
      }
      while (poslow < poshigh && (position = positions[poshigh]).Index == num2)
      {
        if (position.Left)
        {
          position.View.endsentinel = a.next;
        }
        else
        {
          position.View.startsentinel = a.prev;
          position.View.offset = num1;
        }
        --poshigh;
      }
    }

    public bool IsSorted() => this.IsSorted((IComparer<T>) Comparer<T>.Default);

    public virtual bool IsSorted(IComparer<T> c)
    {
      this.validitycheck();
      if (this.size <= 1)
        return true;
      LinkedList<T>.Node next1 = this.startsentinel.next;
      T x = next1.item;
      for (LinkedList<T>.Node next2 = next1.next; next2 != this.endsentinel; next2 = next2.next)
      {
        if (c.Compare(x, next2.item) > 0)
          return false;
        x = next2.item;
      }
      return true;
    }

    public virtual void Sort() => this.Sort((IComparer<T>) Comparer<T>.Default);

    public virtual void Sort(IComparer<T> c)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      this.disposeOverlappingViews(false);
      LinkedList<T>.Node node1 = this.startsentinel.next;
      LinkedList<T>.Node node2 = this.startsentinel.next;
      this.endsentinel.prev.next = (LinkedList<T>.Node) null;
      LinkedList<T>.Node next;
      for (; node2 != null; node2 = next)
      {
        for (next = node2.next; next != null && c.Compare(node2.item, next.item) <= 0; next = node2.next)
          node2 = next;
        node2.next = (LinkedList<T>.Node) null;
        node1.prev = next;
        node1 = next;
        if (c.Compare(this.endsentinel.prev.item, node2.item) <= 0)
          this.endsentinel.prev = node2;
      }
      while (this.startsentinel.next.prev != null)
      {
        LinkedList<T>.Node run1 = this.startsentinel.next;
        LinkedList<T>.Node node3 = (LinkedList<T>.Node) null;
        LinkedList<T>.Node prev;
        for (; run1 != null && run1.prev != null; run1 = prev)
        {
          prev = run1.prev.prev;
          LinkedList<T>.Node node4 = LinkedList<T>.mergeRuns(run1, run1.prev, c);
          if (node3 != null)
            node3.prev = node4;
          else
            this.startsentinel.next = node4;
          node3 = node4;
        }
        if (run1 != null)
          node3.prev = run1;
      }
      this.endsentinel.prev.next = this.endsentinel;
      this.startsentinel.next.prev = this.startsentinel;
      (this.underlying ?? this).raiseCollectionChanged();
    }

    private static LinkedList<T>.Node mergeRuns(
      LinkedList<T>.Node run1,
      LinkedList<T>.Node run2,
      IComparer<T> c)
    {
      LinkedList<T>.Node node1;
      bool flag;
      if (c.Compare(run1.item, run2.item) <= 0)
      {
        node1 = run1;
        flag = true;
        run1 = run1.next;
      }
      else
      {
        node1 = run2;
        flag = false;
        run2 = run2.next;
      }
      LinkedList<T>.Node node2 = node1;
      node2.prev = (LinkedList<T>.Node) null;
      while (run1 != null && run2 != null)
      {
        if (flag)
        {
          for (; run1 != null && c.Compare(run2.item, run1.item) >= 0; run1 = node1.next)
            node1 = run1;
          if (run1 != null)
          {
            node1.next = run2;
            run2.prev = node1;
            node1 = run2;
            run2 = node1.next;
            flag = false;
          }
        }
        else
        {
          for (; run2 != null && c.Compare(run1.item, run2.item) > 0; run2 = node1.next)
            node1 = run2;
          if (run2 != null)
          {
            node1.next = run1;
            run1.prev = node1;
            node1 = run1;
            run1 = node1.next;
            flag = true;
          }
        }
      }
      if (run1 != null)
      {
        node1.next = run1;
        run1.prev = node1;
      }
      else if (run2 != null)
      {
        node1.next = run2;
        run2.prev = node1;
      }
      return node2;
    }

    public virtual void Shuffle() => this.Shuffle((Random) new C5Random());

    public virtual void Shuffle(Random rnd)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      this.disposeOverlappingViews(false);
      ArrayList<T> arrayList = new ArrayList<T>();
      arrayList.AddAll((IEnumerable<T>) this);
      arrayList.Shuffle(rnd);
      LinkedList<T>.Node next = this.startsentinel.next;
      int num = 0;
      for (; next != this.endsentinel; next = next.next)
        next.item = arrayList[num++];
      (this.underlying ?? this).raiseCollectionChanged();
    }

    public IDirectedCollectionValue<T> this[int start, int count]
    {
      get
      {
        this.validitycheck();
        this.checkRange(start, count);
        return (IDirectedCollectionValue<T>) new LinkedList<T>.Range(this, start, count, true);
      }
    }

    public virtual int IndexOf(T item)
    {
      this.validitycheck();
      LinkedList<T>.Node next = this.startsentinel.next;
      int index = 0;
      return this.find(item, ref next, ref index) ? index : ~this.size;
    }

    public virtual int LastIndexOf(T item)
    {
      this.validitycheck();
      LinkedList<T>.Node prev = this.endsentinel.prev;
      int index = this.size - 1;
      return this.dnif(item, ref prev, ref index) ? index : ~this.size;
    }

    public virtual T RemoveAt(int i)
    {
      this.updatecheck();
      T obj = this.remove(this.get(i), i);
      if (this.ActiveEvents != EventTypeEnum.None)
        (this.underlying ?? this).raiseForRemoveAt(this.Offset + i, obj);
      return obj;
    }

    public virtual void RemoveInterval(int start, int count)
    {
      this.updatecheck();
      this.checkRange(start, count);
      if (count == 0)
        return;
      LinkedList<T>.Node first = this.get(start);
      LinkedList<T>.Node last = this.get(start + count - 1);
      this.fixViewsBeforeRemove(start, count, first, last);
      first.prev.next = last.next;
      last.next.prev = first.prev;
      if (this.underlying != null)
        this.underlying.size -= count;
      this.size -= count;
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      (this.underlying ?? this).raiseForRemoveInterval(start + this.Offset, count);
    }

    private void raiseForRemoveInterval(int start, int count)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseCollectionCleared(this.size == 0, count, new int?(start));
      this.raiseCollectionChanged();
    }

    public override int GetSequencedHashCode()
    {
      this.validitycheck();
      return base.GetSequencedHashCode();
    }

    public override bool SequencedEquals(ISequenced<T> that)
    {
      this.validitycheck();
      return base.SequencedEquals(that);
    }

    public override IDirectedCollectionValue<T> Backwards() => this[0, this.size].Backwards();

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public virtual Speed ContainsSpeed => Speed.Linear;

    public override int GetUnsequencedHashCode()
    {
      this.validitycheck();
      return base.GetUnsequencedHashCode();
    }

    public override bool UnsequencedEquals(ICollection<T> that)
    {
      this.validitycheck();
      return base.UnsequencedEquals(that);
    }

    public virtual bool Contains(T item)
    {
      this.validitycheck();
      return this.contains(item, out LinkedList<T>.Node _);
    }

    public virtual bool Find(ref T item)
    {
      this.validitycheck();
      LinkedList<T>.Node node;
      if (!this.contains(item, out node))
        return false;
      item = node.item;
      return true;
    }

    public virtual bool Update(T item) => this.Update(item, out T _);

    public virtual bool Update(T item, out T olditem)
    {
      this.updatecheck();
      LinkedList<T>.Node node;
      if (this.contains(item, out node))
      {
        olditem = node.item;
        node.item = item;
        (this.underlying ?? this).raiseForUpdate(item, olditem);
        return true;
      }
      olditem = default (T);
      return false;
    }

    public virtual bool FindOrAdd(ref T item)
    {
      this.updatecheck();
      if (this.Find(ref item))
        return true;
      this.Add(item);
      return false;
    }

    public virtual bool UpdateOrAdd(T item) => this.UpdateOrAdd(item, out T _);

    public virtual bool UpdateOrAdd(T item, out T olditem)
    {
      this.updatecheck();
      if (this.Update(item, out olditem))
        return true;
      this.Add(item);
      olditem = default (T);
      return false;
    }

    public virtual bool Remove(T item)
    {
      this.updatecheck();
      int index = 0;
      LinkedList<T>.Node node = this.fIFO ? this.startsentinel.next : this.endsentinel.prev;
      if ((this.fIFO ? (this.find(item, ref node, ref index) ? 1 : 0) : (this.dnif(item, ref node, ref index) ? 1 : 0)) == 0)
        return false;
      T obj = this.remove(node, index);
      (this.underlying ?? this).raiseForRemove(obj);
      return true;
    }

    public virtual bool Remove(T item, out T removeditem)
    {
      this.updatecheck();
      int index = 0;
      LinkedList<T>.Node node = this.fIFO ? this.startsentinel.next : this.endsentinel.prev;
      if ((this.fIFO ? (this.find(item, ref node, ref index) ? 1 : 0) : (this.dnif(item, ref node, ref index) ? 1 : 0)) == 0)
      {
        removeditem = default (T);
        return false;
      }
      removeditem = node.item;
      this.remove(node, index);
      (this.underlying ?? this).raiseForRemove(removeditem);
      return true;
    }

    public virtual void RemoveAll(IEnumerable<T> items)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll(items);
      LinkedList<T>.ViewHandler viewHandler = new LinkedList<T>.ViewHandler(this);
      int num1 = 0;
      int removed = 0;
      int offset = this.Offset;
      LinkedList<T>.Node next = this.startsentinel.next;
      while (next != this.endsentinel)
      {
        while (next != this.endsentinel && !hashBag.Contains(next.item))
        {
          next = next.next;
          ++num1;
        }
        viewHandler.skipEndpoints(removed, offset + num1);
        LinkedList<T>.Node prev = next.prev;
        while (next != this.endsentinel && hashBag.Remove(next.item))
        {
          if (mustFire)
            removeAllHandler.Remove(next.item);
          ++removed;
          next = next.next;
          ++num1;
          viewHandler.updateViewSizesAndCounts(removed, offset + num1);
        }
        viewHandler.updateSentinels(offset + num1, prev, next);
        prev.next = next;
        next.prev = prev;
      }
      int num2 = this.underlying != null ? this.underlying.size + 1 - offset : this.size + 1 - offset;
      viewHandler.updateViewSizesAndCounts(removed, offset + num2);
      this.size -= removed;
      if (this.underlying != null)
        this.underlying.size -= removed;
      removeAllHandler.Raise();
    }

    private void RemoveAll(Func<T, bool> predicate)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      LinkedList<T>.ViewHandler viewHandler = new LinkedList<T>.ViewHandler(this);
      int num1 = 0;
      int removed = 0;
      int offset = this.Offset;
      LinkedList<T>.Node next = this.startsentinel.next;
      while (next != this.endsentinel)
      {
        while (next != this.endsentinel && !predicate(next.item))
        {
          this.updatecheck();
          next = next.next;
          ++num1;
        }
        this.updatecheck();
        viewHandler.skipEndpoints(removed, offset + num1);
        LinkedList<T>.Node prev = next.prev;
        while (next != this.endsentinel && predicate(next.item))
        {
          this.updatecheck();
          if (mustFire)
            removeAllHandler.Remove(next.item);
          ++removed;
          next = next.next;
          ++num1;
          viewHandler.updateViewSizesAndCounts(removed, offset + num1);
        }
        this.updatecheck();
        viewHandler.updateSentinels(offset + num1, prev, next);
        prev.next = next;
        next.prev = prev;
      }
      int num2 = this.underlying != null ? this.underlying.size + 1 - offset : this.size + 1 - offset;
      viewHandler.updateViewSizesAndCounts(removed, offset + num2);
      this.size -= removed;
      if (this.underlying != null)
        this.underlying.size -= removed;
      removeAllHandler.Raise();
    }

    public virtual void Clear()
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      int size = this.size;
      this.clear();
      (this.underlying ?? this).raiseForRemoveInterval(this.Offset, size);
    }

    private void clear()
    {
      if (this.size == 0)
        return;
      this.fixViewsBeforeRemove(this.Offset, this.size, this.startsentinel.next, this.endsentinel.prev);
      this.endsentinel.prev = this.startsentinel;
      this.startsentinel.next = this.endsentinel;
      if (this.underlying != null)
        this.underlying.size -= this.size;
      this.size = 0;
    }

    public virtual void RetainAll(IEnumerable<T> items)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll(items);
      LinkedList<T>.ViewHandler viewHandler = new LinkedList<T>.ViewHandler(this);
      int num1 = 0;
      int removed = 0;
      int offset = this.Offset;
      LinkedList<T>.Node next = this.startsentinel.next;
      while (next != this.endsentinel)
      {
        while (next != this.endsentinel && hashBag.Remove(next.item))
        {
          next = next.next;
          ++num1;
        }
        viewHandler.skipEndpoints(removed, offset + num1);
        LinkedList<T>.Node prev = next.prev;
        while (next != this.endsentinel && !hashBag.Contains(next.item))
        {
          if (mustFire)
            removeAllHandler.Remove(next.item);
          ++removed;
          next = next.next;
          ++num1;
          viewHandler.updateViewSizesAndCounts(removed, offset + num1);
        }
        viewHandler.updateSentinels(offset + num1, prev, next);
        prev.next = next;
        next.prev = prev;
      }
      int num2 = this.underlying != null ? this.underlying.size + 1 - offset : this.size + 1 - offset;
      viewHandler.updateViewSizesAndCounts(removed, offset + num2);
      this.size -= removed;
      if (this.underlying != null)
        this.underlying.size -= removed;
      removeAllHandler.Raise();
    }

    private void RetainAll(Func<T, bool> predicate)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      LinkedList<T>.ViewHandler viewHandler = new LinkedList<T>.ViewHandler(this);
      int num1 = 0;
      int removed = 0;
      int offset = this.Offset;
      LinkedList<T>.Node next = this.startsentinel.next;
      while (next != this.endsentinel)
      {
        while (next != this.endsentinel && predicate(next.item))
        {
          this.updatecheck();
          next = next.next;
          ++num1;
        }
        this.updatecheck();
        viewHandler.skipEndpoints(removed, offset + num1);
        LinkedList<T>.Node prev = next.prev;
        while (next != this.endsentinel && !predicate(next.item))
        {
          this.updatecheck();
          if (mustFire)
            removeAllHandler.Remove(next.item);
          ++removed;
          next = next.next;
          ++num1;
          viewHandler.updateViewSizesAndCounts(removed, offset + num1);
        }
        this.updatecheck();
        viewHandler.updateSentinels(offset + num1, prev, next);
        prev.next = next;
        next.prev = prev;
      }
      int num2 = this.underlying != null ? this.underlying.size + 1 - offset : this.size + 1 - offset;
      viewHandler.updateViewSizesAndCounts(removed, offset + num2);
      this.size -= removed;
      if (this.underlying != null)
        this.underlying.size -= removed;
      removeAllHandler.Raise();
    }

    public virtual bool ContainsAll(IEnumerable<T> items)
    {
      this.validitycheck();
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll(items);
      if (hashBag.Count > this.size)
        return false;
      for (LinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel; next = next.next)
        hashBag.Remove(next.item);
      return hashBag.IsEmpty;
    }

    public IList<T> FindAll(Func<T, bool> filter)
    {
      this.validitycheck();
      int stamp = this.stamp;
      LinkedList<T> all = new LinkedList<T>();
      LinkedList<T>.Node next = this.startsentinel.next;
      LinkedList<T>.Node prev = all.startsentinel;
      for (; next != this.endsentinel; next = next.next)
      {
        bool flag = filter(next.item);
        this.modifycheck(stamp);
        if (flag)
        {
          prev.next = new LinkedList<T>.Node(next.item, prev, (LinkedList<T>.Node) null);
          prev = prev.next;
          ++all.size;
        }
      }
      all.endsentinel.prev = prev;
      prev.next = all.endsentinel;
      return (IList<T>) all;
    }

    public virtual int ContainsCount(T item)
    {
      this.validitycheck();
      int num = 0;
      for (LinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel; next = next.next)
      {
        if (this.itemequalityComparer.Equals(next.item, item))
          ++num;
      }
      return num;
    }

    public virtual ICollectionValue<T> UniqueItems()
    {
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll((IEnumerable<T>) this);
      return hashBag.UniqueItems();
    }

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll((IEnumerable<T>) this);
      return hashBag.ItemMultiplicities();
    }

    public virtual void RemoveAllCopies(T item)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      LinkedList<T>.ViewHandler viewHandler = new LinkedList<T>.ViewHandler(this);
      int num1 = 0;
      int removed = 0;
      int offset = this.Offset;
      LinkedList<T>.Node next = this.startsentinel.next;
      while (next != this.endsentinel)
      {
        while (next != this.endsentinel && !this.itemequalityComparer.Equals(next.item, item))
        {
          next = next.next;
          ++num1;
        }
        viewHandler.skipEndpoints(removed, offset + num1);
        LinkedList<T>.Node prev = next.prev;
        while (next != this.endsentinel && this.itemequalityComparer.Equals(next.item, item))
        {
          if (mustFire)
            removeAllHandler.Remove(next.item);
          ++removed;
          next = next.next;
          ++num1;
          viewHandler.updateViewSizesAndCounts(removed, offset + num1);
        }
        viewHandler.updateSentinels(offset + num1, prev, next);
        prev.next = next;
        next.prev = prev;
      }
      int num2 = this.underlying != null ? this.underlying.size + 1 - offset : this.size + 1 - offset;
      viewHandler.updateViewSizesAndCounts(removed, offset + num2);
      this.size -= removed;
      if (this.underlying != null)
        this.underlying.size -= removed;
      removeAllHandler.Raise();
    }

    public override int Count
    {
      get
      {
        this.validitycheck();
        return this.size;
      }
    }

    public override T Choose() => this.First;

    public override IEnumerable<T> Filter(Func<T, bool> filter)
    {
      this.validitycheck();
      return base.Filter(filter);
    }

    public override IEnumerator<T> GetEnumerator()
    {
      this.validitycheck();
      LinkedList<T>.Node cursor = this.startsentinel.next;
      int enumeratorstamp = this.underlying != null ? this.underlying.stamp : this.stamp;
      for (; cursor != this.endsentinel; cursor = cursor.next)
      {
        this.modifycheck(enumeratorstamp);
        yield return cursor.item;
      }
    }

    public virtual bool Add(T item)
    {
      this.updatecheck();
      this.insert(this.size, this.endsentinel, item);
      (this.underlying ?? this).raiseForAdd(item);
      return true;
    }

    public virtual bool AllowsDuplicates => true;

    public virtual bool DuplicatesByCounting => false;

    public virtual void AddAll(IEnumerable<T> items) => this.InsertAll(this.size, items, false);

    public void Push(T item) => this.InsertLast(item);

    public T Pop() => this.RemoveLast();

    public virtual void Enqueue(T item) => this.InsertLast(item);

    public virtual T Dequeue() => this.RemoveFirst();

    private bool checkViews()
    {
      if (this.underlying != null)
        throw new InternalException("checkViews() called on a view");
      if (this.views == null)
        return true;
      bool flag = true;
      LinkedList<T>.Node[] nodeArray = new LinkedList<T>.Node[this.size + 2];
      int num = 0;
      for (LinkedList<T>.Node node = this.startsentinel; node != null; node = node.next)
        nodeArray[num++] = node;
      foreach (LinkedList<T> view in this.views)
      {
        if (!view.isValid)
        {
          Logger.Log(string.Format("Invalid view(hash {0}, offset {1}, size {2})", (object) view.GetHashCode(), (object) view.offset, (object) view.size));
          flag = false;
        }
        else
        {
          if (view.Offset > this.size || view.Offset < 0)
          {
            Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), Offset > underlying.size ({2})", (object) view.GetHashCode(), (object) view.offset, (object) view.size, (object) this.size));
            flag = false;
          }
          else if (view.startsentinel != nodeArray[view.Offset])
          {
            Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), startsentinel {3} should be {4}", (object) view.GetHashCode(), (object) view.offset, (object) view.size, (object) (view.startsentinel.ToString() + " " + (object) view.startsentinel.GetHashCode()), (object) (nodeArray[view.Offset].ToString() + " " + (object) nodeArray[view.Offset].GetHashCode())));
            flag = false;
          }
          if (view.Offset + view.size > this.size || view.Offset + view.size < 0)
          {
            Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), end index > underlying.size ({3})", (object) view.GetHashCode(), (object) view.offset, (object) view.size, (object) this.size));
            flag = false;
          }
          else if (view.endsentinel != nodeArray[view.Offset + view.size + 1])
          {
            Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), endsentinel {3} should be {4}", (object) view.GetHashCode(), (object) view.offset, (object) view.size, (object) (view.endsentinel.ToString() + " " + (object) view.endsentinel.GetHashCode()), (object) (nodeArray[view.Offset + view.size + 1].ToString() + " " + (object) nodeArray[view.Offset + view.size + 1].GetHashCode())));
            flag = false;
          }
          if (view.views != this.views)
          {
            Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), wrong views list {3} <> {4}", (object) view.GetHashCode(), (object) view.offset, (object) view.size, (object) view.views.GetHashCode(), (object) this.views.GetHashCode()));
            flag = false;
          }
          if (view.underlying != this)
          {
            Logger.Log(string.Format("Bad view(hash {0}, offset {1}, size {2}), wrong underlying {3} <> this {4}", (object) view.GetHashCode(), (object) view.offset, (object) view.size, (object) view.underlying.GetHashCode(), (object) this.GetHashCode()));
            flag = false;
          }
          int stamp1 = view.stamp;
          int stamp2 = this.stamp;
        }
      }
      return flag;
    }

    private string zeitem(LinkedList<T>.Node node)
    {
      return node != null ? node.item.ToString() : "(null node)";
    }

    public virtual bool Check()
    {
      bool flag = true;
      if (this.underlying != null)
        return this.underlying.Check();
      if (this.startsentinel == null)
      {
        Logger.Log("startsentinel == null");
        flag = false;
      }
      if (this.endsentinel == null)
      {
        Logger.Log("endsentinel == null");
        flag = false;
      }
      if (this.size == 0)
      {
        if (this.startsentinel != null && this.startsentinel.next != this.endsentinel)
        {
          Logger.Log("size == 0 but startsentinel.next != endsentinel");
          flag = false;
        }
        if (this.endsentinel != null && this.endsentinel.prev != this.startsentinel)
        {
          Logger.Log("size == 0 but endsentinel.prev != startsentinel");
          flag = false;
        }
      }
      if (this.startsentinel == null)
      {
        Logger.Log("NULL startsentinel");
        return flag;
      }
      int num = 0;
      LinkedList<T>.Node next = this.startsentinel.next;
      LinkedList<T>.Node node = this.startsentinel;
      while (next != this.endsentinel)
      {
        ++num;
        if (next.prev != node)
        {
          Logger.Log(string.Format("Bad backpointer at node {0}", (object) num));
          flag = false;
        }
        node = next;
        next = next.next;
        if (next == null)
        {
          Logger.Log(string.Format("Null next pointer at node {0}", (object) num));
          return false;
        }
      }
      if (num != this.size)
      {
        Logger.Log(string.Format("size={0} but enumeration gives {1} nodes ", (object) this.size, (object) num));
        flag = false;
      }
      return this.checkViews() && flag;
    }

    void System.Collections.Generic.IList<T>.RemoveAt(int index) => this.RemoveAt(index);

    void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

    bool ICollection.IsSynchronized => false;

    [Obsolete]
    object ICollection.SyncRoot
    {
      get
      {
        return this.underlying == null ? (object) this.startsentinel : ((ICollection) this.underlying).SyncRoot;
      }
    }

    void ICollection.CopyTo(Array arr, int index)
    {
      if (index < 0 || index + this.Count > arr.Length)
        throw new ArgumentOutOfRangeException();
      foreach (T obj in (EnumerableBase<T>) this)
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

    [Serializable]
    private class Node
    {
      public LinkedList<T>.Node prev;
      public LinkedList<T>.Node next;
      public T item;

      internal Node(T item) => this.item = item;

      internal Node(T item, LinkedList<T>.Node prev, LinkedList<T>.Node next)
      {
        this.item = item;
        this.prev = prev;
        this.next = next;
      }

      public override string ToString() => string.Format("Node(item={0})", (object) this.item);
    }

    [Serializable]
    private class PositionComparer : IComparer<LinkedList<T>.Position>
    {
      private static LinkedList<T>.PositionComparer _default;

      private PositionComparer()
      {
      }

      public static LinkedList<T>.PositionComparer Default
      {
        get
        {
          return LinkedList<T>.PositionComparer._default ?? (LinkedList<T>.PositionComparer._default = new LinkedList<T>.PositionComparer());
        }
      }

      public int Compare(LinkedList<T>.Position a, LinkedList<T>.Position b)
      {
        return a.Index.CompareTo(b.Index);
      }
    }

    private struct Position
    {
      public readonly LinkedList<T> View;
      public bool Left;
      public readonly int Index;

      public Position(LinkedList<T> view, bool left)
      {
        this.View = view;
        this.Left = left;
        this.Index = left ? view.Offset : view.Offset + view.size - 1;
      }

      public Position(int index)
      {
        this.Index = index;
        this.View = (LinkedList<T>) null;
        this.Left = false;
      }
    }

    private struct ViewHandler
    {
      private ArrayList<LinkedList<T>.Position> leftEnds;
      private ArrayList<LinkedList<T>.Position> rightEnds;
      private int leftEndIndex;
      private int rightEndIndex;
      private int leftEndIndex2;
      private int rightEndIndex2;
      internal readonly int viewCount;

      internal ViewHandler(LinkedList<T> list)
      {
        this.leftEndIndex = this.rightEndIndex = this.leftEndIndex2 = this.rightEndIndex2 = this.viewCount = 0;
        this.leftEnds = this.rightEnds = (ArrayList<LinkedList<T>.Position>) null;
        if (list.views != null)
        {
          foreach (LinkedList<T> view in list.views)
          {
            if (view != list)
            {
              if (this.leftEnds == null)
              {
                this.leftEnds = new ArrayList<LinkedList<T>.Position>();
                this.rightEnds = new ArrayList<LinkedList<T>.Position>();
              }
              this.leftEnds.Add(new LinkedList<T>.Position(view, true));
              this.rightEnds.Add(new LinkedList<T>.Position(view, false));
            }
          }
        }
        if (this.leftEnds == null)
          return;
        this.viewCount = this.leftEnds.Count;
        this.leftEnds.Sort((IComparer<LinkedList<T>.Position>) LinkedList<T>.PositionComparer.Default);
        this.rightEnds.Sort((IComparer<LinkedList<T>.Position>) LinkedList<T>.PositionComparer.Default);
      }

      internal void skipEndpoints(int removed, int realindex)
      {
        if (this.viewCount > 0)
        {
          LinkedList<T>.Position leftEnd;
          for (; this.leftEndIndex < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex]).Index <= realindex; ++this.leftEndIndex)
          {
            LinkedList<T> view = leftEnd.View;
            view.offset -= removed;
            view.size += removed;
          }
          LinkedList<T>.Position rightEnd;
          for (; this.rightEndIndex < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex]).Index < realindex; ++this.rightEndIndex)
            rightEnd.View.size -= removed;
        }
        if (this.viewCount <= 0)
          return;
        while (this.leftEndIndex2 < this.viewCount && this.leftEnds[this.leftEndIndex2].Index <= realindex)
          ++this.leftEndIndex2;
        while (this.rightEndIndex2 < this.viewCount && this.rightEnds[this.rightEndIndex2].Index < realindex - 1)
          ++this.rightEndIndex2;
      }

      internal void updateViewSizesAndCounts(int removed, int realindex)
      {
        if (this.viewCount <= 0)
          return;
        LinkedList<T>.Position leftEnd;
        for (; this.leftEndIndex < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex]).Index <= realindex; ++this.leftEndIndex)
        {
          LinkedList<T> view = leftEnd.View;
          view.offset = view.Offset - removed;
          view.size += removed;
        }
        LinkedList<T>.Position rightEnd;
        for (; this.rightEndIndex < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex]).Index < realindex; ++this.rightEndIndex)
          rightEnd.View.size -= removed;
      }

      internal void updateSentinels(
        int realindex,
        LinkedList<T>.Node newstart,
        LinkedList<T>.Node newend)
      {
        if (this.viewCount <= 0)
          return;
        LinkedList<T>.Position leftEnd;
        for (; this.leftEndIndex2 < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex2]).Index <= realindex; ++this.leftEndIndex2)
          leftEnd.View.startsentinel = newstart;
        LinkedList<T>.Position rightEnd;
        for (; this.rightEndIndex2 < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex2]).Index < realindex - 1; ++this.rightEndIndex2)
          rightEnd.View.endsentinel = newend;
      }
    }

    [Serializable]
    private class Range : 
      DirectedCollectionValueBase<T>,
      IDirectedCollectionValue<T>,
      ICollectionValue<T>,
      IShowable,
      IFormattable,
      IDirectedEnumerable<T>,
      IEnumerable<T>,
      IEnumerable
    {
      private int start;
      private int count;
      private int rangestamp;
      private LinkedList<T>.Node startnode;
      private LinkedList<T>.Node endnode;
      private LinkedList<T> list;
      private bool forwards;

      internal Range(LinkedList<T> list, int start, int count, bool forwards)
      {
        this.list = list;
        this.rangestamp = list.underlying != null ? list.underlying.stamp : list.stamp;
        this.start = start;
        this.count = count;
        this.forwards = forwards;
        if (count <= 0)
          return;
        this.startnode = list.get(start);
        this.endnode = list.get(start + count - 1);
      }

      public override bool IsEmpty
      {
        get
        {
          this.list.modifycheck(this.rangestamp);
          return this.count == 0;
        }
      }

      public override int Count
      {
        get
        {
          this.list.modifycheck(this.rangestamp);
          return this.count;
        }
      }

      public override Speed CountSpeed
      {
        get
        {
          this.list.modifycheck(this.rangestamp);
          return Speed.Constant;
        }
      }

      public override T Choose()
      {
        this.list.modifycheck(this.rangestamp);
        if (this.count > 0)
          return this.startnode.item;
        throw new NoSuchItemException();
      }

      public override IEnumerator<T> GetEnumerator()
      {
        int togo = this.count;
        this.list.modifycheck(this.rangestamp);
        if (togo != 0)
        {
          LinkedList<T>.Node cursor = this.forwards ? this.startnode : this.endnode;
          yield return cursor.item;
          while (--togo > 0)
          {
            cursor = this.forwards ? cursor.next : cursor.prev;
            this.list.modifycheck(this.rangestamp);
            yield return cursor.item;
          }
        }
      }

      public override IDirectedCollectionValue<T> Backwards()
      {
        this.list.modifycheck(this.rangestamp);
        LinkedList<T>.Range range = (LinkedList<T>.Range) this.MemberwiseClone();
        range.forwards = !this.forwards;
        return (IDirectedCollectionValue<T>) range;
      }

      IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
      {
        return (IDirectedEnumerable<T>) this.Backwards();
      }

      public override EnumerationDirection Direction
      {
        get => !this.forwards ? EnumerationDirection.Backwards : EnumerationDirection.Forwards;
      }
    }
  }
}
