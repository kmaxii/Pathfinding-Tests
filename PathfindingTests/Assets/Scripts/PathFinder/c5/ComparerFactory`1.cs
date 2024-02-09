// Decompiled with JetBrains decompiler
// Type: C5.ComparerFactory`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public class ComparerFactory<T>
  {
    public static IComparer<T> CreateComparer(Func<T, T, int> comparer)
    {
      return (IComparer<T>) new InternalComparer<T>(comparer);
    }

    public static IEqualityComparer<T> CreateEqualityComparer(
      Func<T, T, bool> equals,
      Func<T, int> getHashCode)
    {
      return (IEqualityComparer<T>) new InternalEqualityComparer<T>(equals, getHashCode);
    }
  }
}
