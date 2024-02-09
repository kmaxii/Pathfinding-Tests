// Decompiled with JetBrains decompiler
// Type: C5.ArrayList`1
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
  public class ArrayList<T> : 
    ArrayBase<T>,
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
    private bool isValid = true;
    private ArrayList<T> underlying;
    private WeakViewList<ArrayList<T>> views;
    private WeakViewList<ArrayList<T>>.Node myWeakReference;
    private bool fIFO;

    private int underlyingsize => (this.underlying ?? this).size;

    public override EventTypeEnum ListenableEvents
    {
      get => this.underlying != null ? EventTypeEnum.None : EventTypeEnum.All;
    }

    private bool equals(T i1, T i2) => this.itemequalityComparer.Equals(i1, i2);

    private void addtosize(int delta)
    {
      this.size += delta;
      if (this.underlying == null)
        return;
      this.underlying.size += delta;
    }

    protected override void expand() => this.expand(2 * this.array.Length, this.underlyingsize);

    protected override void expand(int newcapacity, int newsize)
    {
      if (this.underlying != null)
      {
        this.underlying.expand(newcapacity, newsize);
      }
      else
      {
        base.expand(newcapacity, newsize);
        if (this.views == null)
          return;
        foreach (ArrayBase<T> view in this.views)
          view.array = this.array;
      }
    }

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
      if (this.stamp != stamp)
        throw new CollectionModifiedException();
    }

    private int indexOf(T item)
    {
      for (int index = 0; index < this.size; ++index)
      {
        if (this.equals(item, this.array[this.offsetField + index]))
          return index;
      }
      return ~this.size;
    }

    private int lastIndexOf(T item)
    {
      for (int index = this.size - 1; index >= 0; --index)
      {
        if (this.equals(item, this.array[this.offsetField + index]))
          return index;
      }
      return ~this.size;
    }

    protected override void InsertProtected(int i, T item) => this.baseinsert(i, item);

    private void baseinsert(int i, T item)
    {
      if (this.underlyingsize == this.array.Length)
        this.expand();
      i += this.offsetField;
      if (i < this.underlyingsize)
        Array.Copy((Array) this.array, i, (Array) this.array, i + 1, this.underlyingsize - i);
      this.array[i] = item;
      this.addtosize(1);
      this.fixViewsAfterInsert(1, i);
    }

    private T removeAt(int i)
    {
      i += this.offsetField;
      this.fixViewsBeforeSingleRemove(i);
      T obj = this.array[i];
      this.addtosize(-1);
      if (this.underlyingsize > i)
        Array.Copy((Array) this.array, i + 1, (Array) this.array, i, this.underlyingsize - i);
      this.array[this.underlyingsize] = default (T);
      return obj;
    }

    private void fixViewsAfterInsert(int added, int realInsertionIndex)
    {
      if (this.views == null)
        return;
      foreach (ArrayList<T> view in this.views)
      {
        if (view != this)
        {
          if (view.offsetField < realInsertionIndex && view.offsetField + view.size > realInsertionIndex)
            view.size += added;
          if (view.offsetField > realInsertionIndex || view.offsetField == realInsertionIndex && view.size > 0)
            view.offsetField += added;
        }
      }
    }

    private void fixViewsBeforeSingleRemove(int realRemovalIndex)
    {
      if (this.views == null)
        return;
      foreach (ArrayList<T> view in this.views)
      {
        if (view != this)
        {
          if (view.offsetField <= realRemovalIndex && view.offsetField + view.size > realRemovalIndex)
            --view.size;
          if (view.offsetField > realRemovalIndex)
            --view.offsetField;
        }
      }
    }

    private void fixViewsBeforeRemove(int start, int count)
    {
      int num1 = start + count - 1;
      if (this.views == null)
        return;
      foreach (ArrayList<T> view in this.views)
      {
        if (view != this)
        {
          int offsetField = view.offsetField;
          int num2 = offsetField + view.size - 1;
          if (start < offsetField)
          {
            if (num1 < offsetField)
            {
              view.offsetField = offsetField - count;
            }
            else
            {
              view.offsetField = start;
              view.size = num1 < num2 ? num2 - num1 : 0;
            }
          }
          else if (start <= num2)
            view.size = num1 <= num2 ? view.size - count : start - offsetField;
        }
      }
    }

    private MutualViewPosition viewPosition(int otherOffset, int otherSize)
    {
      int num1 = this.offsetField + this.size;
      int num2 = otherOffset + otherSize;
      if (otherOffset >= num1 || num2 <= this.offsetField)
        return MutualViewPosition.NonOverlapping;
      if (this.size == 0 || otherOffset <= this.offsetField && num1 <= num2)
        return MutualViewPosition.Contains;
      return otherSize == 0 || this.offsetField <= otherOffset && num2 <= num1 ? MutualViewPosition.ContainedIn : MutualViewPosition.Overlapping;
    }

    private void disposeOverlappingViews(bool reverse)
    {
      if (this.views == null)
        return;
      foreach (ArrayList<T> view in this.views)
      {
        if (view != this)
        {
          switch (this.viewPosition(view.offsetField, view.size))
          {
            case MutualViewPosition.ContainedIn:
              if (reverse)
              {
                view.offsetField = 2 * this.offsetField + this.size - view.size - view.offsetField;
                continue;
              }
              view.Dispose();
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

    public ArrayList()
      : this(8)
    {
    }

    public ArrayList(MemoryType memoryType = MemoryType.Normal)
      : this(8, memoryType)
    {
    }

    public ArrayList(IEqualityComparer<T> itemequalityComparer, MemoryType memoryType = MemoryType.Normal)
      : this(8, itemequalityComparer, memoryType)
    {
    }

    public ArrayList(int capacity, MemoryType memoryType = MemoryType.Normal)
      : this(capacity, C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    public ArrayList(
      int capacity,
      IEqualityComparer<T> itemequalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(capacity, itemequalityComparer, memoryType)
    {
    }

    public virtual T First
    {
      get
      {
        this.validitycheck();
        if (this.size == 0)
          throw new NoSuchItemException();
        return this.array[this.offsetField];
      }
    }

    public virtual T Last
    {
      get
      {
        this.validitycheck();
        if (this.size == 0)
          throw new NoSuchItemException();
        return this.array[this.offsetField + this.size - 1];
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
        if (index < 0 || index >= this.size)
          throw new IndexOutOfRangeException();
        return this.array[this.offsetField + index];
      }
      set
      {
        this.updatecheck();
        if (index < 0 || index >= this.size)
          throw new IndexOutOfRangeException();
        index += this.offsetField;
        T obj = this.array[index];
        this.array[index] = value;
        (this.underlying ?? this).raiseForSetThis(index, value, obj);
      }
    }

    public virtual Speed IndexingSpeed => Speed.Constant;

    public virtual void Insert(int index, T item)
    {
      this.updatecheck();
      if (index < 0 || index > this.size)
        throw new IndexOutOfRangeException();
      this.InsertProtected(index, item);
      (this.underlying ?? this).raiseForInsert(index + this.offsetField, item);
    }

    public void Insert(IList<T> pointer, T item)
    {
      if (pointer == null || (pointer.Underlying ?? pointer) != (this.underlying ?? this))
        throw new IncompatibleViewException();
      this.Insert(pointer.Offset + pointer.Count - this.Offset, item);
    }

    public virtual void InsertAll(int index, IEnumerable<T> items)
    {
      this.updatecheck();
      if (index < 0 || index > this.size)
        throw new IndexOutOfRangeException();
      index += this.offsetField;
      int num1 = EnumerableBase<T>.countItems(items);
      if (num1 == 0)
        return;
      if (num1 + this.underlyingsize > this.array.Length)
        this.expand(num1 + this.underlyingsize, this.underlyingsize);
      if (this.underlyingsize > index)
        Array.Copy((Array) this.array, index, (Array) this.array, index + num1, this.underlyingsize - index);
      int destinationIndex = index;
      try
      {
        foreach (T obj in items)
          this.array[destinationIndex++] = obj;
      }
      finally
      {
        int num2 = destinationIndex - index;
        if (num2 < num1)
        {
          Array.Copy((Array) this.array, index + num1, (Array) this.array, destinationIndex, this.underlyingsize - index);
          Array.Clear((Array) this.array, this.underlyingsize + num2, num1 - num2);
        }
        if (num2 > 0)
        {
          this.addtosize(num2);
          this.fixViewsAfterInsert(num2, index);
          (this.underlying ?? this).raiseForInsertAll(index, num2);
        }
      }
    }

    private void raiseForInsertAll(int index, int added)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      if ((this.ActiveEvents & (EventTypeEnum.Added | EventTypeEnum.Inserted)) != EventTypeEnum.None)
      {
        for (int index1 = index; index1 < index + added; ++index1)
        {
          this.raiseItemInserted(this.array[index1], index1);
          this.raiseItemsAdded(this.array[index1], 1);
        }
      }
      this.raiseCollectionChanged();
    }

    public virtual void InsertFirst(T item)
    {
      this.updatecheck();
      this.InsertProtected(0, item);
      (this.underlying ?? this).raiseForInsert(this.offsetField, item);
    }

    public virtual void InsertLast(T item)
    {
      this.updatecheck();
      this.InsertProtected(this.size, item);
      (this.underlying ?? this).raiseForInsert(this.size - 1 + this.offsetField, item);
    }

    public virtual IList<T> FindAll(Func<T, bool> filter)
    {
      this.validitycheck();
      int stamp = this.stamp;
      ArrayList<T> all = new ArrayList<T>(this.itemequalityComparer);
      int newsize = 0;
      int num = all.array.Length;
      for (int index = 0; index < this.size; ++index)
      {
        T obj = this.array[this.offsetField + index];
        bool flag = filter(obj);
        this.modifycheck(stamp);
        if (flag)
        {
          if (newsize == num)
            all.expand(num = 2 * num, newsize);
          all.array[newsize++] = obj;
        }
      }
      all.size = newsize;
      return (IList<T>) all;
    }

    public virtual IList<V> Map<V>(Func<T, V> mapper)
    {
      this.validitycheck();
      ArrayList<V> res = new ArrayList<V>(this.size);
      return this.map<V>(mapper, res);
    }

    public virtual IList<V> Map<V>(Func<T, V> mapper, IEqualityComparer<V> itemequalityComparer)
    {
      this.validitycheck();
      ArrayList<V> res = new ArrayList<V>(this.size, itemequalityComparer);
      return this.map<V>(mapper, res);
    }

    private IList<V> map<V>(Func<T, V> mapper, ArrayList<V> res)
    {
      int stamp = this.stamp;
      if (this.size > 0)
      {
        for (int index = 0; index < this.size; ++index)
        {
          V v = mapper(this.array[this.offsetField + index]);
          this.modifycheck(stamp);
          res.array[index] = v;
        }
      }
      res.size = this.size;
      return (IList<V>) res;
    }

    public virtual T Remove()
    {
      this.updatecheck();
      T obj = this.size != 0 ? this.removeAt(this.fIFO ? 0 : this.size - 1) : throw new NoSuchItemException("List is empty");
      (this.underlying ?? this).raiseForRemove(obj);
      return obj;
    }

    public virtual T RemoveFirst()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException("List is empty");
      T obj = this.removeAt(0);
      (this.underlying ?? this).raiseForRemoveAt(this.offsetField, obj);
      return obj;
    }

    public virtual T RemoveLast()
    {
      this.updatecheck();
      T obj = this.size != 0 ? this.removeAt(this.size - 1) : throw new NoSuchItemException("List is empty");
      (this.underlying ?? this).raiseForRemoveAt(this.size + this.offsetField, obj);
      return obj;
    }

    public virtual IList<T> View(int start, int count)
    {
      this.validitycheck();
      this.checkRange(start, count);
      if (this.views == null)
        this.views = new WeakViewList<ArrayList<T>>();
      ArrayList<T> view = (ArrayList<T>) this.MemberwiseClone();
      view.underlying = this.underlying != null ? this.underlying : this;
      view.offsetField = start + this.offsetField;
      view.size = count;
      view.myWeakReference = this.views.Add(view);
      return (IList<T>) view;
    }

    public virtual IList<T> ViewOf(T item)
    {
      int start = this.indexOf(item);
      return start < 0 ? (IList<T>) null : this.View(start, 1);
    }

    public virtual IList<T> LastViewOf(T item)
    {
      int start = this.lastIndexOf(item);
      return start < 0 ? (IList<T>) null : this.View(start, 1);
    }

    public virtual IList<T> Underlying => (IList<T>) this.underlying;

    public virtual int Offset => this.offsetField;

    public virtual bool IsValid => this.isValid;

    public virtual IList<T> Slide(int offset)
    {
      if (!this.TrySlide(offset, this.size))
        throw new ArgumentOutOfRangeException();
      return (IList<T>) this;
    }

    public virtual IList<T> Slide(int offset, int size)
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
        throw new NotAViewException("Not a view");
      int num1 = this.offsetField + offset;
      int num2 = size;
      if (num1 < 0 || num2 < 0 || num1 + num2 > this.underlyingsize)
        return false;
      this.offsetField = num1;
      this.size = num2;
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
      int num1 = 0;
      int num2 = this.size / 2;
      int num3 = this.offsetField + this.size - 1;
      for (; num1 < num2; ++num1)
      {
        T obj = this.array[this.offsetField + num1];
        this.array[this.offsetField + num1] = this.array[num3 - num1];
        this.array[num3 - num1] = obj;
      }
      this.disposeOverlappingViews(true);
      (this.underlying ?? this).raiseCollectionChanged();
    }

    public bool IsSorted() => this.IsSorted((IComparer<T>) Comparer<T>.Default);

    public virtual bool IsSorted(IComparer<T> c)
    {
      this.validitycheck();
      int index1 = this.offsetField + 1;
      for (int index2 = this.offsetField + this.size; index1 < index2; ++index1)
      {
        if (c.Compare(this.array[index1 - 1], this.array[index1]) > 0)
          return false;
      }
      return true;
    }

    public virtual void Sort() => this.Sort((IComparer<T>) Comparer<T>.Default);

    public virtual void Sort(IComparer<T> comparer)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      Sorting.IntroSort<T>(this.array, this.offsetField, this.size, comparer);
      this.disposeOverlappingViews(false);
      (this.underlying ?? this).raiseCollectionChanged();
    }

    public virtual void Shuffle() => this.Shuffle((Random) new C5Random());

    public virtual void Shuffle(Random rnd)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      int offsetField = this.offsetField;
      int maxValue = this.offsetField + this.size;
      for (int index1 = maxValue - 1; offsetField < index1; ++offsetField)
      {
        int index2 = rnd.Next(offsetField, maxValue);
        if (index2 != offsetField)
        {
          T obj = this.array[offsetField];
          this.array[offsetField] = this.array[index2];
          this.array[index2] = obj;
        }
      }
      this.disposeOverlappingViews(false);
      (this.underlying ?? this).raiseCollectionChanged();
    }

    public virtual int IndexOf(T item)
    {
      this.validitycheck();
      return this.indexOf(item);
    }

    public virtual int LastIndexOf(T item)
    {
      this.validitycheck();
      return this.lastIndexOf(item);
    }

    public virtual T RemoveAt(int index)
    {
      this.updatecheck();
      T obj = index >= 0 && index < this.size ? this.removeAt(index) : throw new IndexOutOfRangeException("Index out of range for sequenced collection");
      (this.underlying ?? this).raiseForRemoveAt(this.offsetField + index, obj);
      return obj;
    }

    public virtual void RemoveInterval(int start, int count)
    {
      this.updatecheck();
      if (count == 0)
        return;
      this.checkRange(start, count);
      start += this.offsetField;
      this.fixViewsBeforeRemove(start, count);
      Array.Copy((Array) this.array, start + count, (Array) this.array, start, this.underlyingsize - start - count);
      this.addtosize(-count);
      Array.Clear((Array) this.array, this.underlyingsize, count);
      (this.underlying ?? this).raiseForRemoveInterval(start, count);
    }

    private void raiseForRemoveInterval(int start, int count)
    {
      if (this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseCollectionCleared(this.size == 0, count, new int?(start));
      this.raiseCollectionChanged();
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
      return this.indexOf(item) >= 0;
    }

    public virtual bool Find(ref T item)
    {
      this.validitycheck();
      int num;
      if ((num = this.indexOf(item)) < 0)
        return false;
      item = this.array[this.offsetField + num];
      return true;
    }

    public virtual bool Update(T item) => this.Update(item, out T _);

    public virtual bool Update(T item, out T olditem)
    {
      this.updatecheck();
      int num;
      if ((num = this.indexOf(item)) >= 0)
      {
        olditem = this.array[this.offsetField + num];
        this.array[this.offsetField + num] = item;
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

    public virtual bool UpdateOrAdd(T item)
    {
      this.updatecheck();
      if (this.Update(item))
        return true;
      this.Add(item);
      return false;
    }

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
      int i = this.fIFO ? this.indexOf(item) : this.lastIndexOf(item);
      if (i < 0)
        return false;
      T obj = this.removeAt(i);
      (this.underlying ?? this).raiseForRemove(obj);
      return true;
    }

    public virtual bool Remove(T item, out T removeditem)
    {
      this.updatecheck();
      int i = this.fIFO ? this.indexOf(item) : this.lastIndexOf(item);
      if (i < 0)
      {
        removeditem = default (T);
        return false;
      }
      removeditem = this.removeAt(i);
      (this.underlying ?? this).raiseForRemove(removeditem);
      return true;
    }

    public virtual void RemoveAll(IEnumerable<T> items)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll(items);
      if (hashBag.Count == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      ArrayList<T>.ViewHandler viewHandler = new ArrayList<T>.ViewHandler(this);
      int offsetField1 = this.offsetField;
      int num1 = 0;
      int offsetField2 = this.offsetField;
      int num2 = this.offsetField + this.size;
label_12:
      while (offsetField2 < num2)
      {
        T obj1;
        while (offsetField2 < num2 && !hashBag.Contains(obj1 = this.array[offsetField2]))
        {
          this.array[offsetField1] = obj1;
          ++offsetField2;
          ++offsetField1;
        }
        viewHandler.skipEndpoints(num1, offsetField2);
        while (true)
        {
          T obj2;
          if (offsetField2 < num2 && hashBag.Remove(obj2 = this.array[offsetField2]))
          {
            if (mustFire)
              removeAllHandler.Remove(obj2);
            ++num1;
            ++offsetField2;
            viewHandler.updateViewSizesAndCounts(num1, offsetField2);
          }
          else
            goto label_12;
        }
      }
      if (num1 == 0)
        return;
      viewHandler.updateViewSizesAndCounts(num1, this.underlyingsize);
      Array.Copy((Array) this.array, this.offsetField + this.size, (Array) this.array, offsetField1, this.underlyingsize - this.offsetField - this.size);
      this.addtosize(-num1);
      Array.Clear((Array) this.array, this.underlyingsize, num1);
      if (!mustFire)
        return;
      removeAllHandler.Raise();
    }

    private void RemoveAll(Func<T, bool> predicate)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      ArrayList<T>.ViewHandler viewHandler = new ArrayList<T>.ViewHandler(this);
      int offsetField1 = this.offsetField;
      int num1 = 0;
      int offsetField2 = this.offsetField;
      int num2 = this.offsetField + this.size;
      while (offsetField2 < num2)
      {
        T obj1;
        while (offsetField2 < num2 && !predicate(obj1 = this.array[offsetField2]))
        {
          this.updatecheck();
          this.array[offsetField1] = obj1;
          ++offsetField2;
          ++offsetField1;
        }
        this.updatecheck();
        viewHandler.skipEndpoints(num1, offsetField2);
        T obj2;
        while (offsetField2 < num2 && predicate(obj2 = this.array[offsetField2]))
        {
          this.updatecheck();
          if (mustFire)
            removeAllHandler.Remove(obj2);
          ++num1;
          ++offsetField2;
          viewHandler.updateViewSizesAndCounts(num1, offsetField2);
        }
        this.updatecheck();
      }
      if (num1 == 0)
        return;
      viewHandler.updateViewSizesAndCounts(num1, this.underlyingsize);
      Array.Copy((Array) this.array, this.offsetField + this.size, (Array) this.array, offsetField1, this.underlyingsize - this.offsetField - this.size);
      this.addtosize(-num1);
      Array.Clear((Array) this.array, this.underlyingsize, num1);
      if (!mustFire)
        return;
      removeAllHandler.Raise();
    }

    public override void Clear()
    {
      if (this.underlying == null)
      {
        this.updatecheck();
        if (this.size == 0)
          return;
        int size = this.size;
        this.fixViewsBeforeRemove(0, this.size);
        this.array = new T[8];
        this.size = 0;
        (this.underlying ?? this).raiseForRemoveInterval(this.offsetField, size);
      }
      else
        this.RemoveInterval(0, this.size);
    }

    public virtual void RetainAll(IEnumerable<T> items)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll(items);
      if (hashBag.Count == 0)
      {
        this.Clear();
      }
      else
      {
        CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
        bool mustFire = removeAllHandler.MustFire;
        ArrayList<T>.ViewHandler viewHandler = new ArrayList<T>.ViewHandler(this);
        int offsetField1 = this.offsetField;
        int num1 = 0;
        int offsetField2 = this.offsetField;
        int num2 = this.offsetField + this.size;
label_12:
        while (offsetField2 < num2)
        {
          T obj1;
          while (offsetField2 < num2 && hashBag.Remove(obj1 = this.array[offsetField2]))
          {
            this.array[offsetField1] = obj1;
            ++offsetField2;
            ++offsetField1;
          }
          viewHandler.skipEndpoints(num1, offsetField2);
          while (true)
          {
            T obj2;
            if (offsetField2 < num2 && !hashBag.Contains(obj2 = this.array[offsetField2]))
            {
              if (mustFire)
                removeAllHandler.Remove(obj2);
              ++num1;
              ++offsetField2;
              viewHandler.updateViewSizesAndCounts(num1, offsetField2);
            }
            else
              goto label_12;
          }
        }
        if (num1 == 0)
          return;
        viewHandler.updateViewSizesAndCounts(num1, this.underlyingsize);
        Array.Copy((Array) this.array, this.offsetField + this.size, (Array) this.array, offsetField1, this.underlyingsize - this.offsetField - this.size);
        this.addtosize(-num1);
        Array.Clear((Array) this.array, this.underlyingsize, num1);
        removeAllHandler.Raise();
      }
    }

    private void RetainAll(Func<T, bool> predicate)
    {
      this.updatecheck();
      if (this.size == 0)
        return;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) (this.underlying ?? this));
      bool mustFire = removeAllHandler.MustFire;
      ArrayList<T>.ViewHandler viewHandler = new ArrayList<T>.ViewHandler(this);
      int offsetField1 = this.offsetField;
      int num1 = 0;
      int offsetField2 = this.offsetField;
      int num2 = this.offsetField + this.size;
      while (offsetField2 < num2)
      {
        T obj1;
        while (offsetField2 < num2 && predicate(obj1 = this.array[offsetField2]))
        {
          this.updatecheck();
          this.array[offsetField1] = obj1;
          ++offsetField2;
          ++offsetField1;
        }
        this.updatecheck();
        viewHandler.skipEndpoints(num1, offsetField2);
        T obj2;
        while (offsetField2 < num2 && !predicate(obj2 = this.array[offsetField2]))
        {
          this.updatecheck();
          if (mustFire)
            removeAllHandler.Remove(obj2);
          ++num1;
          ++offsetField2;
          viewHandler.updateViewSizesAndCounts(num1, offsetField2);
        }
        this.updatecheck();
      }
      if (num1 == 0)
        return;
      viewHandler.updateViewSizesAndCounts(num1, this.underlyingsize);
      Array.Copy((Array) this.array, this.offsetField + this.size, (Array) this.array, offsetField1, this.underlyingsize - this.offsetField - this.size);
      this.addtosize(-num1);
      Array.Clear((Array) this.array, this.underlyingsize, num1);
      removeAllHandler.Raise();
    }

    public virtual bool ContainsAll(IEnumerable<T> items)
    {
      this.validitycheck();
      HashBag<T> hashBag = new HashBag<T>(this.itemequalityComparer);
      hashBag.AddAll(items);
      if (hashBag.Count == 0)
        return true;
      int offsetField = this.offsetField;
      for (int index = this.offsetField + this.size; offsetField < index; ++offsetField)
      {
        hashBag.Remove(this.array[offsetField]);
        if (hashBag.Count == 0)
          return true;
      }
      return false;
    }

    public virtual int ContainsCount(T item)
    {
      this.validitycheck();
      int num = 0;
      for (int index = 0; index < this.size; ++index)
      {
        if (this.equals(item, this.array[this.offsetField + index]))
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
      ArrayList<T>.ViewHandler viewHandler = new ArrayList<T>.ViewHandler(this);
      int offsetField1 = this.offsetField;
      int num1 = 0;
      int offsetField2 = this.offsetField;
      int num2 = this.offsetField + this.size;
label_10:
      while (offsetField2 < num2)
      {
        while (offsetField2 < num2 && !this.equals(item, this.array[offsetField2]))
          this.array[offsetField1++] = this.array[offsetField2++];
        viewHandler.skipEndpoints(num1, offsetField2);
        while (true)
        {
          if (offsetField2 < num2 && this.equals(item, this.array[offsetField2]))
          {
            if (mustFire)
              removeAllHandler.Remove(this.array[offsetField2]);
            ++num1;
            ++offsetField2;
            viewHandler.updateViewSizesAndCounts(num1, offsetField2);
          }
          else
            goto label_10;
        }
      }
      if (num1 == 0)
        return;
      viewHandler.updateViewSizesAndCounts(num1, this.underlyingsize);
      Array.Copy((Array) this.array, this.offsetField + this.size, (Array) this.array, offsetField1, this.underlyingsize - this.offsetField - this.size);
      this.addtosize(-num1);
      Array.Clear((Array) this.array, this.underlyingsize, num1);
      removeAllHandler.Raise();
    }

    public override bool Check()
    {
      bool flag = true;
      if (this.underlyingsize > this.array.Length)
      {
        Logger.Log(string.Format("underlyingsize ({0}) > array.Length ({1})", (object) this.size, (object) this.array.Length));
        return false;
      }
      if (this.offsetField + this.size > this.underlyingsize)
      {
        Logger.Log(string.Format("offset({0})+size({1}) > underlyingsize ({2})", (object) this.offsetField, (object) this.size, (object) this.underlyingsize));
        return false;
      }
      if (this.offsetField < 0)
      {
        Logger.Log(string.Format("offset({0}) < 0", (object) this.offsetField));
        return false;
      }
      for (int index = 0; index < this.underlyingsize; ++index)
      {
        if ((object) this.array[index] == null)
        {
          Logger.Log(string.Format("Bad element: null at (base)index {0}", (object) index));
          flag = false;
        }
      }
      int underlyingsize = this.underlyingsize;
      for (int length = this.array.Length; underlyingsize < length; ++underlyingsize)
      {
        if (!this.equals(this.array[underlyingsize], default (T)))
        {
          Logger.Log(string.Format("Bad element: != default(T) at (base)index {0}", (object) underlyingsize));
          flag = false;
        }
      }
      ArrayList<T> arrayList = this.underlying ?? this;
      if (arrayList.views != null)
      {
        foreach (ArrayList<T> view in arrayList.views)
        {
          if (arrayList.array != view.array)
          {
            Logger.Log(string.Format("View from {0} of length has different base array than the underlying list", (object) view.offsetField, (object) view.size));
            flag = false;
          }
        }
      }
      return flag;
    }

    public virtual bool AllowsDuplicates => true;

    public virtual bool DuplicatesByCounting => false;

    public virtual bool Add(T item)
    {
      this.updatecheck();
      this.baseinsert(this.size, item);
      (this.underlying ?? this).raiseForAdd(item);
      return true;
    }

    public virtual void AddAll(IEnumerable<T> items)
    {
      this.updatecheck();
      int num1 = EnumerableBase<T>.countItems(items);
      if (num1 == 0)
        return;
      if (num1 + this.underlyingsize > this.array.Length)
        this.expand(num1 + this.underlyingsize, this.underlyingsize);
      int num2 = this.size + this.offsetField;
      if (this.underlyingsize > num2)
        Array.Copy((Array) this.array, num2, (Array) this.array, num2 + num1, this.underlyingsize - num2);
      try
      {
        foreach (T obj in items)
          this.array[num2++] = obj;
      }
      finally
      {
        int num3 = num2 - this.size - this.offsetField;
        if (num3 < num1)
        {
          Array.Copy((Array) this.array, this.size + this.offsetField + num1, (Array) this.array, num2, this.underlyingsize - this.size - this.offsetField);
          Array.Clear((Array) this.array, this.underlyingsize + num3, num1 - num3);
        }
        if (num3 > 0)
        {
          this.addtosize(num3);
          this.fixViewsAfterInsert(num3, num2 - num3);
          (this.underlying ?? this).raiseForAddAll(num2 - num3, num3);
        }
      }
    }

    private void raiseForAddAll(int start, int added)
    {
      if ((this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None)
      {
        int index1 = start;
        for (int index2 = start + added; index1 < index2; ++index1)
          this.raiseItemsAdded(this.array[index1], 1);
      }
      this.raiseCollectionChanged();
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public override int Count
    {
      get
      {
        this.validitycheck();
        return this.size;
      }
    }

    public override IEnumerator<T> GetEnumerator()
    {
      this.validitycheck();
      return base.GetEnumerator();
    }

    public virtual void Push(T item) => this.InsertLast(item);

    public virtual T Pop() => this.RemoveLast();

    public virtual void Enqueue(T item) => this.InsertLast(item);

    public virtual T Dequeue() => this.RemoveFirst();

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
        this.underlying = (ArrayList<T>) null;
        this.views = (WeakViewList<ArrayList<T>>) null;
        this.myWeakReference = (WeakViewList<ArrayList<T>>.Node) null;
      }
      else
      {
        if (this.views != null)
        {
          foreach (ArrayList<T> view in this.views)
            view.Dispose(true);
        }
        this.Clear();
      }
    }

    void System.Collections.Generic.IList<T>.RemoveAt(int index) => this.RemoveAt(index);

    void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

    bool ICollection.IsSynchronized => false;

    [Obsolete]
    object ICollection.SyncRoot
    {
      get
      {
        return this.underlying == null ? (object) this.array : ((ICollection) this.underlying).SyncRoot;
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
    private class PositionComparer : IComparer<ArrayList<T>.Position>
    {
      public int Compare(ArrayList<T>.Position a, ArrayList<T>.Position b)
      {
        return a.index.CompareTo(b.index);
      }
    }

    private struct Position
    {
      public readonly ArrayList<T> view;
      public readonly int index;

      public Position(ArrayList<T> view, bool left)
      {
        this.view = view;
        this.index = left ? view.offsetField : view.offsetField + view.size - 1;
      }

      public Position(int index)
      {
        this.index = index;
        this.view = (ArrayList<T>) null;
      }
    }

    private struct ViewHandler
    {
      private ArrayList<ArrayList<T>.Position> leftEnds;
      private ArrayList<ArrayList<T>.Position> rightEnds;
      private int leftEndIndex;
      private int rightEndIndex;
      internal readonly int viewCount;

      internal ViewHandler(ArrayList<T> list)
      {
        this.leftEndIndex = this.rightEndIndex = this.viewCount = 0;
        this.leftEnds = this.rightEnds = (ArrayList<ArrayList<T>.Position>) null;
        if (list.views != null)
        {
          foreach (ArrayList<T> view in list.views)
          {
            if (view != list)
            {
              if (this.leftEnds == null)
              {
                this.leftEnds = new ArrayList<ArrayList<T>.Position>();
                this.rightEnds = new ArrayList<ArrayList<T>.Position>();
              }
              this.leftEnds.Add(new ArrayList<T>.Position(view, true));
              this.rightEnds.Add(new ArrayList<T>.Position(view, false));
            }
          }
        }
        if (this.leftEnds == null)
          return;
        this.viewCount = this.leftEnds.Count;
        this.leftEnds.Sort((IComparer<ArrayList<T>.Position>) new ArrayList<T>.PositionComparer());
        this.rightEnds.Sort((IComparer<ArrayList<T>.Position>) new ArrayList<T>.PositionComparer());
      }

      internal void skipEndpoints(int removed, int realindex)
      {
        if (this.viewCount <= 0)
          return;
        ArrayList<T>.Position leftEnd;
        for (; this.leftEndIndex < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex]).index <= realindex; ++this.leftEndIndex)
        {
          ArrayList<T> view = leftEnd.view;
          view.offsetField -= removed;
          view.size += removed;
        }
        ArrayList<T>.Position rightEnd;
        for (; this.rightEndIndex < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex]).index < realindex; ++this.rightEndIndex)
          rightEnd.view.size -= removed;
      }

      internal void updateViewSizesAndCounts(int removed, int realindex)
      {
        if (this.viewCount <= 0)
          return;
        ArrayList<T>.Position leftEnd;
        for (; this.leftEndIndex < this.viewCount && (leftEnd = this.leftEnds[this.leftEndIndex]).index <= realindex; ++this.leftEndIndex)
        {
          ArrayList<T> view = leftEnd.view;
          view.offsetField = view.Offset - removed;
          view.size += removed;
        }
        ArrayList<T>.Position rightEnd;
        for (; this.rightEndIndex < this.viewCount && (rightEnd = this.rightEnds[this.rightEndIndex]).index < realindex; ++this.rightEndIndex)
          rightEnd.view.size -= removed;
      }
    }
  }
}
