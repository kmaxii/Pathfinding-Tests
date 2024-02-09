// Decompiled with JetBrains decompiler
// Type: C5.WeakViewList`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace C5
{
  [Serializable]
  internal class WeakViewList<V> where V : class
  {
    private WeakViewList<V>.Node start;

    internal WeakViewList<V>.Node Add(V view)
    {
      WeakViewList<V>.Node node = new WeakViewList<V>.Node(view);
      if (this.start != null)
      {
        this.start.prev = node;
        node.next = this.start;
      }
      this.start = node;
      return node;
    }

    internal void Remove(WeakViewList<V>.Node n)
    {
      if (n == this.start)
      {
        this.start = this.start.next;
        if (this.start == null)
          return;
        this.start.prev = (WeakViewList<V>.Node) null;
      }
      else
      {
        n.prev.next = n.next;
        if (n.next == null)
          return;
        n.next.prev = n.prev;
      }
    }

    public IEnumerator<V> GetEnumerator()
    {
      for (WeakViewList<V>.Node n = this.start; n != null; n = n.next)
      {
        object o = n.weakview.Target;
        V view = o is V ? (V) o : default (V);
        if ((object) view == null)
          this.Remove(n);
        else
          yield return view;
      }
    }

    [Serializable]
    internal class Node
    {
      internal WeakReference weakview;
      internal WeakViewList<V>.Node prev;
      internal WeakViewList<V>.Node next;

      internal Node(V view) => this.weakview = new WeakReference((object) view);
    }
  }
}
