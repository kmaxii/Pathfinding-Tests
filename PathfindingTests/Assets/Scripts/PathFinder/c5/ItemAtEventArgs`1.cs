// Decompiled with JetBrains decompiler
// Type: C5.ItemAtEventArgs`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  public class ItemAtEventArgs<T> : EventArgs
  {
    public readonly T Item;
    public readonly int Index;

    public ItemAtEventArgs(T item, int index)
    {
      this.Item = item;
      this.Index = index;
    }

    public override string ToString()
    {
      return string.Format("(ItemAtEventArgs {0} '{1}')", (object) this.Index, (object) this.Item);
    }
  }
}
