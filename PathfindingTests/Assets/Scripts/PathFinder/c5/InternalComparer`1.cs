// Decompiled with JetBrains decompiler
// Type: C5.InternalComparer`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  internal class InternalComparer<T> : IComparer<T>
  {
    private readonly Func<T, T, int> _compare;

    public InternalComparer(Func<T, T, int> compare) => this._compare = compare;

    public int Compare(T x, T y) => this._compare(x, y);
  }
}
