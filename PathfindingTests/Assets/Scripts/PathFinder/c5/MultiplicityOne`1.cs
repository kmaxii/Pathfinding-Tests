// Decompiled with JetBrains decompiler
// Type: C5.MultiplicityOne`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  internal class MultiplicityOne<K> : MappedCollectionValue<K, KeyValuePair<K, int>>
  {
    public MultiplicityOne(ICollectionValue<K> coll)
      : base(coll)
    {
    }

    public override KeyValuePair<K, int> Map(K k) => new KeyValuePair<K, int>(k, 1);
  }
}
