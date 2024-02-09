// Decompiled with JetBrains decompiler
// Type: C5.SequencedCollectionEqualityComparer`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public class SequencedCollectionEqualityComparer<T, W> : IEqualityComparer<T> where T : ISequenced<W>
  {
    private static SequencedCollectionEqualityComparer<T, W> cached;

    private SequencedCollectionEqualityComparer()
    {
    }

    public static SequencedCollectionEqualityComparer<T, W> Default
    {
      get
      {
        return SequencedCollectionEqualityComparer<T, W>.cached ?? (SequencedCollectionEqualityComparer<T, W>.cached = new SequencedCollectionEqualityComparer<T, W>());
      }
    }

    public int GetHashCode(T collection) => collection.GetSequencedHashCode();

    public bool Equals(T collection1, T collection2)
    {
      return (object) collection1 != null ? collection1.SequencedEquals((ISequenced<W>) collection2) : (object) collection2 == null;
    }
  }
}
