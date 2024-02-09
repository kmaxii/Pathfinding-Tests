// Decompiled with JetBrains decompiler
// Type: C5.Rec`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Text;

#nullable disable
namespace C5
{
  public struct Rec<T1, T2> : IEquatable<Rec<T1, T2>>, IShowable, IFormattable
  {
    public readonly T1 X1;
    public readonly T2 X2;

    public Rec(T1 x1, T2 x2)
    {
      this.X1 = x1;
      this.X2 = x2;
    }

    public bool Equals(Rec<T1, T2> other)
    {
      if (((object) this.X1 == null ? ((object) other.X1 == null ? 1 : 0) : (this.X1.Equals((object) other.X1) ? 1 : 0)) == 0)
        return false;
      return (object) this.X2 != null ? this.X2.Equals((object) other.X2) : (object) other.X2 == null;
    }

    public override bool Equals(object obj) => obj is Rec<T1, T2> other && this.Equals(other);

    public static bool operator ==(Rec<T1, T2> record1, Rec<T1, T2> record2)
    {
      return record1.Equals(record2);
    }

    public static bool operator !=(Rec<T1, T2> record1, Rec<T1, T2> record2)
    {
      return !record1.Equals(record2);
    }

    public override int GetHashCode()
    {
      return ((object) this.X1 == null ? 0 : this.X1.GetHashCode()) * 387281 + ((object) this.X2 == null ? 0 : this.X2.GetHashCode());
    }

    public override string ToString()
    {
      return string.Format("({0}, {1})", (object) this.X1, (object) this.X2);
    }

    public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
    {
      bool flag = true;
      stringbuilder.Append("(");
      rest -= 2;
      try
      {
        if (flag = !Showing.Show((object) this.X1, stringbuilder, ref rest, formatProvider))
          return false;
        stringbuilder.Append(", ");
        rest -= 2;
        if (flag = !Showing.Show((object) this.X2, stringbuilder, ref rest, formatProvider))
          return false;
      }
      finally
      {
        if (flag)
        {
          stringbuilder.Append("...");
          rest -= 3;
        }
        stringbuilder.Append(")");
      }
      return true;
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Showing.ShowString((IShowable) this, format, formatProvider);
    }
  }
}
