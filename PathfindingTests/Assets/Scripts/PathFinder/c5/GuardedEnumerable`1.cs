﻿// Decompiled with JetBrains decompiler
// Type: C5.GuardedEnumerable`1
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
  public class GuardedEnumerable<T> : IEnumerable<T>, IEnumerable
  {
    private IEnumerable<T> enumerable;

    public GuardedEnumerable(IEnumerable<T> enumerable) => this.enumerable = enumerable;

    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) new GuardedEnumerator<T>(this.enumerable.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}