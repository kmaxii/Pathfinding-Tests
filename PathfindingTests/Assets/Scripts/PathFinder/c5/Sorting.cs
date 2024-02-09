// Decompiled with JetBrains decompiler
// Type: C5.Sorting
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  public class Sorting
  {
    private Sorting()
    {
    }

    public static void IntroSort<T>(T[] array, int start, int count, IComparer<T> comparer)
    {
      if (start < 0 || count < 0 || start + count > array.Length)
        throw new ArgumentOutOfRangeException();
      new Sorting.Sorter<T>(array, comparer).IntroSort(start, start + count);
    }

    public static void IntroSort<T>(T[] array)
    {
      new Sorting.Sorter<T>(array, (IComparer<T>) Comparer<T>.Default).IntroSort(0, array.Length);
    }

    public static void InsertionSort<T>(T[] array, int start, int count, IComparer<T> comparer)
    {
      if (start < 0 || count < 0 || start + count > array.Length)
        throw new ArgumentOutOfRangeException();
      new Sorting.Sorter<T>(array, comparer).InsertionSort(start, start + count);
    }

    public static void HeapSort<T>(T[] array, int start, int count, IComparer<T> comparer)
    {
      if (start < 0 || count < 0 || start + count > array.Length)
        throw new ArgumentOutOfRangeException();
      new Sorting.Sorter<T>(array, comparer).HeapSort(start, start + count);
    }

    [Serializable]
    private class Sorter<T>
    {
      private T[] a;
      private IComparer<T> c;

      internal Sorter(T[] a, IComparer<T> c)
      {
        this.a = a;
        this.c = c;
      }

      internal void IntroSort(int f, int b)
      {
        if (b - f > 31)
        {
          int depth_limit = (int) Math.Floor(2.5 * Math.Log((double) (b - f), 2.0));
          this.introSort(f, b, depth_limit);
        }
        else
          this.InsertionSort(f, b);
      }

      private void introSort(int f, int b, int depth_limit)
      {
        if (depth_limit-- == 0)
          this.HeapSort(f, b);
        else if (b - f <= 14)
        {
          this.InsertionSort(f, b);
        }
        else
        {
          int num = this.partition(f, b);
          this.introSort(f, num, depth_limit);
          this.introSort(num, b, depth_limit);
        }
      }

      private int compare(T i1, T i2) => this.c.Compare(i1, i2);

      private int partition(int f, int b)
      {
        int index1 = f;
        int index2 = (b + f) / 2;
        int index3 = b - 1;
        T obj1 = this.a[index1];
        T obj2 = this.a[index2];
        T obj3 = this.a[index3];
        if (this.compare(obj1, obj2) < 0)
        {
          if (this.compare(obj3, obj1) < 0)
          {
            this.a[index3] = obj2;
            obj2 = this.a[index2] = obj1;
            this.a[index1] = obj3;
          }
          else if (this.compare(obj3, obj2) < 0)
          {
            this.a[index3] = obj2;
            obj2 = this.a[index2] = obj3;
          }
        }
        else if (this.compare(obj2, obj3) > 0)
        {
          this.a[index1] = obj3;
          this.a[index3] = obj1;
        }
        else if (this.compare(obj1, obj3) > 0)
        {
          this.a[index1] = obj2;
          obj2 = this.a[index2] = obj3;
          this.a[index3] = obj1;
        }
        else
        {
          this.a[index1] = obj2;
          obj2 = this.a[index2] = obj1;
        }
        int index4 = index1;
        int index5 = index3;
        while (true)
        {
          do
            ;
          while (this.compare(this.a[++index4], obj2) < 0);
          do
            ;
          while (this.compare(obj2, this.a[--index5]) < 0);
          if (index4 < index5)
          {
            T obj4 = this.a[index4];
            this.a[index4] = this.a[index5];
            this.a[index5] = obj4;
          }
          else
            break;
        }
        return index4;
      }

      internal void InsertionSort(int f, int b)
      {
        for (int index1 = f + 1; index1 < b; ++index1)
        {
          T y = this.a[index1];
          int index2 = index1 - 1;
          T obj1;
          if (this.c.Compare(obj1 = this.a[index2], y) > 0)
          {
            this.a[index1] = obj1;
            T obj2;
            while (index2 > f && this.c.Compare(obj2 = this.a[index2 - 1], y) > 0)
              this.a[index2--] = obj2;
            this.a[index2] = y;
          }
        }
      }

      internal void HeapSort(int f, int b)
      {
        for (int i = (b + f) / 2; i >= f; --i)
          this.heapify(f, b, i);
        for (int b1 = b - 1; b1 > f; --b1)
        {
          T obj = this.a[f];
          this.a[f] = this.a[b1];
          this.a[b1] = obj;
          this.heapify(f, b1, f);
        }
      }

      private void heapify(int f, int b, int i)
      {
        T obj1 = this.a[i];
        T i2 = obj1;
        int index1 = i;
        int num = index1;
        while (true)
        {
          int index2 = 2 * index1 - f + 1;
          int index3 = index2 + 1;
          T obj2;
          if (index2 < b && this.compare(obj2 = this.a[index2], i2) > 0)
          {
            num = index2;
            i2 = obj2;
          }
          T obj3;
          if (index3 < b && this.compare(obj3 = this.a[index3], i2) > 0)
          {
            num = index3;
            i2 = obj3;
          }
          if (num != index1)
          {
            this.a[index1] = i2;
            i2 = obj1;
            index1 = num;
          }
          else
            break;
        }
        if (index1 <= i)
          return;
        this.a[index1] = obj1;
      }
    }
  }
}
