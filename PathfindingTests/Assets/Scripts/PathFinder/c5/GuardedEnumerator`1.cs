// Decompiled with JetBrains decompiler
// Type: C5.GuardedEnumerator`1
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
  public class GuardedEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
  {
    private IEnumerator<T> enumerator;

    public GuardedEnumerator(IEnumerator<T> enumerator) => this.enumerator = enumerator;

    public bool MoveNext() => this.enumerator.MoveNext();

    public T Current => this.enumerator.Current;

    public void Dispose() => this.enumerator.Dispose();

    object IEnumerator.Current => (object) this.enumerator.Current;

    void IEnumerator.Reset() => this.enumerator.Reset();
  }
}
