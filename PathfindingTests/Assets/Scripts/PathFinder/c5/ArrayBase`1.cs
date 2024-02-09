// Decompiled with JetBrains decompiler
// Type: C5.ArrayBase`1
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
  public abstract class ArrayBase<T> : SequencedBase<T>
  {
    public T[] array;
    protected int offsetField;
    private readonly ArrayBase<T>.Enumerator _internalEnumerator;

    protected virtual void expand() => this.expand(2 * this.array.Length, this.size);

    protected virtual void expand(int newcapacity, int newsize)
    {
      int length = this.array.Length;
      while (length < newcapacity)
        length *= 2;
      T[] destinationArray = new T[length];
      Array.Copy((Array) this.array, (Array) destinationArray, newsize);
      this.array = destinationArray;
    }

    protected virtual void InsertProtected(int i, T item)
    {
      if (this.size == this.array.Length)
        this.expand();
      if (i < this.size)
        Array.Copy((Array) this.array, i, (Array) this.array, i + 1, this.size - i);
      this.array[i] = item;
      ++this.size;
    }

    protected ArrayBase(
      int capacity,
      IEqualityComparer<T> itemequalityComparer,
      MemoryType memoryType)
      : base(itemequalityComparer, memoryType)
    {
      int length = 8;
      while (length < capacity)
        length *= 2;
      this.array = new T[length];
      this._internalEnumerator = new ArrayBase<T>.Enumerator(this, memoryType);
    }

    public virtual IDirectedCollectionValue<T> this[int start, int count]
    {
      get
      {
        this.checkRange(start, count);
        return (IDirectedCollectionValue<T>) new ArrayBase<T>.Range(this, start, count, true);
      }
    }

    public virtual void Clear()
    {
      this.updatecheck();
      this.array = new T[8];
      this.size = 0;
    }

    public override T[] ToArray()
    {
      T[] destinationArray = new T[this.size];
      Array.Copy((Array) this.array, this.offsetField, (Array) destinationArray, 0, this.size);
      return destinationArray;
    }

    public virtual bool Check()
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
      }
      return flag;
    }

    public override IDirectedCollectionValue<T> Backwards() => this[0, this.size].Backwards();

    public override T Choose()
    {
      return this.size > 0 ? this.array[this.size - 1] : throw new NoSuchItemException();
    }

    public override IEnumerator<T> GetEnumerator()
    {
      int stamp = this.stamp;
      int end = this.size + this.offsetField;
      int offsetField = this.offsetField;
      ArrayBase<T>.Enumerator enumerator = (ArrayBase<T>.Enumerator) this._internalEnumerator.GetEnumerator();
      enumerator.UpdateReference(this, offsetField, end, stamp);
      return (IEnumerator<T>) enumerator;
    }

    [Serializable]
    private class Enumerator : MemorySafeEnumerator<T>
    {
      private ArrayBase<T> _internalList;
      private int _internalIncrementalIndex;
      private int _theStamp;
      private int _end;

      public Enumerator(ArrayBase<T> list, MemoryType memoryType)
        : base(memoryType)
      {
        this._internalList = list;
      }

      internal void UpdateReference(ArrayBase<T> list, int start, int end, int theStamp)
      {
        this._internalIncrementalIndex = start;
        this._end = end;
        this._internalList = list;
        this.Current = default (T);
        this._theStamp = theStamp;
      }

      public override bool MoveNext()
      {
        ArrayBase<T> internalList = this._internalList;
        if (internalList.stamp != this._theStamp)
          throw new CollectionModifiedException();
        if (this._internalIncrementalIndex < this._end)
        {
          this.Current = internalList.array[this._internalIncrementalIndex];
          ++this._internalIncrementalIndex;
          return true;
        }
        this.Current = default (T);
        return false;
      }

      public override void Reset()
      {
        this._internalIncrementalIndex = 0;
        this.Current = default (T);
      }

      protected override MemorySafeEnumerator<T> Clone()
      {
        ArrayBase<T>.Enumerator enumerator = new ArrayBase<T>.Enumerator(this._internalList, this.MemoryType);
        enumerator.Current = default (T);
        return (MemorySafeEnumerator<T>) enumerator;
      }
    }

    [Serializable]
    protected class Range : 
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
      private int delta;
      private int stamp;
      private ArrayBase<T> thebase;
      private readonly ArrayBase<T>.Range.RangeEnumerator _rangeInternalEnumerator;

      internal Range(
        ArrayBase<T> thebase,
        int start,
        int count,
        bool forwards,
        MemoryType memoryType = MemoryType.Normal)
      {
        this.thebase = thebase;
        this.stamp = thebase.stamp;
        this.delta = forwards ? 1 : -1;
        this.start = start + thebase.offsetField;
        this.count = count;
        this._rangeInternalEnumerator = new ArrayBase<T>.Range.RangeEnumerator(thebase, memoryType);
      }

      public override bool IsEmpty
      {
        get
        {
          this.thebase.modifycheck(this.stamp);
          return this.count == 0;
        }
      }

      public override int Count
      {
        get
        {
          this.thebase.modifycheck(this.stamp);
          return this.count;
        }
      }

      public override Speed CountSpeed
      {
        get
        {
          this.thebase.modifycheck(this.stamp);
          return Speed.Constant;
        }
      }

      public override T Choose()
      {
        this.thebase.modifycheck(this.stamp);
        if (this.count == 0)
          throw new NoSuchItemException();
        return this.thebase.array[this.start];
      }

      public override IEnumerator<T> GetEnumerator()
      {
        ArrayBase<T>.Range.RangeEnumerator enumerator = (ArrayBase<T>.Range.RangeEnumerator) this._rangeInternalEnumerator.GetEnumerator();
        enumerator.UpdateReference(this.thebase, this.start, this.delta, this.stamp, this.count);
        return (IEnumerator<T>) enumerator;
      }

      public override IDirectedCollectionValue<T> Backwards()
      {
        this.thebase.modifycheck(this.stamp);
        ArrayBase<T>.Range range = (ArrayBase<T>.Range) this.MemberwiseClone();
        range.delta = -this.delta;
        range.start = this.start + (this.count - 1) * this.delta;
        return (IDirectedCollectionValue<T>) range;
      }

      IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
      {
        return (IDirectedEnumerable<T>) this.Backwards();
      }

      public override EnumerationDirection Direction
      {
        get
        {
          this.thebase.modifycheck(this.stamp);
          return this.delta <= 0 ? EnumerationDirection.Backwards : EnumerationDirection.Forwards;
        }
      }

      private sealed class RangeEnumerator : MemorySafeEnumerator<T>
      {
        private ArrayBase<T> _rangeEnumeratorArrayBase;
        private int _start;
        private int _count;
        private int _theStamp;
        private int _delta;
        private int _index;

        public RangeEnumerator(ArrayBase<T> internalList, MemoryType memoryType)
          : base(memoryType)
        {
          this._rangeEnumeratorArrayBase = internalList;
          this.IteratorState = -1;
          this._index = 0;
        }

        internal void UpdateReference(
          ArrayBase<T> list,
          int start,
          int delta,
          int theStamp,
          int count)
        {
          this._count = count;
          this._start = start;
          this._delta = delta;
          this._rangeEnumeratorArrayBase = list;
          this.Current = default (T);
          this._theStamp = theStamp;
        }

        protected override MemorySafeEnumerator<T> Clone()
        {
          ArrayBase<T>.Range.RangeEnumerator rangeEnumerator = new ArrayBase<T>.Range.RangeEnumerator(this._rangeEnumeratorArrayBase, this.MemoryType);
          rangeEnumerator.Current = default (T);
          return (MemorySafeEnumerator<T>) rangeEnumerator;
        }

        public override bool MoveNext()
        {
          ArrayBase<T> enumeratorArrayBase = this._rangeEnumeratorArrayBase;
          enumeratorArrayBase.modifycheck(this._theStamp);
          if (this._index < this._count)
          {
            this.Current = enumeratorArrayBase.array[this._start + this._delta * this._index];
            ++this._index;
            return true;
          }
          this.Current = default (T);
          return false;
        }

        public override void Reset()
        {
          this._index = 0;
          this.Current = default (T);
        }
      }
    }
  }
}
