// Decompiled with JetBrains decompiler
// Type: C5.KeyValuePair`2
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Text;

#nullable disable
namespace C5
{
  [Serializable]
  public struct KeyValuePair<K, V> : IEquatable<KeyValuePair<K, V>>, IShowable, IFormattable
  {
    public K Key;
    public V Value;

    public KeyValuePair(K key, V value)
    {
      this.Key = key;
      this.Value = value;
    }

    public KeyValuePair(K key)
    {
      this.Key = key;
      this.Value = default (V);
    }

    public override string ToString() => "(" + (object) this.Key + ", " + (object) this.Value + ")";

    public override bool Equals(object obj)
    {
      return obj is KeyValuePair<K, V> other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      return EqualityComparer<K>.Default.GetHashCode(this.Key) + 13984681 * EqualityComparer<V>.Default.GetHashCode(this.Value);
    }

    public bool Equals(KeyValuePair<K, V> other)
    {
      return EqualityComparer<K>.Default.Equals(this.Key, other.Key) && EqualityComparer<V>.Default.Equals(this.Value, other.Value);
    }

    public static bool operator ==(KeyValuePair<K, V> pair1, KeyValuePair<K, V> pair2)
    {
      return pair1.Equals(pair2);
    }

    public static bool operator !=(KeyValuePair<K, V> pair1, KeyValuePair<K, V> pair2)
    {
      return !pair1.Equals(pair2);
    }

    public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
    {
      if (rest < 0 || !Showing.Show((object) this.Key, stringbuilder, ref rest, formatProvider))
        return false;
      stringbuilder.Append(" => ");
      rest -= 4;
      return Showing.Show((object) this.Value, stringbuilder, ref rest, formatProvider) && rest >= 0;
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Showing.ShowString((IShowable) this, format, formatProvider);
    }
  }
}
