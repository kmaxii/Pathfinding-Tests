// Decompiled with JetBrains decompiler
// Type: C5.MemorySafeEnumerator`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace C5
{
  [Serializable]
  internal abstract class MemorySafeEnumerator<T> : 
    IEnumerator<T>,
    IEnumerator,
    IEnumerable<T>,
    IEnumerable,
    IDisposable
  {
    private static int MainThreadId;
    protected int IteratorState;

    protected MemoryType MemoryType { get; private set; }

    protected static bool IsMainThread
    {
      get => Thread.CurrentThread.ManagedThreadId == MemorySafeEnumerator<T>.MainThreadId;
    }

    protected MemorySafeEnumerator(MemoryType memoryType)
    {
      this.MemoryType = memoryType;
      MemorySafeEnumerator<T>.MainThreadId = Thread.CurrentThread.ManagedThreadId;
      this.IteratorState = -1;
    }

    protected abstract MemorySafeEnumerator<T> Clone();

    public abstract bool MoveNext();

    public abstract void Reset();

    public T Current { get; protected set; }

    object IEnumerator.Current => (object) this.Current;

    public virtual void Dispose() => this.IteratorState = -1;

    public IEnumerator<T> GetEnumerator()
    {
      MemorySafeEnumerator<T> enumerator;
      switch (this.MemoryType)
      {
        case MemoryType.Normal:
          enumerator = this.Clone();
          break;
        case MemoryType.Safe:
          if (MemorySafeEnumerator<T>.IsMainThread)
          {
            enumerator = this.IteratorState != -1 ? this.Clone() : this;
            this.IteratorState = 0;
            break;
          }
          enumerator = this.Clone();
          break;
        case MemoryType.Strict:
          if (!MemorySafeEnumerator<T>.IsMainThread)
            throw new ConcurrentEnumerationException("Multithread access detected! In Strict memory mode is not possible to iterate the collection from different threads");
          if (this.IteratorState != -1)
            throw new MultipleEnumerationException("Multiple Enumeration detected! In Strict memory mode is not possible to iterate the collection multiple times");
          enumerator = this;
          this.IteratorState = 0;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      return (IEnumerator<T>) enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
