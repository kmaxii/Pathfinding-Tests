// Decompiled with JetBrains decompiler
// Type: C5.SortedArray`1
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
  public class SortedArray<T> : 
    ArrayBase<T>,
    IIndexedSorted<T>,
    ISorted<T>,
    IIndexed<T>,
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
    private readonly IComparer<T> _comparer;

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    private bool BinarySearch(T item, out int middle)
    {
      int num1 = 0;
      int num2 = this.size;
      middle = num2 / 2;
      while (num2 > num1)
      {
        int num3;
        if ((num3 = this._comparer.Compare(this.array[middle], item)) == 0)
          return true;
        if (num3 > 0)
          num2 = middle;
        else
          num1 = middle + 1;
        middle = num1 + (num2 - num1) / 2;
      }
      return false;
    }

    private int indexOf(T item)
    {
      int middle;
      return this.BinarySearch(item, out middle) ? middle : ~middle;
    }

    public SortedArray(MemoryType memoryType = MemoryType.Normal)
      : this(8, memoryType)
    {
    }

    public SortedArray(int capacity, MemoryType memoryType = MemoryType.Normal)
      : this(capacity, (IComparer<T>) System.Collections.Generic.Comparer<T>.Default, C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    public SortedArray(IComparer<T> comparer, MemoryType memoryType = MemoryType.Normal)
      : this(8, comparer, memoryType)
    {
    }

    public SortedArray(int capacity, IComparer<T> comparer, MemoryType memoryType = MemoryType.Normal)
      : this(capacity, comparer, (IEqualityComparer<T>) new ComparerZeroHashCodeEqualityComparer<T>(comparer), memoryType)
    {
    }

    public SortedArray(
      int capacity,
      IComparer<T> comparer,
      IEqualityComparer<T> equalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(capacity, equalityComparer, memoryType)
    {
      this._comparer = comparer != null ? comparer : throw new NullReferenceException("Comparer cannot be null");
    }

    public int CountFrom(T bot)
    {
      int middle;
      this.BinarySearch(bot, out middle);
      return this.size - middle;
    }

    public int CountFromTo(T bot, T top)
    {
      int middle1;
      this.BinarySearch(bot, out middle1);
      int middle2;
      this.BinarySearch(top, out middle2);
      return middle2 <= middle1 ? 0 : middle2 - middle1;
    }

    public int CountTo(T top)
    {
      int middle;
      this.BinarySearch(top, out middle);
      return middle;
    }

    public IDirectedCollectionValue<T> RangeFrom(T bot)
    {
      int middle;
      this.BinarySearch(bot, out middle);
      return (IDirectedCollectionValue<T>) new ArrayBase<T>.Range((ArrayBase<T>) this, middle, this.size - middle, true, this.MemoryType);
    }

    public IDirectedCollectionValue<T> RangeFromTo(T bot, T top)
    {
      int middle1;
      this.BinarySearch(bot, out middle1);
      int middle2;
      this.BinarySearch(top, out middle2);
      int count = middle2 - middle1;
      return (IDirectedCollectionValue<T>) new ArrayBase<T>.Range((ArrayBase<T>) this, middle1, count, true, this.MemoryType);
    }

    public IDirectedCollectionValue<T> RangeTo(T top)
    {
      int middle;
      this.BinarySearch(top, out middle);
      return (IDirectedCollectionValue<T>) new ArrayBase<T>.Range((ArrayBase<T>) this, 0, middle, true, this.MemoryType);
    }

    public IIndexedSorted<T> FindAll(Func<T, bool> f)
    {
      SortedArray<T> all = new SortedArray<T>(this._comparer);
      int newsize = 0;
      int num = all.array.Length;
      for (int index = 0; index < this.size; ++index)
      {
        T obj = this.array[index];
        if (f(obj))
        {
          if (newsize == num)
            all.expand(num = 2 * num, newsize);
          all.array[newsize++] = obj;
        }
      }
      all.size = newsize;
      return (IIndexedSorted<T>) all;
    }

    public IIndexedSorted<V> Map<V>(Func<T, V> m, IComparer<V> c)
    {
      SortedArray<V> sortedArray = new SortedArray<V>(this.size, c);
      if (this.size > 0)
      {
        V x = sortedArray.array[0] = m(this.array[0]);
        for (int index = 1; index < this.size; ++index)
        {
          V v;
          if (c.Compare(x, v = sortedArray.array[index] = m(this.array[index])) >= 0)
            throw new ArgumentException("mapper not monotonic");
          x = v;
        }
      }
      sortedArray.size = this.size;
      return (IIndexedSorted<V>) sortedArray;
    }

    public bool TryPredecessor(T item, out T res)
    {
      int middle;
      this.BinarySearch(item, out middle);
      if (middle == 0)
      {
        res = default (T);
        return false;
      }
      res = this.array[middle - 1];
      return true;
    }

    public bool TrySuccessor(T item, out T res)
    {
      int middle;
      if (this.BinarySearch(item, out middle))
        ++middle;
      if (middle >= this.size)
      {
        res = default (T);
        return false;
      }
      res = this.array[middle];
      return true;
    }

    public bool TryWeakPredecessor(T item, out T res)
    {
      int middle;
      if (!this.BinarySearch(item, out middle))
        --middle;
      if (middle < 0)
      {
        res = default (T);
        return false;
      }
      res = this.array[middle];
      return true;
    }

    public bool TryWeakSuccessor(T item, out T res)
    {
      int middle;
      this.BinarySearch(item, out middle);
      if (middle >= this.size)
      {
        res = default (T);
        return false;
      }
      res = this.array[middle];
      return true;
    }

    public T Predecessor(T item)
    {
      int middle;
      this.BinarySearch(item, out middle);
      return middle != 0 ? this.array[middle - 1] : throw new NoSuchItemException();
    }

    public T Successor(T item)
    {
      int middle;
      if (this.BinarySearch(item, out middle))
        ++middle;
      return middle < this.size ? this.array[middle] : throw new NoSuchItemException();
    }

    public T WeakPredecessor(T item)
    {
      int middle;
      if (!this.BinarySearch(item, out middle))
        --middle;
      return middle >= 0 ? this.array[middle] : throw new NoSuchItemException();
    }

    public T WeakSuccessor(T item)
    {
      int middle;
      this.BinarySearch(item, out middle);
      return middle < this.size ? this.array[middle] : throw new NoSuchItemException();
    }

    public bool Cut(
      IComparable<T> c,
      out T low,
      out bool lowIsValid,
      out T high,
      out bool highIsValid)
    {
      int index1 = -1;
      int index2 = this.size;
      low = default (T);
      lowIsValid = false;
      high = default (T);
      highIsValid = false;
      int num1 = 0;
      int num2 = this.size;
      int num3 = -1;
      int index3;
      for (index3 = num2 / 2; num2 > num1 && (num3 = c.CompareTo(this.array[index3])) != 0; index3 = (num1 + num2) / 2)
      {
        if (num3 < 0)
        {
          index2 = num2 = index3;
        }
        else
        {
          index1 = index3;
          num1 = index3 + 1;
        }
      }
      if (num3 != 0)
      {
        if (index1 >= 0)
        {
          lowIsValid = true;
          low = this.array[index1];
        }
        if (index2 < this.size)
        {
          highIsValid = true;
          high = this.array[index2];
        }
        return false;
      }
      int num4 = index3;
      int num5 = num4 - 1;
      while (index2 > num5)
      {
        int index4 = (num5 + index2) / 2;
        if (c.CompareTo(this.array[index4]) < 0)
          index2 = index4;
        else
          num5 = index4 + 1;
      }
      if (index2 < this.size)
      {
        highIsValid = true;
        high = this.array[index2];
      }
      int num6 = num4 + 1;
      while (num6 > index1)
      {
        int index5 = (index1 + num6 + 1) / 2;
        if (c.CompareTo(this.array[index5]) > 0)
          index1 = index5;
        else
          num6 = index5 - 1;
      }
      if (index1 >= 0)
      {
        lowIsValid = true;
        low = this.array[index1];
      }
      return true;
    }

    IDirectedEnumerable<T> ISorted<T>.RangeFrom(T bot)
    {
      return (IDirectedEnumerable<T>) this.RangeFrom(bot);
    }

    IDirectedEnumerable<T> ISorted<T>.RangeFromTo(T bot, T top)
    {
      return (IDirectedEnumerable<T>) this.RangeFromTo(bot, top);
    }

    IDirectedEnumerable<T> ISorted<T>.RangeTo(T top) => (IDirectedEnumerable<T>) this.RangeTo(top);

    public IDirectedCollectionValue<T> RangeAll()
    {
      return (IDirectedCollectionValue<T>) new ArrayBase<T>.Range((ArrayBase<T>) this, 0, this.size, true);
    }

    public void AddSorted(IEnumerable<T> items)
    {
      this.updatecheck();
      int num1 = 0;
      int index1 = 0;
      int num2 = -1;
      int length = EnumerableBase<T>.countItems(items);
      int numAdded = 0;
      SortedArray<T> sortedArray = new SortedArray<T>(this.size + length, this._comparer);
      T x = default (T);
      T[] addedItems = new T[length];
      foreach (T y in items)
      {
        int num3;
        while (index1 < this.size && (num3 = this._comparer.Compare(this.array[index1], y)) <= 0)
        {
          T[] array1 = sortedArray.array;
          int index2 = num1++;
          T[] array2 = this.array;
          int index3 = index1++;
          T obj1;
          T obj2 = obj1 = array2[index3];
          array1[index2] = obj1;
          x = obj2;
          if (num3 == 0)
            goto label_8;
        }
        if (num1 > 0 && this._comparer.Compare(x, y) >= 0)
          throw new ArgumentException("Argument not sorted");
        T[] objArray = addedItems;
        int index4 = numAdded++;
        T obj3;
        sortedArray.array[num1++] = obj3 = y;
        T obj4;
        x = obj4 = obj3;
        objArray[index4] = obj4;
label_8:
        num2 = -1;
      }
      while (index1 < this.size)
        sortedArray.array[num1++] = this.array[index1++];
      this.size = num1;
      this.array = sortedArray.array;
      this.raiseForAddAll(addedItems, numAdded);
    }

    public void RemoveRangeFrom(T low)
    {
      int middle;
      this.BinarySearch(low, out middle);
      if (middle == this.size)
        return;
      T[] objArray = new T[this.size - middle];
      Array.Copy((Array) this.array, middle, (Array) objArray, 0, objArray.Length);
      Array.Reverse((Array) objArray);
      Array.Clear((Array) this.array, middle, this.size - middle);
      this.size = middle;
      this.raiseForRemoveRange(objArray);
    }

    public void RemoveRangeFromTo(T low, T hi)
    {
      int middle1;
      this.BinarySearch(low, out middle1);
      int middle2;
      this.BinarySearch(hi, out middle2);
      if (middle2 <= middle1)
        return;
      T[] objArray = new T[middle2 - middle1];
      Array.Copy((Array) this.array, middle1, (Array) objArray, 0, objArray.Length);
      Array.Reverse((Array) objArray);
      Array.Copy((Array) this.array, middle2, (Array) this.array, middle1, this.size - middle2);
      Array.Clear((Array) this.array, this.size - middle2 + middle1, middle2 - middle1);
      this.size -= middle2 - middle1;
      this.raiseForRemoveRange(objArray);
    }

    public void RemoveRangeTo(T hi)
    {
      int middle;
      this.BinarySearch(hi, out middle);
      if (middle == 0)
        return;
      T[] objArray = new T[middle];
      Array.Copy((Array) this.array, 0, (Array) objArray, 0, objArray.Length);
      Array.Copy((Array) this.array, middle, (Array) this.array, 0, this.size - middle);
      Array.Clear((Array) this.array, this.size - middle, middle);
      this.size -= middle;
      this.raiseForRemoveRange(objArray);
    }

    private void raiseForRemoveRange(T[] removed)
    {
      foreach (T obj in removed)
        this.raiseItemsRemoved(obj, 1);
      if (removed.Length <= 0)
        return;
      this.raiseCollectionChanged();
    }

    public Speed ContainsSpeed => Speed.Log;

    public override void Clear()
    {
      int size = this.size;
      base.Clear();
      if (size <= 0)
        return;
      this.raiseCollectionCleared(true, size);
      this.raiseCollectionChanged();
    }

    public bool Contains(T item) => this.BinarySearch(item, out int _);

    public bool Find(ref T item)
    {
      int middle;
      if (!this.BinarySearch(item, out middle))
        return false;
      item = this.array[middle];
      return true;
    }

    public bool FindOrAdd(ref T item)
    {
      this.updatecheck();
      int middle;
      if (this.BinarySearch(item, out middle))
      {
        item = this.array[middle];
        return true;
      }
      if (this.size == this.array.Length)
        this.expand();
      Array.Copy((Array) this.array, middle, (Array) this.array, middle + 1, this.size - middle);
      this.array[middle] = item;
      ++this.size;
      this.raiseForAdd(item);
      return false;
    }

    public bool Update(T item) => this.Update(item, out T _);

    public bool Update(T item, out T olditem)
    {
      this.updatecheck();
      int middle;
      if (this.BinarySearch(item, out middle))
      {
        olditem = this.array[middle];
        this.array[middle] = item;
        this.raiseForUpdate(item, olditem);
        return true;
      }
      olditem = default (T);
      return false;
    }

    public bool UpdateOrAdd(T item) => this.UpdateOrAdd(item, out T _);

    public bool UpdateOrAdd(T item, out T olditem)
    {
      this.updatecheck();
      int middle;
      if (this.BinarySearch(item, out middle))
      {
        olditem = this.array[middle];
        this.array[middle] = item;
        this.raiseForUpdate(item, olditem);
        return true;
      }
      if (this.size == this.array.Length)
        this.expand();
      Array.Copy((Array) this.array, middle, (Array) this.array, middle + 1, this.size - middle);
      this.array[middle] = item;
      ++this.size;
      olditem = default (T);
      this.raiseForAdd(item);
      return false;
    }

    public bool Remove(T item)
    {
      this.updatecheck();
      int middle;
      if (!this.BinarySearch(item, out middle))
        return false;
      T obj = this.array[middle];
      Array.Copy((Array) this.array, middle + 1, (Array) this.array, middle, this.size - middle - 1);
      this.array[--this.size] = default (T);
      this.raiseForRemove(obj);
      return true;
    }

    public bool Remove(T item, out T removeditem)
    {
      this.updatecheck();
      int middle;
      if (this.BinarySearch(item, out middle))
      {
        removeditem = this.array[middle];
        Array.Copy((Array) this.array, middle + 1, (Array) this.array, middle, this.size - middle - 1);
        this.array[--this.size] = default (T);
        this.raiseForRemove(removeditem);
        return true;
      }
      removeditem = default (T);
      return false;
    }

    public void RemoveAll(IEnumerable<T> items)
    {
      this.updatecheck();
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) this);
      bool mustFire = removeAllHandler.MustFire;
      int[] numArray = new int[(this.size >> 5) + 1];
      int index1 = 0;
      foreach (T obj in items)
      {
        int middle;
        if (this.BinarySearch(obj, out middle))
          numArray[middle >> 5] |= 1 << middle;
      }
      for (int index2 = 0; index2 < this.size; ++index2)
      {
        if ((numArray[index2 >> 5] & 1 << index2) == 0)
          this.array[index1++] = this.array[index2];
        else if (mustFire)
          removeAllHandler.Remove(this.array[index2]);
      }
      Array.Clear((Array) this.array, index1, this.size - index1);
      this.size = index1;
      if (!mustFire)
        return;
      removeAllHandler.Raise();
    }

    public void RetainAll(IEnumerable<T> items)
    {
      this.updatecheck();
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) this);
      bool mustFire = removeAllHandler.MustFire;
      int[] numArray = new int[(this.size >> 5) + 1];
      int index1 = 0;
      foreach (T obj in items)
      {
        int middle;
        if (this.BinarySearch(obj, out middle))
          numArray[middle >> 5] |= 1 << middle;
      }
      for (int index2 = 0; index2 < this.size; ++index2)
      {
        if ((numArray[index2 >> 5] & 1 << index2) != 0)
          this.array[index1++] = this.array[index2];
        else if (mustFire)
          removeAllHandler.Remove(this.array[index2]);
      }
      Array.Clear((Array) this.array, index1, this.size - index1);
      this.size = index1;
      if (!mustFire)
        return;
      removeAllHandler.Raise();
    }

    public bool ContainsAll(IEnumerable<T> items)
    {
      foreach (T obj in items)
      {
        if (!this.BinarySearch(obj, out int _))
          return false;
      }
      return true;
    }

    public int ContainsCount(T item) => !this.BinarySearch(item, out int _) ? 0 : 1;

    public virtual ICollectionValue<T> UniqueItems() => (ICollectionValue<T>) this;

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      return (ICollectionValue<KeyValuePair<T, int>>) new MultiplicityOne<T>((ICollectionValue<T>) this);
    }

    public void RemoveAllCopies(T item) => this.Remove(item);

    public override bool Check()
    {
      bool flag = true;
      if (this.size > this.array.Length)
      {
        Logger.Log(string.Format("Bad size ({0}) > array.Length ({1})", (object) this.size, (object) this.array.Length));
        return false;
      }
      for (int index = 0; index < this.size; ++index)
      {
        if ((object) this.array[index] == null)
        {
          Logger.Log(string.Format("Bad element: null at index {0}", (object) index));
          return false;
        }
        if (index > 0 && this._comparer.Compare(this.array[index], this.array[index - 1]) <= 0)
        {
          Logger.Log(string.Format("Inversion at index {0}", (object) index));
          flag = false;
        }
      }
      return flag;
    }

    public bool AllowsDuplicates => false;

    public virtual bool DuplicatesByCounting => true;

    public bool Add(T item)
    {
      this.updatecheck();
      int middle;
      if (this.BinarySearch(item, out middle))
        return false;
      this.InsertProtected(middle, item);
      this.raiseForAdd(item);
      return true;
    }

    void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

    public void AddAll(IEnumerable<T> items)
    {
      int num = EnumerableBase<T>.countItems(items);
      int length = this.array.Length;
      while (length < this.size + num)
        length *= 2;
      T[] array1 = new T[length];
      int count = 0;
      foreach (T obj in items)
        array1[this.size + count++] = obj;
      Sorting.IntroSort<T>(array1, this.size, count, this._comparer);
      int index1 = 0;
      int index2 = 0;
      int numAdded = 0;
      T x = default (T);
      T[] addedItems = new T[count];
      int size = this.size;
      for (int index3 = this.size + count; size < index3; ++size)
      {
        while (index2 < this.size && this._comparer.Compare(this.array[index2], array1[size]) <= 0)
        {
          T[] objArray = array1;
          int index4 = index1++;
          T[] array2 = this.array;
          int index5 = index2++;
          T obj1;
          T obj2 = obj1 = array2[index5];
          objArray[index4] = obj1;
          x = obj2;
        }
        if (index1 == 0 || this._comparer.Compare(x, array1[size]) < 0)
        {
          T[] objArray = addedItems;
          int index6 = numAdded++;
          T obj3;
          array1[index1++] = obj3 = array1[size];
          T obj4;
          x = obj4 = obj3;
          objArray[index6] = obj4;
        }
      }
      while (index2 < this.size)
        array1[index1++] = this.array[index2++];
      Array.Clear((Array) array1, index1, this.size + count - index1);
      this.size = index1;
      this.array = array1;
      this.raiseForAddAll(addedItems, numAdded);
    }

    private void raiseForAddAll(T[] addedItems, int numAdded)
    {
      if ((this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None)
      {
        for (int index = 0; index < numAdded; ++index)
          this.raiseItemsAdded(addedItems[index], 1);
      }
      if (numAdded <= 0)
        return;
      this.raiseCollectionChanged();
    }

    public T FindMin()
    {
      if (this.size == 0)
        throw new NoSuchItemException();
      return this.array[0];
    }

    public T DeleteMin()
    {
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException();
      T obj = this.array[0];
      --this.size;
      Array.Copy((Array) this.array, 1, (Array) this.array, 0, this.size);
      this.array[this.size] = default (T);
      this.raiseForRemove(obj);
      return obj;
    }

    public T FindMax()
    {
      return this.size != 0 ? this.array[this.size - 1] : throw new NoSuchItemException();
    }

    public T DeleteMax()
    {
      this.updatecheck();
      T obj = this.size != 0 ? this.array[this.size - 1] : throw new NoSuchItemException();
      --this.size;
      this.array[this.size] = default (T);
      this.raiseForRemove(obj);
      return obj;
    }

    public IComparer<T> Comparer => this._comparer;

    public T this[int i]
    {
      get => i >= 0 && i < this.size ? this.array[i] : throw new IndexOutOfRangeException();
    }

    public virtual Speed IndexingSpeed => Speed.Constant;

    public int IndexOf(T item) => this.indexOf(item);

    public int LastIndexOf(T item) => this.indexOf(item);

    public T RemoveAt(int i)
    {
      if (i < 0 || i >= this.size)
        throw new IndexOutOfRangeException("Index out of range for sequenced collectionvalue");
      this.updatecheck();
      T obj = this.array[i];
      --this.size;
      Array.Copy((Array) this.array, i + 1, (Array) this.array, i, this.size - i);
      this.array[this.size] = default (T);
      this.raiseForRemoveAt(i, obj);
      return obj;
    }

    public void RemoveInterval(int start, int count)
    {
      this.updatecheck();
      this.checkRange(start, count);
      Array.Copy((Array) this.array, start + count, (Array) this.array, start, this.size - start - count);
      this.size -= count;
      Array.Clear((Array) this.array, this.size, count);
      this.raiseForRemoveInterval(start, count);
    }

    private void raiseForRemoveInterval(int start, int count)
    {
      if (this.ActiveEvents == EventTypeEnum.None || count <= 0)
        return;
      this.raiseCollectionCleared(this.size == 0, count);
      this.raiseCollectionChanged();
    }

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }
  }
}
