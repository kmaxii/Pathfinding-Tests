// Decompiled with JetBrains decompiler
// Type: C5.ClearedRangeEventArgs
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  public class ClearedRangeEventArgs : ClearedEventArgs
  {
    public readonly int? Start;

    public ClearedRangeEventArgs(bool full, int count, int? start)
      : base(full, count)
    {
      this.Start = start;
    }

    public override string ToString()
    {
      return string.Format("(ClearedRangeEventArgs {0} {1} {2})", (object) this.Count, (object) this.Full, (object) this.Start);
    }
  }
}
