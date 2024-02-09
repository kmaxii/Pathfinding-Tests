// Decompiled with JetBrains decompiler
// Type: C5.C5Random
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;

#nullable disable
namespace C5
{
  [Serializable]
  public class C5Random : Random
  {
    private uint[] _q = new uint[16];
    private uint _c = 362436;
    private uint _i = 15;

    private uint Cmwc()
    {
      ulong num1 = 487198574;
      uint num2 = 4294967294;
      this._i = (uint) ((int) this._i + 1 & 15);
      ulong num3 = num1 * (ulong) this._q[(int) this._i] + (ulong) this._c;
      this._c = (uint) (num3 >> 32);
      uint num4 = (uint) (num3 + (ulong) this._c);
      if (num4 < this._c)
      {
        ++num4;
        ++this._c;
      }
      return this._q[(int) this._i] = num2 - num4;
    }

    public override double NextDouble() => (double) this.Cmwc() / 4294967296.0;

    protected override double Sample() => this.NextDouble();

    public override int Next() => (int) this.Cmwc();

    public override int Next(int max)
    {
      if (max < 0)
        throw new ArgumentException("max must be non-negative");
      return (int) ((double) this.Cmwc() / 4294967296.0 * (double) max);
    }

    public override int Next(int min, int max)
    {
      if (min > max)
        throw new ArgumentException("min must be less than or equal to max");
      return min + (int) ((double) this.Cmwc() / 4294967296.0 * (double) (max - min));
    }

    public override void NextBytes(byte[] buffer)
    {
      int index = 0;
      for (int length = buffer.Length; index < length; ++index)
        buffer[index] = (byte) this.Cmwc();
    }

    public C5Random()
      : this(DateTime.Now.Ticks)
    {
    }

    public C5Random(long seed)
    {
      if (seed == 0L)
        throw new ArgumentException("Seed must be non-zero");
      uint num1 = (uint) ((ulong) seed & (ulong) uint.MaxValue);
      for (int index = 0; index < 16; ++index)
      {
        uint num2 = num1 ^ num1 << 13;
        uint num3 = num2 ^ num2 >> 17;
        num1 = num3 ^ num3 << 5;
        this._q[index] = num1;
      }
      this._q[15] = (uint) (seed ^ seed >> 32);
    }

    [CLSCompliant(false)]
    public C5Random(uint[] q)
    {
      if (q.Length != 16)
        throw new ArgumentException("Q must have length 16, was " + (object) q.Length);
      Array.Copy((Array) q, (Array) this._q, 16);
    }
  }
}
