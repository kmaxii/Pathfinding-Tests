// Decompiled with JetBrains decompiler
// Type: C5.ItemCountEventArgs`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  public class ItemCountEventArgs<T> : EventArgs
  {
    public readonly T Item;
    public readonly int Count;

    public ItemCountEventArgs(T item, int count)
    {
      this.Item = item;
      this.Count = count;
    }

    public override string ToString()
    {
      return string.Format("(ItemCountEventArgs {0} '{1}')", (object) this.Count, (object) this.Item);
    }
  }
}
