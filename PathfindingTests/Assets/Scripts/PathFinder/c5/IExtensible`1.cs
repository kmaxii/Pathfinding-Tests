// Decompiled with JetBrains decompiler
// Type: C5.IExtensible`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  public interface IExtensible<T> : 
    ICollectionValue<T>,
    IEnumerable<T>,
    IEnumerable,
    IShowable,
    IFormattable
  {
    bool IsReadOnly { get; }

    bool AllowsDuplicates { get; }

    IEqualityComparer<T> EqualityComparer { get; }

    bool DuplicatesByCounting { get; }

    bool Add(T item);

    void AddAll(IEnumerable<T> items);

    bool Check();
  }
}
