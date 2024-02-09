// Decompiled with JetBrains decompiler
// Type: C5.EventTypeEnum
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Flags]
  public enum EventTypeEnum
  {
    None = 0,
    Changed = 1,
    Cleared = 2,
    Added = 4,
    Removed = 8,
    Basic = Removed | Added | Cleared | Changed, // 0x0000000F
    Inserted = 16, // 0x00000010
    RemovedAt = 32, // 0x00000020
    All = RemovedAt | Inserted | Basic, // 0x0000003F
  }
}
