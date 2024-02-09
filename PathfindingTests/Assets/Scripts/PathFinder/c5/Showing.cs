// Decompiled with JetBrains decompiler
// Type: C5.Showing
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace C5
{
  [Serializable]
  public static class Showing
  {
    public static bool Show(
      object obj,
      StringBuilder stringbuilder,
      ref int rest,
      IFormatProvider formatProvider)
    {
      if (rest <= 0)
        return false;
      if (obj is IShowable showable)
        return showable.Show(stringbuilder, ref rest, formatProvider);
      int length = stringbuilder.Length;
      stringbuilder.AppendFormat(formatProvider, "{0}", obj);
      rest -= stringbuilder.Length - length;
      return true;
    }

    public static string ShowString(
      IShowable showable,
      string format,
      IFormatProvider formatProvider)
    {
      int rest = Showing.maxLength(format);
      StringBuilder stringbuilder = new StringBuilder();
      showable.Show(stringbuilder, ref rest, formatProvider);
      return stringbuilder.ToString();
    }

    private static int maxLength(string format)
    {
      if (format == null)
        return 80;
      return format.Length > 1 && format.StartsWith("L") ? int.Parse(format.Substring(1)) : int.MaxValue;
    }

    public static bool ShowCollectionValue<T>(
      ICollectionValue<T> items,
      StringBuilder stringbuilder,
      ref int rest,
      IFormatProvider formatProvider)
    {
      string str1 = "{ ";
      string str2 = " }";
      bool flag1 = false;
      bool flag2 = false;
      ICollection<T> collection = items as ICollection<T>;
      if (items is IList<T> list)
      {
        str1 = "[ ";
        str2 = " ]";
        flag1 = list.IndexingSpeed == Speed.Constant;
      }
      else if (collection != null && collection.AllowsDuplicates)
      {
        str1 = "{{ ";
        str2 = " }}";
        if (collection.DuplicatesByCounting)
          flag2 = true;
      }
      stringbuilder.Append(str1);
      rest -= 2 * str1.Length;
      bool flag3 = true;
      bool flag4 = true;
      int num = 0;
      if (flag2)
      {
        foreach (KeyValuePair<T, int> itemMultiplicity in (IEnumerable<KeyValuePair<T, int>>) collection.ItemMultiplicities())
        {
          flag4 = false;
          if (rest > 0)
          {
            if (flag3)
            {
              flag3 = false;
            }
            else
            {
              stringbuilder.Append(", ");
              rest -= 2;
            }
            if (flag4 = Showing.Show((object) itemMultiplicity.Key, stringbuilder, ref rest, formatProvider))
            {
              string str3 = string.Format("(*{0})", (object) itemMultiplicity.Value);
              stringbuilder.Append(str3);
              rest -= str3.Length;
            }
          }
          else
            break;
        }
      }
      else
      {
        foreach (T obj in (IEnumerable<T>) items)
        {
          flag4 = false;
          if (rest > 0)
          {
            if (flag3)
            {
              flag3 = false;
            }
            else
            {
              stringbuilder.Append(", ");
              rest -= 2;
            }
            if (flag1)
            {
              string str4 = string.Format("{0}:", (object) num++);
              stringbuilder.Append(str4);
              rest -= str4.Length;
            }
            flag4 = Showing.Show((object) obj, stringbuilder, ref rest, formatProvider);
          }
          else
            break;
        }
      }
      if (!flag4)
      {
        stringbuilder.Append("...");
        rest -= 3;
      }
      stringbuilder.Append(str2);
      return flag4;
    }

    public static bool ShowDictionary<K, V>(
      IDictionary<K, V> dictionary,
      StringBuilder stringbuilder,
      ref int rest,
      IFormatProvider formatProvider)
    {
      bool flag1 = dictionary is ISortedDictionary<K, V>;
      stringbuilder.Append(flag1 ? "[ " : "{ ");
      rest -= 4;
      bool flag2 = true;
      bool flag3 = true;
      foreach (KeyValuePair<K, V> keyValuePair in (IEnumerable<KeyValuePair<K, V>>) dictionary)
      {
        flag3 = false;
        if (rest > 0)
        {
          if (flag2)
          {
            flag2 = false;
          }
          else
          {
            stringbuilder.Append(", ");
            rest -= 2;
          }
          flag3 = Showing.Show((object) keyValuePair, stringbuilder, ref rest, formatProvider);
        }
        else
          break;
      }
      if (!flag3)
      {
        stringbuilder.Append("...");
        rest -= 3;
      }
      stringbuilder.Append(flag1 ? " ]" : " }");
      return flag3;
    }
  }
}
