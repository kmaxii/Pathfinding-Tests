// Decompiled with JetBrains decompiler
// Type: C5.UnsequencedCollectionEqualityComparer`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public class UnsequencedCollectionEqualityComparer<T, W> : IEqualityComparer<T> where T : ICollection<W>
  {
    private static UnsequencedCollectionEqualityComparer<T, W> cached;

    private UnsequencedCollectionEqualityComparer()
    {
    }

    public static UnsequencedCollectionEqualityComparer<T, W> Default
    {
      get
      {
        return UnsequencedCollectionEqualityComparer<T, W>.cached ?? (UnsequencedCollectionEqualityComparer<T, W>.cached = new UnsequencedCollectionEqualityComparer<T, W>());
      }
    }

    public int GetHashCode(T collection) => collection.GetUnsequencedHashCode();

    public bool Equals(T collection1, T collection2)
    {
      return (object) collection1 != null ? collection1.UnsequencedEquals((ICollection<W>) collection2) : (object) collection2 == null;
    }
  }
}
