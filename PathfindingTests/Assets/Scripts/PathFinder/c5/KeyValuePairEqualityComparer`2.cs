// Decompiled with JetBrains decompiler
// Type: C5.KeyValuePairEqualityComparer`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public sealed class KeyValuePairEqualityComparer<K, V> : IEqualityComparer<KeyValuePair<K, V>>
  {
    private IEqualityComparer<K> keyequalityComparer;

    public KeyValuePairEqualityComparer() => this.keyequalityComparer = EqualityComparer<K>.Default;

    public KeyValuePairEqualityComparer(IEqualityComparer<K> keyequalityComparer)
    {
      this.keyequalityComparer = keyequalityComparer != null ? keyequalityComparer : throw new NullReferenceException("Key equality comparer cannot be null");
    }

    public int GetHashCode(KeyValuePair<K, V> entry)
    {
      return this.keyequalityComparer.GetHashCode(entry.Key);
    }

    public bool Equals(KeyValuePair<K, V> entry1, KeyValuePair<K, V> entry2)
    {
      return this.keyequalityComparer.Equals(entry1.Key, entry2.Key);
    }
  }
}
