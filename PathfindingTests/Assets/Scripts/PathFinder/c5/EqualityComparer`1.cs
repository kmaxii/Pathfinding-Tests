// Decompiled with JetBrains decompiler
// Type: C5.EqualityComparer`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace C5
{
  [Serializable]
  public static class EqualityComparer<T>
  {
    private static IEqualityComparer<T> _default;
    private static readonly Type SequencedCollectionEqualityComparer = typeof (C5.SequencedCollectionEqualityComparer<,>);
    private static readonly Type UnsequencedCollectionEqualityComparer = typeof (C5.UnsequencedCollectionEqualityComparer<,>);

    public static IEqualityComparer<T> Default
    {
      get
      {
        if (EqualityComparer<T>._default != null)
          return EqualityComparer<T>._default;
        Type type1 = typeof (T);
        Type[] interfaces = type1.GetInterfaces();
        if (type1.IsGenericType && type1.GetGenericTypeDefinition().Equals(typeof (ISequenced<>)))
          return EqualityComparer<T>.CreateAndCache(EqualityComparer<T>.SequencedCollectionEqualityComparer.MakeGenericType(type1, type1.GetGenericArguments()[0]));
        Type type2 = ((IEnumerable<Type>) interfaces).FirstOrDefault<Type>((Func<Type, bool>) (i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof (ISequenced<>))));
        if (type2 != null)
          return EqualityComparer<T>.CreateAndCache(EqualityComparer<T>.SequencedCollectionEqualityComparer.MakeGenericType(type1, type2.GetGenericArguments()[0]));
        if (type1.IsGenericType && type1.GetGenericTypeDefinition().Equals(typeof (ICollection<>)))
          return EqualityComparer<T>.CreateAndCache(EqualityComparer<T>.UnsequencedCollectionEqualityComparer.MakeGenericType(type1, type1.GetGenericArguments()[0]));
        Type type3 = ((IEnumerable<Type>) interfaces).FirstOrDefault<Type>((Func<Type, bool>) (i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof (ICollection<>))));
        if (type3 == null)
          return EqualityComparer<T>._default = (IEqualityComparer<T>) System.Collections.Generic.EqualityComparer<T>.Default;
        return EqualityComparer<T>.CreateAndCache(EqualityComparer<T>.UnsequencedCollectionEqualityComparer.MakeGenericType(type1, type3.GetGenericArguments()[0]));
      }
    }

    private static IEqualityComparer<T> CreateAndCache(Type equalityComparertype)
    {
      return EqualityComparer<T>._default = (IEqualityComparer<T>) equalityComparertype.GetProperty("Default", BindingFlags.Static | BindingFlags.Public).GetValue((object) null, (object[]) null);
    }
  }
}
