// Decompiled with JetBrains decompiler
// Type: C5.KeyValuePairComparer`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public class KeyValuePairComparer<K, V> : IComparer<KeyValuePair<K, V>>
  {
    private IComparer<K> comparer;

    public KeyValuePairComparer(IComparer<K> comparer)
    {
      this.comparer = comparer != null ? comparer : throw new NullReferenceException();
    }

    public int Compare(KeyValuePair<K, V> entry1, KeyValuePair<K, V> entry2)
    {
      return this.comparer.Compare(entry1.Key, entry2.Key);
    }
  }
}
