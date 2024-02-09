// Decompiled with JetBrains decompiler
// Type: C5.DropMultiplicity`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  internal class DropMultiplicity<K> : MappedCollectionValue<KeyValuePair<K, int>, K>
  {
    public DropMultiplicity(ICollectionValue<KeyValuePair<K, int>> coll)
      : base(coll)
    {
    }

    public override K Map(KeyValuePair<K, int> kvp) => kvp.Key;
  }
}
