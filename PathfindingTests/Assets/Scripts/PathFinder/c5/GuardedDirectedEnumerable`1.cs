// Decompiled with JetBrains decompiler
// Type: C5.GuardedDirectedEnumerable`1
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
  public class GuardedDirectedEnumerable<T> : 
    GuardedEnumerable<T>,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable
  {
    private IDirectedEnumerable<T> directedenumerable;

    public GuardedDirectedEnumerable(IDirectedEnumerable<T> directedenumerable)
      : base((IEnumerable<T>) directedenumerable)
    {
      this.directedenumerable = directedenumerable;
    }

    public IDirectedEnumerable<T> Backwards()
    {
      return (IDirectedEnumerable<T>) new GuardedDirectedEnumerable<T>(this.directedenumerable.Backwards());
    }

    public EnumerationDirection Direction => this.directedenumerable.Direction;
  }
}
