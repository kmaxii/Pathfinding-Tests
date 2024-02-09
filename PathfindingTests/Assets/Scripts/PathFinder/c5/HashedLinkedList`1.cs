// Decompiled with JetBrains decompiler
// Type: C5.HashedLinkedList`1
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
  public class HashedLinkedList<T> : 
    SequencedBase<T>,
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
    private const int wordsize = 32;
    private const int lobits = 3;
    private const int hibits = 4;
    private const int losize = 8;
    private const int hisize = 16;
    private const int logwordsize = 5;
    private bool fIFO = true;
    private HashedLinkedList<T>.Node startsentinel;
    private HashedLinkedList<T>.Node endsentinel;
    private int? offset;
    private HashedLinkedList<T> underlying;
    private WeakViewList<HashedLinkedList<T>> views;
    private WeakViewList<HashedLinkedList<T>>.Node myWeakReference;
    private bool isValid = true;
    private HashDictionary<T, HashedLinkedList<T>.Node> dict;
    private int taggroups;

    public override EventTypeEnum ListenableEvents
    {
      get => this.underlying != null ? EventTypeEnum.None : EventTypeEnum.All;
    }

    private int Taggroups
    {
      get => this.underlying != null ? this.underlying.taggroups : this.taggroups;
      set
      {
        if (this.underlying == null)
          this.taggroups = value;
        else
          this.underlying.taggroups = value;
      }
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

    private bool contains(T item, out HashedLinkedList<T>.Node node)
    {
      return this.dict.Find(ref item, out node) && this.insideview(node);
    }

    private bool find(T item, ref HashedLinkedList<T>.Node node, ref int index)
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

    private bool dnif(T item, ref HashedLinkedList<T>.Node node, ref int index)
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

    private bool insideview(HashedLinkedList<T>.Node node)
    {
      if (this.underlying == null)
        return true;
      return this.startsentinel.precedes(node) && node.precedes(this.endsentinel);
    }

    private HashedLinkedList<T>.Node get(int pos)
    {
      if (pos < 0 || pos >= this.size)
        throw new IndexOutOfRangeException();
      if (pos < this.size / 2)
      {
        HashedLinkedList<T>.Node node = this.startsentinel;
        for (int index = 0; index <= pos; ++index)
          node = node.next;
        return node;
      }
      HashedLinkedList<T>.Node node1 = this.endsentinel;
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

    private HashedLinkedList<T>.Node get(
      int pos,
      int[] positions,
      HashedLinkedList<T>.Node[] nodes)
    {
      int nearest;
      int num = this.dist(pos, out nearest, positions);
      HashedLinkedList<T>.Node node = nodes[nearest];
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
      out HashedLinkedList<T>.Node n1,
      out HashedLinkedList<T>.Node n2,
      int[] positions,
      HashedLinkedList<T>.Node[] nodes)
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
        }, new HashedLinkedList<T>.Node[2]
        {
          nodes[nearest2],
          n1
        });
      }
      else
      {
        n2 = this.get(p2, positions, nodes);
        n1 = this.get(p1, new int[2]
        {
          positions[nearest1],
          p2
        }, new HashedLinkedList<T>.Node[2]
        {
          nodes[nearest1],
          n2
        });
      }
    }

    private void insert(int index, HashedLinkedList<T>.Node succ, T item)
    {
      HashedLinkedList<T>.Node newnode = new HashedLinkedList<T>.Node(item);
      if (this.dict.FindOrAdd(item, ref newnode))
        throw new DuplicateNotAllowedException("Item already in indexed list");
      this.insertNode(true, succ, newnode);
    }

    private void insertNode(
      bool updateViews,
      HashedLinkedList<T>.Node succ,
      HashedLinkedList<T>.Node newnode)
    {
      newnode.next = succ;
      HashedLinkedList<T>.Node pred = newnode.prev = succ.prev;
      succ.prev.next = newnode;
      succ.prev = newnode;
      ++this.size;
      if (this.underlying != null)
        ++this.underlying.size;
      this.settag(newnode);
      if (!updateViews)
        return;
      this.fixViewsAfterInsert(succ, pred, 1, 0);
    }

    private T remove(HashedLinkedList<T>.Node node, int index)
    {
      this.fixViewsBeforeSingleRemove(node, this.Offset + index);
      node.prev.next = node.next;
      node.next.prev = node.prev;
      --this.size;
      if (this.underlying != null)
        --this.underlying.size;
      this.removefromtaggroup(node);
      return node.item;
    }

    private bool dictremove(T item, out HashedLinkedList<T>.Node node)
    {
      if (this.underlying == null)
      {
        if (!this.dict.Remove(item, out node))
          return false;
      }
      else
      {
        if (!this.contains(item, out node))
          return false;
        this.dict.Remove(item);
      }
      return true;
    }

    private void fixViewsAfterInsert(
      HashedLinkedList<T>.Node succ,
      HashedLinkedList<T>.Node pred,
      int added,
      int realInsertionIndex)
    {
      if (this.views == null)
        return;
      foreach (HashedLinkedList<T> view in this.views)
      {
        if (view != this)
        {
          if (pred.precedes(view.startsentinel) || view.startsentinel == pred && view.size > 0)
          {
            HashedLinkedList<T> hashedLinkedList = view;
            int? offset = hashedLinkedList.offset;
            int num = added;
            hashedLinkedList.offset = offset.HasValue ? new int?(offset.GetValueOrDefault() + num) : new int?();
          }
          if (view.startsentinel.precedes(pred) && succ.precedes(view.endsentinel))
            view.size += added;
          if (view.startsentinel == pred && view.size > 0)
            view.startsentinel = succ.prev;
          if (view.endsentinel == succ)
            view.endsentinel = pred.next;
        }
      }
    }

    private void fixViewsBeforeSingleRemove(HashedLinkedList<T>.Node node, int realRemovalIndex)
    {
      if (this.views == null)
        return;
      foreach (HashedLinkedList<T> view in this.views)
      {
        if (view != this)
        {
          if (view.startsentinel.precedes(node) && node.precedes(view.endsentinel))
            --view.size;
          if (!view.startsentinel.precedes(node))
          {
            HashedLinkedList<T> hashedLinkedList = view;
            int? offset = hashedLinkedList.offset;
            hashedLinkedList.offset = offset.HasValue ? new int?(offset.GetValueOrDefault() - 1) : new int?();
          }
          if (view.startsentinel == node)
            view.startsentinel = node.prev;
          if (view.endsentinel == node)
            view.endsentinel = node.next;
        }
      }
    }

    private MutualViewPosition viewPosition(HashedLinkedList<T> otherView)
    {
      HashedLinkedList<T>.Node startsentinel = otherView.startsentinel;
      HashedLinkedList<T>.Node endsentinel = otherView.endsentinel;
      HashedLinkedList<T>.Node next1 = this.startsentinel.next;
      HashedLinkedList<T>.Node prev1 = this.endsentinel.prev;
      HashedLinkedList<T>.Node next2 = startsentinel.next;
      HashedLinkedList<T>.Node prev2 = endsentinel.prev;
      if (prev1.precedes(next2) || prev2.precedes(next1))
        return MutualViewPosition.NonOverlapping;
      if (this.size == 0 || startsentinel.precedes(next1) && prev1.precedes(endsentinel))
        return MutualViewPosition.Contains;
      return otherView.size == 0 || this.startsentinel.precedes(next2) && prev2.precedes(this.endsentinel) ? MutualViewPosition.ContainedIn : MutualViewPosition.Overlapping;
    }

    private void disposeOverlappingViews(bool reverse)
    {
      if (this.views == null)
        return;
      foreach (HashedLinkedList<T> view in this.views)
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

    public HashedLinkedList(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : base(itemequalityComparer, memoryType)
    {
      if (memoryType != MemoryType.Normal)
        throw new Exception("HashedLinkedList doesn't support MemoryType Strict or Safe");
      this.offset = new int?(0);
      this.size = this.stamp = 0;
      this.startsentinel = new HashedLinkedList<T>.Node(default (T));
      this.endsentinel = new HashedLinkedList<T>.Node(default (T));
      this.startsentinel.next = this.endsentinel;
      this.endsentinel.prev = this.startsentinel;
      this.startsentinel.taggroup = new HashedLinkedList<T>.TagGroup();
      this.startsentinel.taggroup.tag = int.MinValue;
      this.startsentinel.taggroup.count = 0;
      this.endsentinel.taggroup = new HashedLinkedList<T>.TagGroup();
      this.endsentinel.taggroup.tag = int.MaxValue;
      this.endsentinel.taggroup.count = 0;
      this.dict = new HashDictionary<T, HashedLinkedList<T>.Node>(itemequalityComparer);
    }

    public HashedLinkedList()
      : this(C5.EqualityComparer<T>.Default)
    {
    }

    public HashedLinkedList(MemoryType memoryType = MemoryType.Normal)
      : this(C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    private HashedLinkedList<T>.TagGroup gettaggroup(
      HashedLinkedList<T>.Node pred,
      HashedLinkedList<T>.Node succ,
      out int lowbound,
      out int highbound)
    {
      HashedLinkedList<T>.TagGroup taggroup1 = pred.taggroup;
      HashedLinkedList<T>.TagGroup taggroup2 = succ.taggroup;
      if (taggroup1 == taggroup2)
      {
        lowbound = pred.tag + 1;
        highbound = succ.tag - 1;
        return taggroup1;
      }
      if (taggroup1.first != null)
      {
        lowbound = pred.tag + 1;
        highbound = int.MaxValue;
        return taggroup1;
      }
      if (taggroup2.first != null)
      {
        lowbound = int.MinValue;
        highbound = succ.tag - 1;
        return taggroup2;
      }
      lowbound = int.MinValue;
      highbound = int.MaxValue;
      return new HashedLinkedList<T>.TagGroup();
    }

    private void settag(HashedLinkedList<T>.Node node)
    {
      HashedLinkedList<T>.Node prev = node.prev;
      HashedLinkedList<T>.Node next = node.next;
      HashedLinkedList<T>.TagGroup taggroup1 = prev.taggroup;
      HashedLinkedList<T>.TagGroup taggroup2 = next.taggroup;
      if (taggroup1 == taggroup2)
      {
        node.taggroup = taggroup1;
        ++taggroup1.count;
        if (prev.tag + 1 == next.tag)
          this.splittaggroup(taggroup1);
        else
          node.tag = (prev.tag + 1) / 2 + (next.tag - 1) / 2;
      }
      else if (taggroup1.first != null)
      {
        node.taggroup = taggroup1;
        taggroup1.last = node;
        ++taggroup1.count;
        if (prev.tag == int.MaxValue)
          this.splittaggroup(taggroup1);
        else
          node.tag = prev.tag / 2 + 1073741823 + 1;
      }
      else if (taggroup2.first != null)
      {
        node.taggroup = taggroup2;
        taggroup2.first = node;
        ++taggroup2.count;
        if (next.tag == int.MinValue)
          this.splittaggroup(node.taggroup);
        else
          node.tag = (next.tag - 1) / 2 - 1073741824;
      }
      else
      {
        HashedLinkedList<T>.TagGroup tagGroup = new HashedLinkedList<T>.TagGroup();
        this.Taggroups = 1;
        node.taggroup = tagGroup;
        tagGroup.first = tagGroup.last = node;
        tagGroup.count = 1;
      }
    }

    private void removefromtaggroup(HashedLinkedList<T>.Node node)
    {
      HashedLinkedList<T>.TagGroup taggroup1 = node.taggroup;
      if (--taggroup1.count == 0)
      {
        --this.Taggroups;
      }
      else
      {
        if (node == taggroup1.first)
          taggroup1.first = node.next;
        if (node == taggroup1.last)
          taggroup1.last = node.prev;
        if (taggroup1.count != 8 || this.Taggroups == 1)
          return;
        HashedLinkedList<T>.Node prev;
        HashedLinkedList<T>.TagGroup taggroup2;
        if ((prev = taggroup1.first.prev) != this.startsentinel && (taggroup2 = prev.taggroup).count <= 8)
        {
          taggroup1.first = taggroup2.first;
        }
        else
        {
          HashedLinkedList<T>.Node next;
          if ((next = taggroup1.last.next) == this.endsentinel || (taggroup2 = next.taggroup).count > 8)
            return;
          taggroup1.last = taggroup2.last;
        }
        HashedLinkedList<T>.Node node1 = taggroup2.first;
        int num1 = 0;
        for (int count = taggroup2.count; num1 < count; ++num1)
        {
          node1.taggroup = taggroup1;
          node1 = node1.next;
        }
        taggroup1.count += taggroup2.count;
        --this.Taggroups;
        HashedLinkedList<T>.Node node2 = taggroup1.first;
        int num2 = 0;
        for (int count = taggroup1.count; num2 < count; ++num2)
        {
          node2.tag = num2 - 8 << 28;
          node2 = node2.next;
        }
      }
    }

    private void splittaggroup(HashedLinkedList<T>.TagGroup taggroup)
    {
      HashedLinkedList<T>.Node node = taggroup.first;
      int tag1 = taggroup.first.prev.taggroup.tag;
      int tag2 = taggroup.last.next.taggroup.tag;
      int num1 = 28;
      int num2 = (taggroup.count - 1) / 16;
      int num3 = (int) (((double) tag2 + 0.0 - (double) tag1) / (double) (num2 + 2));
      int num4 = tag1;
      int num5 = num3 == 0 ? 1 : num3;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        HashedLinkedList<T>.TagGroup tagGroup = new HashedLinkedList<T>.TagGroup();
        tagGroup.tag = num4 = num4 >= tag2 - num5 ? tag2 : num4 + num5;
        tagGroup.first = node;
        tagGroup.count = 16;
        for (int index2 = 0; index2 < 16; ++index2)
        {
          node.taggroup = tagGroup;
          node.tag = index2 - 8 << num1;
          node = node.next;
        }
        tagGroup.last = node.prev;
      }
      int num6 = taggroup.count - 16 * num2;
      taggroup.first = node;
      taggroup.count = num6;
      int num7;
      taggroup.tag = num7 = num4 >= tag2 - num5 ? tag2 : num4 + num5;
      int num8 = num1 - 1;
      for (int index = 0; index < num6; ++index)
      {
        node.tag = index - 16 << num8;
        node = node.next;
      }
      taggroup.last = node.prev;
      this.Taggroups += num2;
      if (num7 != tag2)
        return;
      this.redistributetaggroups(taggroup);
    }

    private void redistributetaggroups(HashedLinkedList<T>.TagGroup taggroup)
    {
      HashedLinkedList<T>.TagGroup tagGroup1 = taggroup;
      HashedLinkedList<T>.TagGroup tagGroup2 = taggroup;
      double num1 = 1.0;
      double num2 = Math.Pow((double) this.Taggroups, 1.0 / 30.0);
      int num3 = 1;
      int num4 = 1;
      do
      {
        ++num3;
        int num5 = ~((1 << num3) - 1);
        int num6;
        HashedLinkedList<T>.TagGroup taggroup1;
        for (num6 = taggroup.tag & num5; (taggroup1 = tagGroup1.first.prev.taggroup).first != null && (taggroup1.tag & num5) == num6; tagGroup1 = taggroup1)
          ++num4;
        HashedLinkedList<T>.TagGroup taggroup2;
        for (; (taggroup2 = tagGroup2.last.next.taggroup).last != null && (taggroup2.tag & num5) == num6; tagGroup2 = taggroup2)
          ++num4;
        num1 *= num2;
      }
      while ((double) num4 > num1);
      int tag = tagGroup1.first.prev.taggroup.tag;
      int num7 = tagGroup2.last.next.taggroup.tag / (num4 + 1) - tag / (num4 + 1);
      for (int index = 0; index < num4; ++index)
      {
        tagGroup1.tag = tag + (index + 1) * num7;
        tagGroup1 = tagGroup1.last.next.taggroup;
      }
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
        this.endsentinel = (HashedLinkedList<T>.Node) null;
        this.startsentinel = (HashedLinkedList<T>.Node) null;
        this.underlying = (HashedLinkedList<T>) null;
        this.views = (WeakViewList<HashedLinkedList<T>>) null;
        this.myWeakReference = (WeakViewList<HashedLinkedList<T>>.Node) null;
      }
      else
      {
        if (this.views != null)
        {
          foreach (HashedLinkedList<T> view in this.views)
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
        HashedLinkedList<T>.Node node = this.get(index);
        T obj = node.item;
        if (this.itemequalityComparer.Equals(value, obj))
        {
          node.item = value;
          this.dict.Update(value, node);
        }
        else
        {
          if (this.dict.FindOrAdd(value, ref node))
            throw new ArgumentException("Item already in indexed list");
          this.dict.Remove(obj);
          node.item = value;
        }
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
      HashedLinkedList<T>.Node succ = i == this.size ? this.endsentinel : this.get(i);
      HashedLinkedList<T>.Node node1;
      HashedLinkedList<T>.Node node2 = node1 = succ.prev;
      int highbound = 0;
      int lowbound = 0;
      HashedLinkedList<T>.TagGroup taggroup = this.gettaggroup(node1, succ, out lowbound, out highbound);
      try
      {
        foreach (T key in items)
        {
          HashedLinkedList<T>.Node node3 = new HashedLinkedList<T>.Node(key, node1, (HashedLinkedList<T>.Node) null);
          if (this.dict.FindOrAdd(key, ref node3))
            throw new DuplicateNotAllowedException("Item already in indexed list");
          HashedLinkedList<T>.Node node4 = node3;
          int num;
          if (lowbound >= highbound)
            num = lowbound;
          else
            lowbound = num = lowbound + 1;
          node4.tag = num;
          node3.taggroup = taggroup;
          node1.next = node3;
          ++added;
          node1 = node3;
        }
      }
      finally
      {
        if (added != 0)
        {
          taggroup.count += added;
          if (taggroup != node2.taggroup)
            taggroup.first = node2.next;
          if (taggroup != succ.taggroup)
            taggroup.last = node1;
          succ.prev = node1;
          node1.next = succ;
          if (node1.tag == node1.prev.tag)
            this.splittaggroup(taggroup);
          this.size += added;
          if (this.underlying != null)
            this.underlying.size += added;
          this.fixViewsAfterInsert(succ, node2, added, 0);
          this.raiseForInsertAll(node2, i, added, insertion);
        }
      }
    }

    private void raiseForInsertAll(
      HashedLinkedList<T>.Node node,
      int i,
      int added,
      bool insertion)
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
      HashedLinkedList<V> retval = new HashedLinkedList<V>();
      return this.map<V>(mapper, retval);
    }

    public IList<V> Map<V>(Func<T, V> mapper, IEqualityComparer<V> equalityComparer)
    {
      this.validitycheck();
      HashedLinkedList<V> retval = new HashedLinkedList<V>(equalityComparer);
      return this.map<V>(mapper, retval);
    }

    private IList<V> map<V>(Func<T, V> mapper, HashedLinkedList<V> retval)
    {
      if (this.size == 0)
        return (IList<V>) retval;
      int stamp = this.stamp;
      HashedLinkedList<T>.Node next = this.startsentinel.next;
      HashedLinkedList<V>.Node prev = retval.startsentinel;
      double num1 = (double) int.MaxValue / ((double) this.size + 1.0);
      int num2 = 1;
      HashedLinkedList<V>.TagGroup tagGroup = new HashedLinkedList<V>.TagGroup();
      retval.taggroups = 1;
      tagGroup.count = this.size;
      while (next != this.endsentinel)
      {
        V key = mapper(next.item);
        this.modifycheck(stamp);
        prev.next = new HashedLinkedList<V>.Node(key, prev, (HashedLinkedList<V>.Node) null);
        next = next.next;
        prev = prev.next;
        retval.dict.Add(key, prev);
        prev.taggroup = tagGroup;
        prev.tag = (int) (num1 * (double) num2++);
      }
      tagGroup.first = retval.startsentinel.next;
      tagGroup.last = prev;
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
      T key = this.fIFO ? this.remove(this.startsentinel.next, 0) : this.remove(this.endsentinel.prev, this.size - 1);
      this.dict.Remove(key);
      (this.underlying ?? this).raiseForRemove(key);
      return key;
    }

    public virtual T RemoveFirst()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException("List is empty");
      T key = this.remove(this.startsentinel.next, 0);
      this.dict.Remove(key);
      if (this.ActiveEvents != EventTypeEnum.None)
        (this.underlying ?? this).raiseForRemoveAt(this.Offset, key);
      return key;
    }

    public virtual T RemoveLast()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException("List is empty");
      T key = this.remove(this.endsentinel.prev, this.size - 1);
      this.dict.Remove(key);
      if (this.ActiveEvents != EventTypeEnum.None)
        (this.underlying ?? this).raiseForRemoveAt(this.size + this.Offset, key);
      return key;
    }

    public virtual IList<T> View(int start, int count)
    {
      this.checkRange(start, count);
      this.validitycheck();
      if (this.views == null)
        this.views = new WeakViewList<HashedLinkedList<T>>();
      HashedLinkedList<T> view = (HashedLinkedList<T>) this.MemberwiseClone();
      view.underlying = this.underlying != null ? this.underlying : this;
      HashedLinkedList<T> hashedLinkedList = view;
      int? offset = this.offset;
      int num = start;
      int? nullable = offset.HasValue ? new int?(offset.GetValueOrDefault() + num) : new int?();
      hashedLinkedList.offset = nullable;
      view.size = count;
      this.getPair(start - 1, start + count, out view.startsentinel, out view.endsentinel, new int[2]
      {
        -1,
        this.size
      }, new HashedLinkedList<T>.Node[2]
      {
        this.startsentinel,
        this.endsentinel
      });
      view.myWeakReference = this.views.Add(view);
      return (IList<T>) view;
    }

    public virtual IList<T> ViewOf(T item)
    {
      this.validitycheck();
      HashedLinkedList<T>.Node node;
      if (!this.contains(item, out node))
        return (IList<T>) null;
      HashedLinkedList<T> hashedLinkedList = (HashedLinkedList<T>) this.MemberwiseClone();
      hashedLinkedList.underlying = this.underlying != null ? this.underlying : this;
      hashedLinkedList.offset = new int?();
      hashedLinkedList.startsentinel = node.prev;
      hashedLinkedList.endsentinel = node.next;
      hashedLinkedList.size = 1;
      return (IList<T>) hashedLinkedList;
    }

    public virtual IList<T> LastViewOf(T item) => this.ViewOf(item);

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
        if (!this.offset.HasValue && this.underlying != null)
        {
          HashedLinkedList<T>.Node node = this.underlying.startsentinel;
          int num = 0;
          while (node != this.startsentinel)
          {
            node = node.next;
            ++num;
          }
          this.offset = new int?(num);
        }
        return this.offset.Value;
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
      if (!this.offset.HasValue)
      {
        try
        {
          this.getPair(offset - 1, offset + size, out this.startsentinel, out this.endsentinel, new int[2]
          {
            -1,
            this.size
          }, new HashedLinkedList<T>.Node[2]
          {
            this.startsentinel,
            this.endsentinel
          });
        }
        catch (NullReferenceException ex)
        {
          return false;
        }
      }
      else
      {
        int num1 = offset;
        int? offset1 = this.offset;
        int? nullable1 = offset1.HasValue ? new int?(num1 + offset1.GetValueOrDefault()) : new int?();
        if ((nullable1.GetValueOrDefault() >= 0 ? 0 : (nullable1.HasValue ? 1 : 0)) == 0)
        {
          int num2 = offset;
          int? offset2 = this.offset;
          int? nullable2 = offset2.HasValue ? new int?(num2 + offset2.GetValueOrDefault()) : new int?();
          int num3 = size;
          int? nullable3 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() + num3) : new int?();
          int size1 = this.underlying.size;
          if ((nullable3.GetValueOrDefault() <= size1 ? 0 : (nullable3.HasValue ? 1 : 0)) == 0)
          {
            int num4 = this.offset.Value;
            this.getPair(offset - 1, offset + size, out this.startsentinel, out this.endsentinel, new int[4]
            {
              -num4 - 1,
              -1,
              this.size,
              this.underlying.size - num4
            }, new HashedLinkedList<T>.Node[4]
            {
              this.underlying.startsentinel,
              this.startsentinel,
              this.endsentinel,
              this.underlying.endsentinel
            });
            goto label_9;
          }
        }
        return false;
      }
label_9:
      this.size = size;
      HashedLinkedList<T> hashedLinkedList = this;
      int? offset3 = hashedLinkedList.offset;
      int num = offset;
      hashedLinkedList.offset = offset3.HasValue ? new int?(offset3.GetValueOrDefault() + num) : new int?();
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
      HashedLinkedList<T>.Position[] positionArray = (HashedLinkedList<T>.Position[]) null;
      int poslow = 0;
      int poshigh = 0;
      if (this.views != null)
      {
        CircularQueue<HashedLinkedList<T>.Position> circularQueue = (CircularQueue<HashedLinkedList<T>.Position>) null;
        foreach (HashedLinkedList<T> view in this.views)
        {
          if (view != this)
          {
            switch (this.viewPosition(view))
            {
              case MutualViewPosition.ContainedIn:
                (circularQueue ?? (circularQueue = new CircularQueue<HashedLinkedList<T>.Position>())).Enqueue(new HashedLinkedList<T>.Position(view, true));
                circularQueue.Enqueue(new HashedLinkedList<T>.Position(view, false));
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
          Sorting.IntroSort<HashedLinkedList<T>.Position>(positionArray, 0, positionArray.Length, (IComparer<HashedLinkedList<T>.Position>) HashedLinkedList<T>.PositionComparer.Default);
          poshigh = positionArray.Length - 1;
        }
      }
      HashedLinkedList<T>.Node next = this.get(0);
      HashedLinkedList<T>.Node prev = this.get(this.size - 1);
      for (int i = 0; i < this.size / 2; ++i)
      {
        T obj = next.item;
        next.item = prev.item;
        prev.item = obj;
        this.dict[next.item] = next;
        this.dict[prev.item] = prev;
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
      HashedLinkedList<T>.Position[] positions,
      ref int poslow,
      ref int poshigh,
      HashedLinkedList<T>.Node a,
      HashedLinkedList<T>.Node b,
      int i)
    {
      int? offset1 = this.offset;
      int num1 = i;
      int? nullable1 = offset1.HasValue ? new int?(offset1.GetValueOrDefault() + num1) : new int?();
      int? offset2 = this.offset;
      int size = this.size;
      int? nullable2 = offset2.HasValue ? new int?(offset2.GetValueOrDefault() + size) : new int?();
      int? nullable3 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() - 1) : new int?();
      int num2 = i;
      int? nullable4 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() - num2) : new int?();
      HashedLinkedList<T>.Position position1;
      while (poslow <= poshigh && (position1 = positions[poslow]).Endpoint == a)
      {
        if (position1.Left)
        {
          position1.View.endsentinel = b.next;
        }
        else
        {
          position1.View.startsentinel = b.prev;
          position1.View.offset = nullable4;
        }
        ++poslow;
      }
      HashedLinkedList<T>.Position position2;
      while (poslow < poshigh && (position2 = positions[poshigh]).Endpoint == b)
      {
        if (position2.Left)
        {
          position2.View.endsentinel = a.next;
        }
        else
        {
          position2.View.startsentinel = a.prev;
          position2.View.offset = nullable1;
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
      HashedLinkedList<T>.Node next1 = this.startsentinel.next;
      T x = next1.item;
      for (HashedLinkedList<T>.Node next2 = next1.next; next2 != this.endsentinel; next2 = next2.next)
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
      if (this.underlying != null)
      {
        for (HashedLinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel; next = next.next)
          --next.taggroup.count;
      }
      HashedLinkedList<T>.Node node1 = this.startsentinel.next;
      HashedLinkedList<T>.Node node2 = this.startsentinel.next;
      this.endsentinel.prev.next = (HashedLinkedList<T>.Node) null;
      HashedLinkedList<T>.Node next1;
      for (; node2 != null; node2 = next1)
      {
        for (next1 = node2.next; next1 != null && c.Compare(node2.item, next1.item) <= 0; next1 = node2.next)
          node2 = next1;
        node2.next = (HashedLinkedList<T>.Node) null;
        node1.prev = next1;
        node1 = next1;
        if (c.Compare(this.endsentinel.prev.item, node2.item) <= 0)
          this.endsentinel.prev = node2;
      }
      while (this.startsentinel.next.prev != null)
      {
        HashedLinkedList<T>.Node run1 = this.startsentinel.next;
        HashedLinkedList<T>.Node node3 = (HashedLinkedList<T>.Node) null;
        HashedLinkedList<T>.Node prev;
        for (; run1 != null && run1.prev != null; run1 = prev)
        {
          prev = run1.prev.prev;
          HashedLinkedList<T>.Node node4 = HashedLinkedList<T>.mergeRuns(run1, run1.prev, c);
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
      HashedLinkedList<T>.Node next2 = this.startsentinel.next;
      HashedLinkedList<T>.Node endsentinel = this.endsentinel;
      int lowbound;
      int highbound;
      HashedLinkedList<T>.TagGroup taggroup = this.gettaggroup(this.startsentinel, this.endsentinel, out lowbound, out highbound);
      int num1 = highbound / (this.size + 1) - lowbound / (this.size + 1);
      int num2 = num1 == 0 ? 1 : num1;
      if (this.underlying == null)
        this.taggroups = 1;
      for (; next2 != endsentinel; next2 = next2.next)
      {
        lowbound = lowbound + num2 > highbound ? highbound : lowbound + num2;
        next2.tag = lowbound;
        ++taggroup.count;
        next2.taggroup = taggroup;
      }
      if (taggroup != this.startsentinel.taggroup)
        taggroup.first = this.startsentinel.next;
      if (taggroup != this.endsentinel.taggroup)
        taggroup.last = this.endsentinel.prev;
      if (lowbound == highbound)
        this.splittaggroup(taggroup);
      (this.underlying ?? this).raiseCollectionChanged();
    }

    private static HashedLinkedList<T>.Node mergeRuns(
      HashedLinkedList<T>.Node run1,
      HashedLinkedList<T>.Node run2,
      IComparer<T> c)
    {
      HashedLinkedList<T>.Node node1;
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
      HashedLinkedList<T>.Node node2 = node1;
      node2.prev = (HashedLinkedList<T>.Node) null;
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
      HashedLinkedList<T>.Node next = this.startsentinel.next;
      int num = 0;
      for (; next != this.endsentinel; next = next.next)
      {
        next.item = arrayList[num++];
        this.dict[next.item] = next;
      }
      (this.underlying ?? this).raiseCollectionChanged();
    }

    public IDirectedCollectionValue<T> this[int start, int count]
    {
      get
      {
        this.validitycheck();
        this.checkRange(start, count);
        return (IDirectedCollectionValue<T>) new HashedLinkedList<T>.Range(this, start, count, true);
      }
    }

    public virtual int IndexOf(T item)
    {
      this.validitycheck();
      HashedLinkedList<T>.Node node;
      if (!this.dict.Find(ref item, out node) || !this.insideview(node))
        return ~this.size;
      HashedLinkedList<T>.Node next = this.startsentinel.next;
      int index = 0;
      return this.find(item, ref next, ref index) ? index : ~this.size;
    }

    public virtual int LastIndexOf(T item) => this.IndexOf(item);

    public virtual T RemoveAt(int i)
    {
      this.updatecheck();
      T key = this.remove(this.get(i), i);
      this.dict.Remove(key);
      if (this.ActiveEvents != EventTypeEnum.None)
        (this.underlying ?? this).raiseForRemoveAt(this.Offset + i, key);
      return key;
    }

    public virtual void RemoveInterval(int start, int count)
    {
      this.updatecheck();
      this.checkRange(start, count);
      if (count == 0)
        return;
      this.View(start, count).Clear();
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

    public virtual Speed ContainsSpeed => Speed.Constant;

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
      return this.contains(item, out HashedLinkedList<T>.Node _);
    }

    public virtual bool Find(ref T item)
    {
      this.validitycheck();
      HashedLinkedList<T>.Node node;
      if (!this.contains(item, out node))
        return false;
      item = node.item;
      return true;
    }

    public virtual bool Update(T item) => this.Update(item, out T _);

    public virtual bool Update(T item, out T olditem)
    {
      this.updatecheck();
      HashedLinkedList<T>.Node node;
      if (this.contains(item, out node))
      {
        olditem = node.item;
        node.item = item;
        this.dict.Update(item, node);
        (this.underlying ?? this).raiseForUpdate(item, olditem);
        return true;
      }
      olditem = default (T);
      return false;
    }

    public virtual bool FindOrAdd(ref T item)
    {
      this.updatecheck();
      HashedLinkedList<T>.Node node = new HashedLinkedList<T>.Node(item);
      if (!this.dict.FindOrAdd(item, ref node))
      {
        this.insertNode(true, this.endsentinel, node);
        (this.underlying ?? this).raiseForAdd(item);
        return false;
      }
      item = this.insideview(node) ? node.item : throw new ArgumentException("Item alredy in indexed list but outside view");
      return true;
    }

    public virtual bool UpdateOrAdd(T item) => this.UpdateOrAdd(item, out T _);

    public virtual bool UpdateOrAdd(T item, out T olditem)
    {
      this.updatecheck();
      HashedLinkedList<T>.Node node = new HashedLinkedList<T>.Node(item);
      if (this.dict.FindOrAdd(item, ref node))
      {
        olditem = this.insideview(node) ? node.item : throw new ArgumentException("Item in indexed list but outside view");
        this.dict.Update(item, node);
        node.item = item;
        (this.underlying ?? this).raiseForUpdate(item, olditem);
        return true;
      }
      this.insertNode(true, this.endsentinel, node);
      (this.underlying ?? this).raiseForAdd(item);
      olditem = default (T);
      return false;
    }

    public virtual bool Remove(T item)
    {
      this.updatecheck();
      int index = 0;
      HashedLinkedList<T>.Node node;
      if (!this.dictremove(item, out node))
        return false;
      T obj = this.remove(node, index);
      (this.underlying ?? this).raiseForRemove(obj);
      return true;
    }

    public virtual bool Remove(T item, out T removeditem)
    {
      this.updatecheck();
      int index = 0;
      HashedLinkedList<T>.Node node;
      if (!this.dictremove(item, out node))
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
      foreach (T obj in items)
      {
        HashedLinkedList<T>.Node node;
        if (this.dictremove(obj, out node))
        {
          if (mustFire)
            removeAllHandler.Remove(node.item);
          this.remove(node, 118);
        }
      }
      removeAllHandler.Raise();
    }

    private void RemoveAll(Func<T, bool> predicate)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      for (HashedLinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel; next = next.next)
      {
        bool flag = predicate(next.item);
        this.updatecheck();
        if (flag)
        {
          this.dict.Remove(next.item);
          this.remove(next, 119);
          if (mustFire)
            removeAllHandler.Remove(next.item);
        }
      }
      removeAllHandler.Raise();
    }

    public virtual void Clear()
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      int size = this.size;
      if (this.underlying == null)
      {
        this.dict.Clear();
      }
      else
      {
        foreach (T key in (EnumerableBase<T>) this)
          this.dict.Remove(key);
      }
      this.clear();
      (this.underlying ?? this).raiseForRemoveInterval(this.Offset, size);
    }

    private void clear()
    {
      if (this.size == 0)
        return;
      HashedLinkedList<T>.ViewHandler viewHandler = new HashedLinkedList<T>.ViewHandler(this);
      if (viewHandler.viewCount > 0)
      {
        int removed = 0;
        HashedLinkedList<T>.Node next = this.startsentinel.next;
        viewHandler.skipEndpoints(0, next);
        while (next != this.endsentinel)
        {
          ++removed;
          next = next.next;
          viewHandler.updateViewSizesAndCounts(removed, next);
        }
        viewHandler.updateSentinels(this.endsentinel, this.startsentinel, this.endsentinel);
        if (this.underlying != null)
          viewHandler.updateViewSizesAndCounts(removed, this.underlying.endsentinel);
      }
      if (this.underlying != null)
      {
        for (HashedLinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel; next = next.next)
        {
          next.next.prev = this.startsentinel;
          this.startsentinel.next = next.next;
          this.removefromtaggroup(next);
        }
      }
      else
        this.taggroups = 0;
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
      HashSet<T> hashSet = new HashSet<T>(this.itemequalityComparer);
      foreach (T obj in (EnumerableBase<T>) this)
        hashSet.Add(obj);
      foreach (T obj in items)
        hashSet.Remove(obj);
      for (HashedLinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel && hashSet.Count > 0; next = next.next)
      {
        if (hashSet.Contains(next.item))
        {
          this.dict.Remove(next.item);
          this.remove(next, 119);
          if (mustFire)
            removeAllHandler.Remove(next.item);
        }
      }
      removeAllHandler.Raise();
    }

    private void RetainAll(Func<T, bool> predicate)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      for (HashedLinkedList<T>.Node next = this.startsentinel.next; next != this.endsentinel; next = next.next)
      {
        bool flag = !predicate(next.item);
        this.updatecheck();
        if (flag)
        {
          this.dict.Remove(next.item);
          this.remove(next, 119);
          if (mustFire)
            removeAllHandler.Remove(next.item);
        }
      }
      removeAllHandler.Raise();
    }

    public virtual bool ContainsAll(IEnumerable<T> items)
    {
      this.validitycheck();
      foreach (T obj in items)
      {
        if (!this.contains(obj, out HashedLinkedList<T>.Node _))
          return false;
      }
      return true;
    }

    public IList<T> FindAll(Func<T, bool> filter)
    {
      this.validitycheck();
      int stamp = this.stamp;
      HashedLinkedList<T> all = new HashedLinkedList<T>();
      HashedLinkedList<T>.Node next = this.startsentinel.next;
      HashedLinkedList<T>.Node prev = all.startsentinel;
      double num1 = (double) int.MaxValue / ((double) this.size + 1.0);
      int num2 = 1;
      HashedLinkedList<T>.TagGroup tagGroup = new HashedLinkedList<T>.TagGroup();
      all.taggroups = 1;
      for (; next != this.endsentinel; next = next.next)
      {
        bool flag = filter(next.item);
        this.modifycheck(stamp);
        if (flag)
        {
          prev.next = new HashedLinkedList<T>.Node(next.item, prev, (HashedLinkedList<T>.Node) null);
          prev = prev.next;
          ++all.size;
          all.dict.Add(next.item, prev);
          prev.taggroup = tagGroup;
          prev.tag = (int) (num1 * (double) num2++);
        }
      }
      if (all.size > 0)
      {
        tagGroup.count = all.size;
        tagGroup.first = all.startsentinel.next;
        tagGroup.last = prev;
      }
      all.endsentinel.prev = prev;
      prev.next = all.endsentinel;
      return (IList<T>) all;
    }

    public virtual int ContainsCount(T item) => !this.Contains(item) ? 0 : 1;

    public virtual ICollectionValue<T> UniqueItems() => (ICollectionValue<T>) this;

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      return (ICollectionValue<KeyValuePair<T, int>>) new MultiplicityOne<T>((ICollectionValue<T>) this);
    }

    public virtual void RemoveAllCopies(T item) => this.Remove(item);

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
      HashedLinkedList<T>.Node cursor = this.startsentinel.next;
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
      HashedLinkedList<T>.Node newnode = new HashedLinkedList<T>.Node(item);
      if (this.dict.FindOrAdd(item, ref newnode))
        return false;
      this.insertNode(true, this.endsentinel, newnode);
      (this.underlying ?? this).raiseForAdd(item);
      return true;
    }

    public virtual bool AllowsDuplicates => false;

    public virtual bool DuplicatesByCounting => true;

    public virtual void AddAll(IEnumerable<T> items)
    {
      this.updatecheck();
      int added = 0;
      HashedLinkedList<T>.Node prev = this.endsentinel.prev;
      foreach (T key in items)
      {
        HashedLinkedList<T>.Node newnode = new HashedLinkedList<T>.Node(key);
        if (!this.dict.FindOrAdd(key, ref newnode))
        {
          this.insertNode(false, this.endsentinel, newnode);
          ++added;
        }
      }
      if (added <= 0)
        return;
      this.fixViewsAfterInsert(this.endsentinel, prev, added, 0);
      this.raiseForInsertAll(prev, this.size - added, added, false);
    }

    private bool checkViews()
    {
      if (this.underlying != null)
        throw new InternalException("checkViews() called on a view");
      if (this.views == null)
        return true;
      bool flag = true;
      HashedLinkedList<T>.Node[] nodeArray = new HashedLinkedList<T>.Node[this.size + 2];
      int num = 0;
      for (HashedLinkedList<T>.Node node = this.startsentinel; node != null; node = node.next)
        nodeArray[num++] = node;
      foreach (HashedLinkedList<T> view in this.views)
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

    private string zeitem(HashedLinkedList<T>.Node node)
    {
      return node != null ? node.item.ToString() : "(null node)";
    }

    public virtual bool Check()
    {
      bool flag1 = true;
      if (this.underlying != null)
        return this.underlying.Check();
      if (this.startsentinel == null)
      {
        Logger.Log("startsentinel == null");
        flag1 = false;
      }
      if (this.endsentinel == null)
      {
        Logger.Log("endsentinel == null");
        flag1 = false;
      }
      if (this.size == 0)
      {
        if (this.startsentinel != null && this.startsentinel.next != this.endsentinel)
        {
          Logger.Log("size == 0 but startsentinel.next != endsentinel");
          flag1 = false;
        }
        if (this.endsentinel != null && this.endsentinel.prev != this.startsentinel)
        {
          Logger.Log("size == 0 but endsentinel.prev != startsentinel");
          flag1 = false;
        }
      }
      if (this.startsentinel == null)
      {
        Logger.Log("NULL startsentinel");
        return flag1;
      }
      int num1 = 0;
      HashedLinkedList<T>.Node next1 = this.startsentinel.next;
      HashedLinkedList<T>.Node node1 = this.startsentinel;
      int num2 = 0;
      int num3 = 9;
      int num4 = 0;
      HashedLinkedList<T>.TagGroup tagGroup = (HashedLinkedList<T>.TagGroup) null;
      if (this.underlying == null)
      {
        HashedLinkedList<T>.TagGroup taggroup1 = this.startsentinel.taggroup;
        if (taggroup1.count != 0 || taggroup1.first != null || taggroup1.last != null || taggroup1.tag != int.MinValue)
        {
          Logger.Log(string.Format("Bad startsentinel tag group: {0}", (object) taggroup1));
          flag1 = false;
        }
        HashedLinkedList<T>.TagGroup taggroup2 = this.endsentinel.taggroup;
        if (taggroup2.count != 0 || taggroup2.first != null || taggroup2.last != null || taggroup2.tag != int.MaxValue)
        {
          Logger.Log(string.Format("Bad endsentinel tag group: {0}", (object) taggroup2));
          flag1 = false;
        }
      }
      while (next1 != this.endsentinel)
      {
        ++num1;
        if (next1.prev != node1)
        {
          Logger.Log(string.Format("Bad backpointer at node {0}", (object) num1));
          flag1 = false;
        }
        if (this.underlying == null)
        {
          if (!next1.prev.precedes(next1))
          {
            Logger.Log(string.Format("node.prev.tag ({0}, {1}) >= node.tag ({2}, {3}) at index={4} item={5} ", (object) next1.prev.taggroup.tag, (object) next1.prev.tag, (object) next1.taggroup.tag, (object) next1.tag, (object) num1, (object) next1.item));
            flag1 = false;
          }
          if (next1.taggroup != tagGroup)
          {
            if (next1.taggroup.first != next1)
            {
              Logger.Log(string.Format("Bad first pointer in taggroup: node.taggroup.first.item ({0}), node.item ({1}) at index={2} item={3}", (object) this.zeitem(next1.taggroup.first), (object) next1.item, (object) num1, (object) next1.item));
              flag1 = false;
            }
            if (tagGroup != null)
            {
              if (tagGroup.count != num2)
              {
                Logger.Log(string.Format("Bad taggroupsize: oldtg.count ({0}) != taggroupsize ({1}) at index={2} item={3}", (object) tagGroup.count, (object) num2, (object) num1, (object) next1.item));
                flag1 = false;
              }
              if (num3 <= 8 && num2 <= 8)
              {
                Logger.Log(string.Format("Two small taggroups in a row: oldtaggroupsize ({0}), taggroupsize ({1}) at index={2} item={3}", (object) num3, (object) num2, (object) num1, (object) next1.item));
                flag1 = false;
              }
              if (next1.taggroup.tag <= tagGroup.tag)
              {
                Logger.Log(string.Format("Taggroup tags not strictly increasing: oldtaggrouptag ({0}), taggrouptag ({1}) at index={2} item={3}", (object) tagGroup.tag, (object) next1.taggroup.tag, (object) num1, (object) next1.item));
                flag1 = false;
              }
              if (tagGroup.last != next1.prev)
              {
                Logger.Log(string.Format("Bad last pointer in taggroup: oldtg.last.item ({0}), node.prev.item ({1}) at index={2} item={3}", (object) tagGroup.last.item, (object) next1.prev.item, (object) num1, (object) next1.item));
                flag1 = false;
              }
              num3 = num2;
            }
            ++num4;
            tagGroup = next1.taggroup;
            num2 = 1;
          }
          else
            ++num2;
        }
        node1 = next1;
        next1 = next1.next;
        if (next1 == null)
        {
          Logger.Log(string.Format("Null next pointer at node {0}", (object) num1));
          return false;
        }
      }
      if (this.underlying == null && this.size == 0 && this.taggroups != 0)
      {
        Logger.Log(string.Format("Bad taggroups for empty list: size={0}   taggroups={1}", (object) this.size, (object) this.taggroups));
        flag1 = false;
      }
      if (this.underlying == null && this.size > 0)
      {
        HashedLinkedList<T>.TagGroup taggroup = next1.prev.taggroup;
        if (taggroup != null)
        {
          if (taggroup.count != num2)
          {
            Logger.Log(string.Format("Bad taggroupsize: oldtg.count ({0}) != taggroupsize ({1}) at index={2} item={3}", (object) taggroup.count, (object) num2, (object) num1, (object) next1.item));
            flag1 = false;
          }
          if (num3 <= 8 && num2 <= 8)
          {
            Logger.Log(string.Format("Two small taggroups in a row: oldtaggroupsize ({0}), taggroupsize ({1}) at index={2} item={3}", (object) num3, (object) num2, (object) num1, (object) next1.item));
            flag1 = false;
          }
          if (next1.taggroup.tag <= taggroup.tag)
          {
            Logger.Log(string.Format("Taggroup tags not strictly increasing: oldtaggrouptag ({0}), taggrouptag ({1}) at index={2} item={3}", (object) taggroup.tag, (object) next1.taggroup.tag, (object) num1, (object) next1.item));
            flag1 = false;
          }
          if (taggroup.last != next1.prev)
          {
            Logger.Log(string.Format("Bad last pointer in taggroup: oldtg.last.item ({0}), node.prev.item ({1}) at index={2} item={3}", (object) this.zeitem(taggroup.last), (object) this.zeitem(next1.prev), (object) num1, (object) next1.item));
            flag1 = false;
          }
        }
        if (num4 != this.taggroups)
        {
          Logger.Log(string.Format("seentaggroups ({0}) != taggroups ({1}) (at size {2})", (object) num4, (object) this.taggroups, (object) this.size));
          flag1 = false;
        }
      }
      if (num1 != this.size)
      {
        Logger.Log(string.Format("size={0} but enumeration gives {1} nodes ", (object) this.size, (object) num1));
        flag1 = false;
      }
      bool flag2 = this.checkViews() && flag1;
      if (!flag2)
        return false;
      if (this.underlying == null)
      {
        if (this.size != this.dict.Count)
        {
          Logger.Log(string.Format("list.size ({0}) != dict.Count ({1})", (object) this.size, (object) this.dict.Count));
          flag2 = false;
        }
        for (HashedLinkedList<T>.Node next2 = this.startsentinel.next; next2 != this.endsentinel; next2 = next2.next)
        {
          HashedLinkedList<T>.Node node2;
          if (!this.dict.Find(ref next2.item, out node2))
          {
            Logger.Log(string.Format("Item in list but not dict: {0}", (object) next2.item));
            flag2 = false;
          }
          else if (next2 != node2)
          {
            Logger.Log(string.Format("Wrong node in dict for item: {0}", (object) next2.item));
            flag2 = false;
          }
        }
      }
      return flag2;
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
      public HashedLinkedList<T>.Node prev;
      public HashedLinkedList<T>.Node next;
      public T item;
      internal int tag;
      internal HashedLinkedList<T>.TagGroup taggroup;

      internal bool precedes(HashedLinkedList<T>.Node that)
      {
        int tag1 = this.taggroup.tag;
        int tag2 = that.taggroup.tag;
        if (tag1 < tag2)
          return true;
        return tag1 <= tag2 && this.tag < that.tag;
      }

      internal Node(T item) => this.item = item;

      internal Node(T item, HashedLinkedList<T>.Node prev, HashedLinkedList<T>.Node next)
      {
        this.item = item;
        this.prev = prev;
        this.next = next;
      }

      public override string ToString()
      {
        return string.Format("Node: (item={0}, tag={1})", (object) this.item, (object) this.tag);
      }
    }

    [Serializable]
    private class TagGroup
    {
      internal int tag;
      internal int count;
      internal HashedLinkedList<T>.Node first;
      internal HashedLinkedList<T>.Node last;

      public override string ToString()
      {
        return string.Format("TagGroup(tag={0}, cnt={1}, fst={2}, lst={3})", (object) this.tag, (object) this.count, (object) this.first, (object) this.last);
      }
    }

    [Serializable]
    private class PositionComparer : IComparer<HashedLinkedList<T>.Position>
    {
      private static HashedLinkedList<T>.PositionComparer _default;

      private PositionComparer()
      {
      }

      public static HashedLinkedList<T>.PositionComparer Default
      {
        get
        {
          return HashedLinkedList<T>.PositionComparer._default ?? (HashedLinkedList<T>.PositionComparer._default = new HashedLinkedList<T>.PositionComparer());
        }
      }

      public int Compare(HashedLinkedList<T>.Position a, HashedLinkedList<T>.Position b)
      {
        if (a.Endpoint == b.Endpoint)
          return 0;
        return !a.Endpoint.precedes(b.Endpoint) ? 1 : -1;
      }
    }

    private struct Position
    {
      public readonly HashedLinkedList<T> View;
      public bool Left;
      public readonly HashedLinkedList<T>.Node Endpoint;

      public Position(HashedLinkedList<T> view, bool left)
      {
        this.View = view;
        this.Left = left;
        this.Endpoint = left ? view.startsentinel.next : view.endsentinel.prev;
      }

      public Position(HashedLinkedList<T>.Node node, int foo)
      {
        this.Endpoint = node;
        this.View = (HashedLinkedList<T>) null;
        this.Left = false;
      }
    }

    private struct ViewHandler
    {
      private ArrayList<HashedLinkedList<T>.Position> leftEnds;
      private ArrayList<HashedLinkedList<T>.Position> rightEnds;
      private int leftEndIndex;
      private int rightEndIndex;
      private int leftEndIndex2;
      private int rightEndIndex2;
      internal readonly int viewCount;

      internal ViewHandler(HashedLinkedList<T> list)
      {
        this.leftEndIndex = this.rightEndIndex = this.leftEndIndex2 = this.rightEndIndex2 = this.viewCount = 0;
        this.leftEnds = this.rightEnds = (ArrayList<HashedLinkedList<T>.Position>) null;
        if (list.views != null)
        {
          foreach (HashedLinkedList<T> view in list.views)
          {
            if (view != list)
            {
              if (this.leftEnds == null)
              {
                this.leftEnds = new ArrayList<HashedLinkedList<T>.Position>();
                this.rightEnds = new ArrayList<HashedLinkedList<T>.Position>();
              }
              this.leftEnds.Add(new HashedLinkedList<T>.Position(view, true));
              this.rightEnds.Add(new HashedLinkedList<T>.Position(view, false));
            }
          }
        }
        if (this.leftEnds == null)
          return;
        this.viewCount = this.leftEnds.Count;
        this.leftEnds.Sort((IComparer<HashedLinkedList<T>.Position>) HashedLinkedList<T>.PositionComparer.Default);
        this.rightEnds.Sort((IComparer<HashedLinkedList<T>.Position>) HashedLinkedList<T>.PositionComparer.Default);
      }

      internal void skipEndpoints(int removed, HashedLinkedList<T>.Node n)
      {
        if (this.viewCount > 0)
        {
          HashedLinkedList<T>.Position leftEnd;
          for (; this.leftEndIndex < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex]).Endpoint.prev.precedes(n); ++this.leftEndIndex)
          {
            HashedLinkedList<T> view = leftEnd.View;
            HashedLinkedList<T> hashedLinkedList = view;
            int? offset = view.offset;
            int num = removed;
            int? nullable = offset.HasValue ? new int?(offset.GetValueOrDefault() - num) : new int?();
            hashedLinkedList.offset = nullable;
            view.size += removed;
          }
          HashedLinkedList<T>.Position rightEnd;
          for (; this.rightEndIndex < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex]).Endpoint.precedes(n); ++this.rightEndIndex)
            rightEnd.View.size -= removed;
        }
        if (this.viewCount <= 0)
          return;
        while (this.leftEndIndex2 < this.viewCount && this.leftEnds[this.leftEndIndex2].Endpoint.prev.precedes(n))
          ++this.leftEndIndex2;
        while (this.rightEndIndex2 < this.viewCount && this.rightEnds[this.rightEndIndex2].Endpoint.next.precedes(n))
          ++this.rightEndIndex2;
      }

      internal void updateViewSizesAndCounts(int removed, HashedLinkedList<T>.Node n)
      {
        if (this.viewCount <= 0)
          return;
        HashedLinkedList<T>.Position leftEnd;
        for (; this.leftEndIndex < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex]).Endpoint.prev.precedes(n); ++this.leftEndIndex)
        {
          HashedLinkedList<T> view = leftEnd.View;
          HashedLinkedList<T> hashedLinkedList = view;
          int? offset = view.offset;
          int num = removed;
          int? nullable = offset.HasValue ? new int?(offset.GetValueOrDefault() - num) : new int?();
          hashedLinkedList.offset = nullable;
          view.size += removed;
        }
        HashedLinkedList<T>.Position rightEnd;
        for (; this.rightEndIndex < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex]).Endpoint.precedes(n); ++this.rightEndIndex)
          rightEnd.View.size -= removed;
      }

      internal void updateSentinels(
        HashedLinkedList<T>.Node n,
        HashedLinkedList<T>.Node newstart,
        HashedLinkedList<T>.Node newend)
      {
        if (this.viewCount <= 0)
          return;
        HashedLinkedList<T>.Position leftEnd;
        for (; this.leftEndIndex2 < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex2]).Endpoint.prev.precedes(n); ++this.leftEndIndex2)
          leftEnd.View.startsentinel = newstart;
        HashedLinkedList<T>.Position rightEnd;
        for (; this.rightEndIndex2 < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex2]).Endpoint.next.precedes(n); ++this.rightEndIndex2)
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
      private HashedLinkedList<T>.Node startnode;
      private HashedLinkedList<T>.Node endnode;
      private HashedLinkedList<T> list;
      private bool forwards;

      internal Range(HashedLinkedList<T> list, int start, int count, bool forwards)
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
          HashedLinkedList<T>.Node cursor = this.forwards ? this.startnode : this.endnode;
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
        HashedLinkedList<T>.Range range = (HashedLinkedList<T>.Range) this.MemberwiseClone();
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
