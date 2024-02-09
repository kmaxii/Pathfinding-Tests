// Decompiled with JetBrains decompiler
// Type: C5.ComparerZeroHashCodeEqualityComparer`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  internal class ComparerZeroHashCodeEqualityComparer<T> : IEqualityComparer<T>
  {
    private IComparer<T> comparer;

    public ComparerZeroHashCodeEqualityComparer(IComparer<T> comparer)
    {
      this.comparer = comparer != null ? comparer : throw new NullReferenceException("Comparer cannot be null");
    }

    public int GetHashCode(T item) => 0;

    public bool Equals(T item1, T item2) => this.comparer.Compare(item1, item2) == 0;
  }
}
