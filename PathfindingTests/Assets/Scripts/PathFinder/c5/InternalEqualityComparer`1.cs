// Decompiled with JetBrains decompiler
// Type: C5.InternalEqualityComparer`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  internal class InternalEqualityComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, T, bool> _equals;
    private readonly Func<T, int> _getHashCode;

    public InternalEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
    {
      this._equals = equals;
      this._getHashCode = getHashCode;
    }

    public bool Equals(T x, T y) => this._equals(x, y);

    public int GetHashCode(T obj) => this._getHashCode(obj);
  }
}
