// Decompiled with JetBrains decompiler
// Type: C5.TreeBag`1
// Assembly: C5, Version=2.4.6353.30055, Culture=neutral, PublicKeyToken=282361b99ded7e8e
// MVID: AEBB566B-968E-48F4-888F-EABAD43298DA
// Assembly location: F:\Programming\EpPathFinding.cs\EpPathFinding.cs\EpPathFinding.cs\PathFinder\UnityC5\C5.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace C5
{
  [Serializable]
  public class TreeBag<T> : 
    SequencedBase<T>,
    IIndexedSorted<T>,
    IIndexed<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IPersistentSorted<T>,
    ISorted<T>,
    ISequenced<T>,
    ICollection<T>,
    IExtensible<T>,
    System.Collections.Generic.ICollection<T>,
    IDirectedCollectionValue<T>,
    ICollectionValue<T>,
    IShowable,
    IFormattable,
    IDirectedEnumerable<T>,
    IEnumerable<T>,
    IEnumerable,
    IDisposable
  {
    private IComparer<T> comparer;
    private TreeBag<T>.Node root;
    private int blackdepth;
    private int[] dirs = new int[2];
    private TreeBag<T>.Node[] path = new TreeBag<T>.Node[2];
    private bool isSnapShot;
    private int generation;
    private bool isValid = true;
    private TreeBag<T>.SnapRef snapList;

    public int UniqueCount { get; private set; }

    public override EventTypeEnum ListenableEvents => EventTypeEnum.Basic;

    private TreeBag<T>.Node left(TreeBag<T>.Node n)
    {
      return this.isSnapShot && n.lastgeneration >= this.generation && n.leftnode ? n.oldref : n.left;
    }

    private TreeBag<T>.Node right(TreeBag<T>.Node n)
    {
      return this.isSnapShot && n.lastgeneration >= this.generation && !n.leftnode ? n.oldref : n.right;
    }

    private void stackcheck()
    {
      while (this.dirs.Length < 2 * this.blackdepth)
      {
        this.dirs = new int[2 * this.dirs.Length];
        this.path = new TreeBag<T>.Node[2 * this.dirs.Length];
      }
    }

    public TreeBag(MemoryType memoryType = MemoryType.Normal)
      : this((IComparer<T>) System.Collections.Generic.Comparer<T>.Default, C5.EqualityComparer<T>.Default, memoryType)
    {
    }

    public TreeBag(IComparer<T> comparer, MemoryType memoryType = MemoryType.Normal)
      : this(comparer, (IEqualityComparer<T>) new ComparerZeroHashCodeEqualityComparer<T>(comparer), memoryType)
    {
    }

    public TreeBag(
      IComparer<T> comparer,
      IEqualityComparer<T> equalityComparer,
      MemoryType memoryType = MemoryType.Normal)
      : base(equalityComparer, memoryType)
    {
      if (comparer == null)
        throw new NullReferenceException("Item comparer cannot be null");
      if (memoryType != MemoryType.Normal)
        throw new Exception("TreeBag doesn't support MemoryType Strict or Safe.");
      this.comparer = comparer;
    }

    private IEnumerator<T> getEnumerator(TreeBag<T>.Node node, int origstamp)
    {
      if (node != null)
      {
        if (node.left != null)
        {
          IEnumerator<T> child = this.getEnumerator(node.left, origstamp);
          while (child.MoveNext())
          {
            this.modifycheck(origstamp);
            yield return child.Current;
          }
        }
        int togo = node.items;
        while (togo-- > 0)
        {
          this.modifycheck(origstamp);
          yield return node.item;
        }
        if (node.right != null)
        {
          IEnumerator<T> child = this.getEnumerator(node.right, origstamp);
          while (child.MoveNext())
          {
            this.modifycheck(origstamp);
            yield return child.Current;
          }
        }
      }
    }

    public override T Choose()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      if (this.size == 0)
        throw new NoSuchItemException();
      return this.root.item;
    }

    public override IEnumerator<T> GetEnumerator()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return this.isSnapShot ? (IEnumerator<T>) new TreeBag<T>.SnapEnumerator(this) : this.getEnumerator(this.root, this.stamp);
    }

    private bool addIterative(T item, ref T founditem, bool update, out bool wasfound)
    {
      wasfound = false;
      if (this.root == null)
      {
        this.root = new TreeBag<T>.Node();
        this.root.red = false;
        this.blackdepth = 1;
        this.root.item = item;
        this.root.generation = this.generation;
        return true;
      }
      this.stackcheck();
      int index1 = 0;
      TreeBag<T>.Node cursor1 = this.root;
      int num1;
      while (true)
      {
        num1 = this.comparer.Compare(cursor1.item, item);
        if (num1 != 0)
        {
          TreeBag<T>.Node node = num1 > 0 ? cursor1.left : cursor1.right;
          if (node != null)
          {
            this.dirs[index1] = num1;
            this.path[index1++] = cursor1;
            cursor1 = node;
          }
          else
            goto label_14;
        }
        else
          break;
      }
      founditem = cursor1.item;
      wasfound = true;
      bool flag1 = true;
      TreeBag<T>.Node.CopyNode(ref cursor1, this.maxsnapid, this.generation);
      if (update)
      {
        cursor1.item = item;
      }
      else
      {
        ++cursor1.items;
        ++cursor1.size;
      }
      while (index1-- > 0)
      {
        if (flag1)
        {
          TreeBag<T>.Node child = cursor1;
          cursor1 = this.path[index1];
          TreeBag<T>.Node.update(ref cursor1, this.dirs[index1] > 0, child, this.maxsnapid, this.generation);
          if (!update)
            ++cursor1.size;
        }
        this.path[index1] = (TreeBag<T>.Node) null;
      }
      this.root = cursor1;
      return !update;
label_14:
      TreeBag<T>.Node.update(ref cursor1, num1 > 0, new TreeBag<T>.Node()
      {
        item = item,
        generation = this.generation
      }, this.maxsnapid, this.generation);
      ++cursor1.size;
      this.dirs[index1] = num1;
      while (cursor1.red)
      {
        TreeBag<T>.Node cursor2 = cursor1;
        int index2;
        TreeBag<T>.Node cursor3 = this.path[index2 = index1 - 1];
        this.path[index2] = (TreeBag<T>.Node) null;
        TreeBag<T>.Node.update(ref cursor3, this.dirs[index2] > 0, cursor2, this.maxsnapid, this.generation);
        ++cursor3.size;
        int dir1 = this.dirs[index2];
        TreeBag<T>.Node child1 = dir1 > 0 ? cursor3.right : cursor3.left;
        if (child1 != null && child1.red)
        {
          cursor2.red = false;
          TreeBag<T>.Node.update(ref cursor3, dir1 < 0, child1, this.maxsnapid, this.generation);
          child1.red = false;
          if (index2 == 0)
          {
            this.root = cursor3;
            ++this.blackdepth;
            return true;
          }
          cursor3.red = true;
          TreeBag<T>.Node child2 = cursor3;
          cursor1 = this.path[index1 = index2 - 1];
          TreeBag<T>.Node.update(ref cursor1, this.dirs[index1] > 0, child2, this.maxsnapid, this.generation);
          this.path[index1] = (TreeBag<T>.Node) null;
          ++cursor1.size;
        }
        else
        {
          int dir2 = this.dirs[index2 + 1];
          cursor3.red = true;
          TreeBag<T>.Node node1;
          if (dir1 > 0)
          {
            if (dir2 > 0)
            {
              TreeBag<T>.Node.update(ref cursor3, true, cursor2.right, this.maxsnapid, this.generation);
              TreeBag<T>.Node.update(ref cursor2, false, cursor3, this.maxsnapid, this.generation);
              node1 = cursor2;
            }
            else
            {
              TreeBag<T>.Node right = cursor2.right;
              TreeBag<T>.Node.update(ref cursor3, true, right.right, this.maxsnapid, this.generation);
              TreeBag<T>.Node.update(ref cursor2, false, right.left, this.maxsnapid, this.generation);
              TreeBag<T>.Node.CopyNode(ref right, this.maxsnapid, this.generation);
              right.left = cursor2;
              right.right = cursor3;
              node1 = right;
            }
          }
          else if (dir2 < 0)
          {
            TreeBag<T>.Node.update(ref cursor3, false, cursor2.left, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor2, true, cursor3, this.maxsnapid, this.generation);
            node1 = cursor2;
          }
          else
          {
            TreeBag<T>.Node left = cursor2.left;
            TreeBag<T>.Node.update(ref cursor3, false, left.left, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor2, true, left.right, this.maxsnapid, this.generation);
            TreeBag<T>.Node.CopyNode(ref left, this.maxsnapid, this.generation);
            left.right = cursor2;
            left.left = cursor3;
            node1 = left;
          }
          node1.red = false;
          TreeBag<T>.Node right1 = node1.right;
          TreeBag<T>.Node node2 = node1;
          TreeBag<T>.Node node3 = right1;
          int size1 = right1.left == null ? 0 : right1.left.size;
          int size2 = right1.right == null ? 0 : right1.right.size;
          int num2;
          int num3 = num2 = size1 + size2 + right1.items;
          node3.size = num2;
          int num4 = num3;
          node2.size = num4;
          TreeBag<T>.Node left1 = node1.left;
          left1.size = (left1.left == null ? 0 : left1.left.size) + (left1.right == null ? 0 : left1.right.size) + left1.items;
          node1.size += left1.size + node1.items;
          if (index2 == 0)
          {
            this.root = node1;
            return true;
          }
          TreeBag<T>.Node child3 = node1;
          cursor1 = this.path[index1 = index2 - 1];
          this.path[index1] = (TreeBag<T>.Node) null;
          TreeBag<T>.Node.update(ref cursor1, this.dirs[index1] > 0, child3, this.maxsnapid, this.generation);
          ++cursor1.size;
          break;
        }
      }
      bool flag2 = true;
      while (index1 > 0)
      {
        TreeBag<T>.Node child = cursor1;
        cursor1 = this.path[--index1];
        this.path[index1] = (TreeBag<T>.Node) null;
        if (flag2)
          flag2 = TreeBag<T>.Node.update(ref cursor1, this.dirs[index1] > 0, child, this.maxsnapid, this.generation);
        ++cursor1.size;
      }
      this.root = cursor1;
      return true;
    }

    public bool Add(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      T j = default (T);
      if (!this.add(item, ref j))
        return false;
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForAdd(j);
      return true;
    }

    void System.Collections.Generic.ICollection<T>.Add(T item) => this.Add(item);

    private bool add(T item, ref T j)
    {
      bool wasfound;
      if (!this.addIterative(item, ref j, false, out wasfound))
        return false;
      ++this.size;
      if (!wasfound)
      {
        ++this.UniqueCount;
        j = item;
      }
      return true;
    }

    public void AddAll(IEnumerable<T> items)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      int num1 = 0;
      int num2 = 0;
      T founditem = default (T);
      bool flag = (this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None;
      CircularQueue<T> circularQueue = flag ? new CircularQueue<T>() : (CircularQueue<T>) null;
      foreach (T obj in items)
      {
        bool wasfound;
        if (this.addIterative(obj, ref founditem, false, out wasfound))
        {
          ++num1;
          if (!wasfound)
            ++num2;
          if (flag)
            circularQueue.Enqueue(wasfound ? founditem : obj);
        }
      }
      if (num1 == 0)
        return;
      this.size += num1;
      this.UniqueCount += num2;
      if (flag)
      {
        foreach (T obj in (EnumerableBase<T>) circularQueue)
          this.raiseItemsAdded(obj, 1);
      }
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public void AddSorted(IEnumerable<T> items)
    {
      if (this.size > 0)
      {
        this.AddAll(items);
      }
      else
      {
        if (!this.isValid)
          throw new ViewDisposedException("Snapshot has been disposed");
        this.updatecheck();
        this.addSorted(items, true, true);
      }
    }

    private static TreeBag<T>.Node maketreer(
      ref TreeBag<T>.Node rest,
      int blackheight,
      int maxred,
      int red)
    {
      if (blackheight == 1)
      {
        TreeBag<T>.Node node = rest;
        rest = rest.right;
        if (red > 0)
        {
          node.right = (TreeBag<T>.Node) null;
          rest.left = node;
          node = rest;
          node.size += node.left.size;
          rest = rest.right;
          --red;
        }
        if (red > 0)
        {
          node.size += rest.size;
          node.right = rest;
          rest = rest.right;
          node.right.right = (TreeBag<T>.Node) null;
        }
        else
          node.right = (TreeBag<T>.Node) null;
        node.red = false;
        return node;
      }
      maxred >>= 1;
      int red1 = red > maxred ? maxred : red;
      TreeBag<T>.Node node1 = TreeBag<T>.maketreer(ref rest, blackheight - 1, maxred, red1);
      TreeBag<T>.Node node2 = rest;
      rest = rest.right;
      node2.left = node1;
      node2.red = false;
      node2.right = TreeBag<T>.maketreer(ref rest, blackheight - 1, maxred, red - red1);
      node2.size = node2.items + node2.left.size + node2.right.size;
      return node2;
    }

    private void addSorted(IEnumerable<T> items, bool safe, bool raise)
    {
      IEnumerator<T> enumerator = items.GetEnumerator();
      if (this.size > 0)
        throw new InternalException("This can't happen");
      if (!enumerator.MoveNext())
        return;
      TreeBag<T>.Node rest = new TreeBag<T>.Node();
      TreeBag<T>.Node node = rest;
      int num1 = 1;
      T x = node.item = enumerator.Current;
      int num2 = 0;
      while (enumerator.MoveNext())
      {
        T current = enumerator.Current;
        int num3 = this.comparer.Compare(x, current);
        if (num3 > 0)
          throw new ArgumentException("Argument not sorted");
        if (num3 == 0)
        {
          ++node.items;
          ++num2;
        }
        else
        {
          node.size = node.items;
          ++num1;
          node.right = new TreeBag<T>.Node();
          node = node.right;
          x = node.item = current;
          node.generation = this.generation;
        }
      }
      node.size = node.items;
      int blackheight = 0;
      int red = num1;
      int maxred = 1;
      while (maxred <= red)
      {
        red -= maxred;
        maxred <<= 1;
        ++blackheight;
      }
      this.root = TreeBag<T>.maketreer(ref rest, blackheight, maxred, red);
      this.blackdepth = blackheight;
      this.size = num1;
      this.UniqueCount = num1;
      this.size += num2;
      if (!raise)
        return;
      if ((this.ActiveEvents & EventTypeEnum.Added) != EventTypeEnum.None)
      {
        CircularQueue<T> circularQueue = new CircularQueue<T>();
        foreach (T obj in (EnumerableBase<T>) this)
          circularQueue.Enqueue(obj);
        foreach (T obj in (EnumerableBase<T>) circularQueue)
          this.raiseItemsAdded(obj, 1);
      }
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public bool AllowsDuplicates => true;

    public virtual bool DuplicatesByCounting => true;

    public Speed ContainsSpeed => Speed.Log;

    public bool Contains(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      int num;
      for (TreeBag<T>.Node n = this.root; n != null; n = num < 0 ? this.right(n) : this.left(n))
      {
        num = this.comparer.Compare(n.item, item);
        if (num == 0)
          return true;
      }
      return false;
    }

    public bool Find(ref T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      int num;
      for (TreeBag<T>.Node n = this.root; n != null; n = num < 0 ? this.right(n) : this.left(n))
      {
        num = this.comparer.Compare(n.item, item);
        if (num == 0)
        {
          item = n.item;
          return true;
        }
      }
      return false;
    }

    public bool FindOrAdd(ref T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      bool wasfound;
      if (!this.addIterative(item, ref item, false, out wasfound))
        return true;
      ++this.size;
      if (!wasfound)
        ++this.UniqueCount;
      if (this.ActiveEvents != EventTypeEnum.None && !wasfound)
        this.raiseForAdd(item);
      return wasfound;
    }

    public bool Update(T item)
    {
      T olditem = item;
      return this.Update(item, out olditem);
    }

    public bool Update(T item, out T olditem)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      this.stackcheck();
      int index = 0;
      TreeBag<T>.Node cursor = this.root;
      int num;
      for (; cursor != null; cursor = num < 0 ? cursor.right : cursor.left)
      {
        num = this.comparer.Compare(cursor.item, item);
        if (num == 0)
        {
          TreeBag<T>.Node.CopyNode(ref cursor, this.maxsnapid, this.generation);
          olditem = cursor.item;
          int items = cursor.items;
          cursor.item = item;
          while (index > 0)
          {
            TreeBag<T>.Node child = cursor;
            cursor = this.path[--index];
            this.path[index] = (TreeBag<T>.Node) null;
            TreeBag<T>.Node.update(ref cursor, this.dirs[index] > 0, child, this.maxsnapid, this.generation);
          }
          this.root = cursor;
          if (this.ActiveEvents != EventTypeEnum.None)
            this.raiseForUpdate(item, olditem, items);
          return true;
        }
        this.dirs[index] = num;
        this.path[index++] = cursor;
      }
      olditem = default (T);
      return false;
    }

    public bool UpdateOrAdd(T item) => this.UpdateOrAdd(item, out T _);

    public bool UpdateOrAdd(T item, out T olditem)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      olditem = default (T);
      bool wasfound;
      if (this.addIterative(item, ref olditem, true, out wasfound))
      {
        ++this.size;
        if (!wasfound)
          ++this.UniqueCount;
        if (this.ActiveEvents != EventTypeEnum.None)
          this.raiseForAdd(wasfound ? olditem : item);
        return wasfound;
      }
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForUpdate(item, olditem, 1);
      return true;
    }

    public bool Remove(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      if (this.root == null)
        return false;
      bool flag = this.removeIterative(ref item, false, out int _);
      if (this.ActiveEvents != EventTypeEnum.None && flag)
        this.raiseForRemove(item);
      return flag;
    }

    public bool Remove(T item, out T removeditem)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      removeditem = item;
      if (this.root == null)
        return false;
      bool flag = this.removeIterative(ref removeditem, false, out int _);
      if (this.ActiveEvents != EventTypeEnum.None && flag)
        this.raiseForRemove(item);
      return flag;
    }

    private bool removeIterative(ref T item, bool all, out int wasRemoved)
    {
      wasRemoved = 0;
      this.stackcheck();
      int level = 0;
      TreeBag<T>.Node cursor = this.root;
      while (true)
      {
        int num = this.comparer.Compare(cursor.item, item);
        if (num != 0)
        {
          TreeBag<T>.Node node = num > 0 ? cursor.left : cursor.right;
          if (node != null)
          {
            this.dirs[level] = num;
            this.path[level++] = cursor;
            cursor = node;
          }
          else
            goto label_9;
        }
        else
          break;
      }
      item = cursor.item;
      if (!all && cursor.items > 1)
      {
        TreeBag<T>.Node.CopyNode(ref cursor, this.maxsnapid, this.generation);
        --cursor.items;
        --cursor.size;
        while (level-- > 0)
        {
          TreeBag<T>.Node child = cursor;
          cursor = this.path[level];
          TreeBag<T>.Node.update(ref cursor, this.dirs[level] > 0, child, this.maxsnapid, this.generation);
          --cursor.size;
          this.path[level] = (TreeBag<T>.Node) null;
        }
        --this.size;
        wasRemoved = 1;
        return true;
      }
      wasRemoved = cursor.items;
      return this.removeIterativePhase2(cursor, level);
label_9:
      return false;
    }

    private bool removeIterativePhase2(TreeBag<T>.Node cursor, int level)
    {
      if (this.size == 1)
      {
        this.clear();
        return true;
      }
      --this.UniqueCount;
      this.size -= cursor.items;
      int index = level;
      if (cursor.left != null && cursor.right != null)
      {
        this.dirs[level] = 1;
        this.path[level++] = cursor;
        for (cursor = cursor.left; cursor.right != null; cursor = cursor.right)
        {
          this.dirs[level] = -1;
          this.path[level++] = cursor;
        }
        TreeBag<T>.Node.CopyNode(ref this.path[index], this.maxsnapid, this.generation);
        this.path[index].item = cursor.item;
        this.path[index].items = cursor.items;
      }
      TreeBag<T>.Node child1 = cursor.right == null ? cursor.left : cursor.right;
      bool flag = child1 == null && !cursor.red;
      if (child1 != null)
        child1.red = false;
      if (level == 0)
      {
        this.root = child1;
        return true;
      }
      --level;
      cursor = this.path[level];
      this.path[level] = (TreeBag<T>.Node) null;
      int dir = this.dirs[level];
      TreeBag<T>.Node.update(ref cursor, dir > 0, child1, this.maxsnapid, this.generation);
      TreeBag<T>.Node cursor1 = dir > 0 ? cursor.right : cursor.left;
      cursor.size = cursor.items + (child1 == null ? 0 : child1.size) + (cursor1 == null ? 0 : cursor1.size);
      TreeBag<T>.Node node1 = (TreeBag<T>.Node) null;
      TreeBag<T>.Node cursor2 = (TreeBag<T>.Node) null;
      while (flag && !cursor1.red)
      {
        node1 = dir > 0 ? cursor1.right : cursor1.left;
        if (node1 == null || !node1.red)
        {
          cursor2 = dir > 0 ? cursor1.left : cursor1.right;
          if (cursor2 == null || !cursor2.red)
          {
            cursor1.red = true;
            if (level == 0)
            {
              cursor.red = false;
              --this.blackdepth;
              this.root = cursor;
              return true;
            }
            if (cursor.red)
            {
              cursor.red = false;
              flag = false;
              break;
            }
            TreeBag<T>.Node child2 = cursor;
            cursor = this.path[--level];
            this.path[level] = (TreeBag<T>.Node) null;
            dir = this.dirs[level];
            cursor1 = dir > 0 ? cursor.right : cursor.left;
            TreeBag<T>.Node.update(ref cursor, dir > 0, child2, this.maxsnapid, this.generation);
            cursor.size = cursor.items + (child2 == null ? 0 : child2.size) + (cursor1 == null ? 0 : cursor1.size);
          }
          else
            break;
        }
        else
          break;
      }
      if (flag)
      {
        TreeBag<T>.Node cursor3 = cursor;
        if (cursor1.red)
        {
          TreeBag<T>.Node cursor4;
          TreeBag<T>.Node node2;
          TreeBag<T>.Node cursor5;
          TreeBag<T>.Node node3;
          if (dir > 0)
          {
            cursor4 = cursor1.left;
            node2 = cursor1.right;
            cursor5 = cursor4.left;
            node3 = cursor4.right;
          }
          else
          {
            cursor4 = cursor1.right;
            node2 = cursor1.left;
            cursor5 = cursor4.right;
            node3 = cursor4.left;
          }
          if (node3 != null && node3.red)
          {
            TreeBag<T>.Node.CopyNode(ref cursor4, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor3, dir < 0, cursor5, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor1, dir > 0, cursor4, this.maxsnapid, this.generation);
            if (dir > 0)
            {
              cursor4.left = cursor3;
              cursor3.right = cursor5;
            }
            else
            {
              cursor4.right = cursor3;
              cursor3.left = cursor5;
            }
            cursor = cursor1;
            cursor1.red = false;
            cursor4.red = true;
            node3.red = false;
            cursor.size = cursor3.size;
            cursor4.size = cursor.size - cursor.items - node2.size;
            cursor3.size = cursor4.size - cursor4.items - node3.size;
          }
          else if (cursor5 != null && cursor5.red)
          {
            TreeBag<T>.Node.CopyNode(ref cursor5, this.maxsnapid, this.generation);
            if (dir > 0)
            {
              TreeBag<T>.Node.update(ref cursor1, true, cursor5, this.maxsnapid, this.generation);
              TreeBag<T>.Node.update(ref cursor4, true, cursor5.right, this.maxsnapid, this.generation);
              TreeBag<T>.Node.update(ref cursor3, false, cursor5.left, this.maxsnapid, this.generation);
              cursor5.left = cursor3;
              cursor5.right = cursor4;
            }
            else
            {
              TreeBag<T>.Node.update(ref cursor1, false, cursor5, this.maxsnapid, this.generation);
              TreeBag<T>.Node.update(ref cursor4, false, cursor5.left, this.maxsnapid, this.generation);
              TreeBag<T>.Node.update(ref cursor3, true, cursor5.right, this.maxsnapid, this.generation);
              cursor5.right = cursor3;
              cursor5.left = cursor4;
            }
            cursor = cursor1;
            cursor1.red = false;
            cursor.size = cursor3.size;
            cursor3.size = cursor3.items + (cursor3.left == null ? 0 : cursor3.left.size) + (cursor3.right == null ? 0 : cursor3.right.size);
            cursor4.size = cursor4.items + (cursor4.left == null ? 0 : cursor4.left.size) + (cursor4.right == null ? 0 : cursor4.right.size);
            cursor5.size = cursor5.items + cursor3.size + cursor4.size;
          }
          else
          {
            TreeBag<T>.Node.update(ref cursor3, dir < 0, cursor4, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor1, dir > 0, cursor3, this.maxsnapid, this.generation);
            cursor = cursor1;
            cursor1.red = false;
            cursor4.red = true;
            cursor.size = cursor3.size;
            cursor3.size -= node2.size + cursor.items;
          }
        }
        else if (node1 != null && node1.red)
        {
          TreeBag<T>.Node child3 = dir > 0 ? cursor1.left : cursor1.right;
          TreeBag<T>.Node.update(ref cursor3, dir < 0, child3, this.maxsnapid, this.generation);
          TreeBag<T>.Node.CopyNode(ref cursor1, this.maxsnapid, this.generation);
          if (dir > 0)
          {
            cursor1.left = cursor3;
            cursor1.right = node1;
          }
          else
          {
            cursor1.right = cursor3;
            cursor1.left = node1;
          }
          cursor = cursor1;
          cursor.red = cursor3.red;
          cursor3.red = false;
          node1.red = false;
          cursor.size = cursor3.size;
          cursor3.size -= node1.size + cursor.items;
        }
        else
        {
          if (cursor2 == null || !cursor2.red)
            throw new InternalException("Case 1a can't happen here");
          TreeBag<T>.Node.CopyNode(ref cursor2, this.maxsnapid, this.generation);
          if (dir > 0)
          {
            TreeBag<T>.Node.update(ref cursor1, true, cursor2.right, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor3, false, cursor2.left, this.maxsnapid, this.generation);
            cursor2.left = cursor3;
            cursor2.right = cursor1;
          }
          else
          {
            TreeBag<T>.Node.update(ref cursor1, false, cursor2.left, this.maxsnapid, this.generation);
            TreeBag<T>.Node.update(ref cursor3, true, cursor2.right, this.maxsnapid, this.generation);
            cursor2.right = cursor3;
            cursor2.left = cursor1;
          }
          cursor = cursor2;
          cursor.red = cursor3.red;
          cursor3.red = false;
          cursor.size = cursor3.size;
          cursor3.size = cursor3.items + (cursor3.left == null ? 0 : cursor3.left.size) + (cursor3.right == null ? 0 : cursor3.right.size);
          cursor1.size = cursor1.items + (cursor1.left == null ? 0 : cursor1.left.size) + (cursor1.right == null ? 0 : cursor1.right.size);
        }
        if (level == 0)
        {
          this.root = cursor;
        }
        else
        {
          TreeBag<T>.Node child4 = cursor;
          cursor = this.path[--level];
          this.path[level] = (TreeBag<T>.Node) null;
          TreeBag<T>.Node.update(ref cursor, this.dirs[level] > 0, child4, this.maxsnapid, this.generation);
          cursor.size = cursor.items + (cursor.right == null ? 0 : cursor.right.size) + (cursor.left == null ? 0 : cursor.left.size);
        }
      }
      while (level > 0)
      {
        TreeBag<T>.Node child5 = cursor;
        cursor = this.path[--level];
        this.path[level] = (TreeBag<T>.Node) null;
        if (child5 != (this.dirs[level] > 0 ? cursor.left : cursor.right))
          TreeBag<T>.Node.update(ref cursor, this.dirs[level] > 0, child5, this.maxsnapid, this.generation);
        cursor.size = cursor.items + (cursor.right == null ? 0 : cursor.right.size) + (cursor.left == null ? 0 : cursor.left.size);
      }
      this.root = cursor;
      return true;
    }

    public void Clear()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      if (this.size == 0)
        return;
      int size = this.size;
      this.clear();
      if ((this.ActiveEvents & EventTypeEnum.Cleared) != EventTypeEnum.None)
        this.raiseCollectionCleared(true, size);
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    private void clear()
    {
      this.size = 0;
      this.UniqueCount = 0;
      this.root = (TreeBag<T>.Node) null;
      this.blackdepth = 0;
    }

    public void RemoveAll(IEnumerable<T> items)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      bool flag = (this.ActiveEvents & (EventTypeEnum.Changed | EventTypeEnum.Removed)) != EventTypeEnum.None;
      CollectionValueBase<T>.RaiseForRemoveAllHandler removeAllHandler = flag ? new CollectionValueBase<T>.RaiseForRemoveAllHandler((CollectionValueBase<T>) this) : (CollectionValueBase<T>.RaiseForRemoveAllHandler) null;
      foreach (T obj1 in items)
      {
        if (this.root != null)
        {
          T obj2 = obj1;
          if (this.removeIterative(ref obj2, false, out int _) && flag)
            removeAllHandler.Remove(obj2);
        }
        else
          break;
      }
      if (!flag)
        return;
      removeAllHandler.Raise();
    }

    public void RetainAll(IEnumerable<T> items)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      TreeBag<T> treeBag = (TreeBag<T>) this.MemberwiseClone();
      T j = default (T);
      treeBag.clear();
      foreach (T obj in items)
      {
        if (this.ContainsCount(obj) > treeBag.ContainsCount(obj))
          treeBag.add(obj, ref j);
      }
      if (this.size == treeBag.size)
        return;
      CircularQueue<KeyValuePair<T, int>> circularQueue = (CircularQueue<KeyValuePair<T, int>>) null;
      if ((this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None)
      {
        circularQueue = new CircularQueue<KeyValuePair<T, int>>();
        IEnumerator<KeyValuePair<T, int>> enumerator = this.ItemMultiplicities().GetEnumerator();
        foreach (KeyValuePair<T, int> itemMultiplicity in (IEnumerable<KeyValuePair<T, int>>) treeBag.ItemMultiplicities())
        {
          while (enumerator.MoveNext())
          {
            if (this.comparer.Compare(enumerator.Current.Key, itemMultiplicity.Key) == 0)
            {
              int num = enumerator.Current.Value - itemMultiplicity.Value;
              if (num > 0)
              {
                circularQueue.Enqueue(new KeyValuePair<T, int>(itemMultiplicity.Key, num));
                break;
              }
              break;
            }
            circularQueue.Enqueue(enumerator.Current);
          }
        }
        while (enumerator.MoveNext())
          circularQueue.Enqueue(enumerator.Current);
      }
      this.root = treeBag.root;
      this.size = treeBag.size;
      this.UniqueCount = treeBag.UniqueCount;
      this.blackdepth = treeBag.blackdepth;
      if (circularQueue != null)
      {
        foreach (KeyValuePair<T, int> keyValuePair in (EnumerableBase<KeyValuePair<T, int>>) circularQueue)
          this.raiseItemsRemoved(keyValuePair.Key, keyValuePair.Value);
      }
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public bool ContainsAll(IEnumerable<T> items)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      foreach (T obj in items)
      {
        if (!this.Contains(obj))
          return false;
      }
      return true;
    }

    public IIndexedSorted<T> FindAll(Func<T, bool> filter)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<T> all = new TreeBag<T>(this.comparer);
      IEnumerator<T> enumerator = this.GetEnumerator();
      TreeBag<T>.Node rest = (TreeBag<T>.Node) null;
      TreeBag<T>.Node node = (TreeBag<T>.Node) null;
      int num1 = 0;
      int num2 = 0;
      while (enumerator.MoveNext())
      {
        T current = enumerator.Current;
        if (node != null && this.comparer.Compare(current, node.item) == 0)
        {
          ++node.items;
          ++num2;
        }
        else if (filter(current))
        {
          if (rest == null)
          {
            rest = node = new TreeBag<T>.Node();
          }
          else
          {
            node.size = node.items;
            node.right = new TreeBag<T>.Node();
            node = node.right;
          }
          node.item = current;
          ++num1;
        }
      }
      if (node != null)
        node.size = node.items;
      if (num1 == 0)
        return (IIndexedSorted<T>) all;
      int blackheight = 0;
      int red = num1;
      int maxred = 1;
      while (maxred <= red)
      {
        red -= maxred;
        maxred <<= 1;
        ++blackheight;
      }
      all.root = TreeBag<T>.maketreer(ref rest, blackheight, maxred, red);
      all.blackdepth = blackheight;
      all.size = num1;
      all.UniqueCount = num1;
      all.size += num2;
      return (IIndexedSorted<T>) all;
    }

    public IIndexedSorted<V> Map<V>(Func<T, V> mapper, IComparer<V> c)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<V> treeBag = new TreeBag<V>(c);
      if (this.size == 0)
        return (IIndexedSorted<V>) treeBag;
      IEnumerator<T> enumerator = this.GetEnumerator();
      TreeBag<V>.Node rest = (TreeBag<V>.Node) null;
      TreeBag<V>.Node node = (TreeBag<V>.Node) null;
      V x = default (V);
      int num1 = 0;
      T y1 = default (T);
      while (enumerator.MoveNext())
      {
        T current = enumerator.Current;
        if (node != null && this.comparer.Compare(current, y1) == 0)
        {
          ++node.items;
        }
        else
        {
          V y2 = mapper(current);
          if (rest == null)
          {
            rest = node = new TreeBag<V>.Node();
            ++num1;
          }
          else
          {
            int num2 = c.Compare(x, y2);
            if (num2 == 0)
            {
              ++node.items;
              continue;
            }
            if (num2 > 0)
              throw new ArgumentException("mapper not monotonic");
            node.size = node.items;
            node.right = new TreeBag<V>.Node();
            node = node.right;
            ++num1;
          }
          y1 = current;
          node.item = x = y2;
        }
      }
      node.size = node.items;
      int blackheight = 0;
      int red = num1;
      int maxred = 1;
      while (maxred <= red)
      {
        red -= maxred;
        maxred <<= 1;
        ++blackheight;
      }
      treeBag.root = TreeBag<V>.maketreer(ref rest, blackheight, maxred, red);
      treeBag.blackdepth = blackheight;
      treeBag.size = this.size;
      treeBag.UniqueCount = this.UniqueCount;
      return (IIndexedSorted<V>) treeBag;
    }

    public int ContainsCount(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      int num;
      for (TreeBag<T>.Node n = this.root; n != null; n = num < 0 ? this.right(n) : this.left(n))
      {
        num = this.comparer.Compare(n.item, item);
        if (num == 0)
          return n.items;
      }
      return 0;
    }

    public virtual ICollectionValue<T> UniqueItems()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return (ICollectionValue<T>) new DropMultiplicity<T>(this.ItemMultiplicities());
    }

    public virtual ICollectionValue<KeyValuePair<T, int>> ItemMultiplicities()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return (ICollectionValue<KeyValuePair<T, int>>) new TreeBag<T>.Multiplicities(this);
    }

    public void RemoveAllCopies(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      int wasRemoved;
      if (!this.removeIterative(ref item, true, out wasRemoved) || this.ActiveEvents == EventTypeEnum.None)
        return;
      this.raiseForRemove(item, wasRemoved);
    }

    private TreeBag<T>.Node findNode(int i)
    {
      if (this.isSnapShot)
        throw new NotSupportedException("Indexing not supported for snapshots");
      TreeBag<T>.Node node = this.root;
      if (i < 0 || i >= this.size)
        throw new IndexOutOfRangeException();
      while (true)
      {
        int size = node.left == null ? 0 : node.left.size;
        if (i > size)
        {
          i -= size + node.items;
          if (i >= 0)
            node = node.right;
          else
            break;
        }
        else if (i != size)
          node = node.left;
        else
          goto label_8;
      }
      return node;
label_8:
      return node;
    }

    public T this[int i] => this.findNode(i).item;

    public virtual Speed IndexingSpeed => Speed.Log;

    public int IndexOf(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return this.indexOf(item, out int _);
    }

    private int indexOf(T item, out int upper)
    {
      if (this.isSnapShot)
        throw new NotSupportedException("Indexing not supported for snapshots");
      int num1 = 0;
      TreeBag<T>.Node node = this.root;
      while (node != null)
      {
        int num2 = this.comparer.Compare(item, node.item);
        if (num2 < 0)
        {
          node = node.left;
        }
        else
        {
          int size = node.left == null ? 0 : node.left.size;
          if (num2 == 0)
          {
            upper = num1 + size + node.items - 1;
            return num1 + size;
          }
          num1 = num1 + node.items + size;
          node = node.right;
        }
      }
      upper = ~num1;
      return ~num1;
    }

    public int LastIndexOf(T item)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      int upper;
      this.indexOf(item, out upper);
      return upper;
    }

    public T RemoveAt(int i)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      T obj = this.removeAt(i);
      if (this.ActiveEvents != EventTypeEnum.None)
        this.raiseForRemove(obj);
      return obj;
    }

    private T removeAt(int i)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      if (i < 0 || i >= this.size)
        throw new IndexOutOfRangeException("Index out of range for sequenced collectionvalue");
      while (this.dirs.Length < 2 * this.blackdepth)
      {
        this.dirs = new int[2 * this.dirs.Length];
        this.path = new TreeBag<T>.Node[2 * this.dirs.Length];
      }
      int level = 0;
      TreeBag<T>.Node cursor = this.root;
      while (true)
      {
        int size = cursor.left == null ? 0 : cursor.left.size;
        if (i > size)
        {
          i -= size + cursor.items;
          if (i >= 0)
          {
            this.dirs[level] = -1;
            this.path[level++] = cursor;
            cursor = cursor.right;
          }
          else
            break;
        }
        else if (i != size)
        {
          this.dirs[level] = 1;
          this.path[level++] = cursor;
          cursor = cursor.left;
        }
        else
          break;
      }
      T obj = cursor.item;
      if (cursor.items > 1)
      {
        this.resplicebag(level, cursor);
        --this.size;
        return obj;
      }
      this.removeIterativePhase2(cursor, level);
      return obj;
    }

    private void resplicebag(int level, TreeBag<T>.Node cursor)
    {
      TreeBag<T>.Node.CopyNode(ref cursor, this.maxsnapid, this.generation);
      --cursor.items;
      --cursor.size;
      while (level-- > 0)
      {
        TreeBag<T>.Node child = cursor;
        cursor = this.path[level];
        TreeBag<T>.Node.update(ref cursor, this.dirs[level] > 0, child, this.maxsnapid, this.generation);
        --cursor.size;
        this.path[level] = (TreeBag<T>.Node) null;
      }
    }

    public void RemoveInterval(int start, int count)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      if (start < 0 || count < 0 || start + count > this.size)
        throw new ArgumentOutOfRangeException();
      this.updatecheck();
      if (count == 0)
        return;
      for (int index = 0; index < count; ++index)
        this.removeAt(start);
      if ((this.ActiveEvents & EventTypeEnum.Cleared) != EventTypeEnum.None)
        this.raiseCollectionCleared(false, count);
      if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
        return;
      this.raiseCollectionChanged();
    }

    public IDirectedCollectionValue<T> this[int start, int count]
    {
      get
      {
        this.checkRange(start, count);
        return (IDirectedCollectionValue<T>) new TreeBag<T>.Interval(this, start, count, true);
      }
    }

    public override IDirectedCollectionValue<T> Backwards() => this.RangeAll().Backwards();

    IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
    {
      return (IDirectedEnumerable<T>) this.Backwards();
    }

    public IComparer<T> Comparer => this.comparer;

    public T FindMin()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      if (this.size == 0)
        throw new NoSuchItemException();
      TreeBag<T>.Node n = this.root;
      for (TreeBag<T>.Node node = this.left(n); node != null; node = this.left(n))
        n = node;
      return n.item;
    }

    public T DeleteMin()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException();
      this.stackcheck();
      T obj = this.deleteMin();
      if (this.ActiveEvents != EventTypeEnum.None)
      {
        this.raiseItemsRemoved(obj, 1);
        this.raiseCollectionChanged();
      }
      return obj;
    }

    private T deleteMin()
    {
      int level = 0;
      TreeBag<T>.Node cursor;
      for (cursor = this.root; cursor.left != null; cursor = cursor.left)
      {
        this.dirs[level] = 1;
        this.path[level++] = cursor;
      }
      T obj = cursor.item;
      if (cursor.items > 1)
      {
        this.resplicebag(level, cursor);
        --this.size;
        return obj;
      }
      this.removeIterativePhase2(cursor, level);
      return obj;
    }

    public T FindMax()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      if (this.size == 0)
        throw new NoSuchItemException();
      TreeBag<T>.Node n = this.root;
      for (TreeBag<T>.Node node = this.right(n); node != null; node = this.right(n))
        n = node;
      return n.item;
    }

    public T DeleteMax()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      if (this.size == 0)
        throw new NoSuchItemException();
      this.stackcheck();
      T obj = this.deleteMax();
      if (this.ActiveEvents != EventTypeEnum.None)
      {
        this.raiseItemsRemoved(obj, 1);
        this.raiseCollectionChanged();
      }
      return obj;
    }

    private T deleteMax()
    {
      int level = 0;
      TreeBag<T>.Node cursor;
      for (cursor = this.root; cursor.right != null; cursor = cursor.right)
      {
        this.dirs[level] = -1;
        this.path[level++] = cursor;
      }
      T obj = cursor.item;
      if (cursor.items > 1)
      {
        this.resplicebag(level, cursor);
        --this.size;
        return obj;
      }
      this.removeIterativePhase2(cursor, level);
      return obj;
    }

    public bool TryPredecessor(T item, out T res)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<T>.Node n = this.root;
      TreeBag<T>.Node node = (TreeBag<T>.Node) null;
      while (n != null)
      {
        int num = this.comparer.Compare(n.item, item);
        if (num < 0)
        {
          node = n;
          n = this.right(n);
        }
        else if (num == 0)
        {
          for (n = this.left(n); n != null; n = this.right(n))
            node = n;
        }
        else
          n = this.left(n);
      }
      if (node == null)
      {
        res = default (T);
        return false;
      }
      res = node.item;
      return true;
    }

    public bool TrySuccessor(T item, out T res)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<T>.Node n = this.root;
      TreeBag<T>.Node node = (TreeBag<T>.Node) null;
      while (n != null)
      {
        int num = this.comparer.Compare(n.item, item);
        if (num > 0)
        {
          node = n;
          n = this.left(n);
        }
        else if (num == 0)
        {
          for (n = this.right(n); n != null; n = this.left(n))
            node = n;
        }
        else
          n = this.right(n);
      }
      if (node == null)
      {
        res = default (T);
        return false;
      }
      res = node.item;
      return true;
    }

    public bool TryWeakPredecessor(T item, out T res)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<T>.Node n = this.root;
      TreeBag<T>.Node node = (TreeBag<T>.Node) null;
      while (n != null)
      {
        int num = this.comparer.Compare(n.item, item);
        if (num < 0)
        {
          node = n;
          n = this.right(n);
        }
        else
        {
          if (num == 0)
          {
            res = n.item;
            return true;
          }
          n = this.left(n);
        }
      }
      if (node == null)
      {
        res = default (T);
        return false;
      }
      res = node.item;
      return true;
    }

    public bool TryWeakSuccessor(T item, out T res)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<T>.Node n = this.root;
      TreeBag<T>.Node node = (TreeBag<T>.Node) null;
      while (n != null)
      {
        int num = this.comparer.Compare(n.item, item);
        if (num == 0)
        {
          res = n.item;
          return true;
        }
        if (num > 0)
        {
          node = n;
          n = this.left(n);
        }
        else
          n = this.right(n);
      }
      if (node == null)
      {
        res = default (T);
        return false;
      }
      res = node.item;
      return true;
    }

    public T Predecessor(T item)
    {
      T res;
      if (this.TryPredecessor(item, out res))
        return res;
      throw new NoSuchItemException();
    }

    public T WeakPredecessor(T item)
    {
      T res;
      if (this.TryWeakPredecessor(item, out res))
        return res;
      throw new NoSuchItemException();
    }

    public T Successor(T item)
    {
      T res;
      if (this.TrySuccessor(item, out res))
        return res;
      throw new NoSuchItemException();
    }

    public T WeakSuccessor(T item)
    {
      T res;
      if (this.TryWeakSuccessor(item, out res))
        return res;
      throw new NoSuchItemException();
    }

    public IDirectedCollectionValue<T> RangeFrom(T bot)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return (IDirectedCollectionValue<T>) new TreeBag<T>.Range(this, true, bot, false, default (T), EnumerationDirection.Forwards);
    }

    public IDirectedCollectionValue<T> RangeFromTo(T bot, T top)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return (IDirectedCollectionValue<T>) new TreeBag<T>.Range(this, true, bot, true, top, EnumerationDirection.Forwards);
    }

    public IDirectedCollectionValue<T> RangeTo(T top)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return (IDirectedCollectionValue<T>) new TreeBag<T>.Range(this, false, default (T), true, top, EnumerationDirection.Forwards);
    }

    public IDirectedCollectionValue<T> RangeAll()
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return (IDirectedCollectionValue<T>) new TreeBag<T>.Range(this, false, default (T), false, default (T), EnumerationDirection.Forwards);
    }

    IDirectedEnumerable<T> ISorted<T>.RangeFrom(T bot)
    {
      return (IDirectedEnumerable<T>) this.RangeFrom(bot);
    }

    IDirectedEnumerable<T> ISorted<T>.RangeFromTo(T bot, T top)
    {
      return (IDirectedEnumerable<T>) this.RangeFromTo(bot, top);
    }

    IDirectedEnumerable<T> ISorted<T>.RangeTo(T top) => (IDirectedEnumerable<T>) this.RangeTo(top);

    private int countTo(T item, bool strict)
    {
      if (this.isSnapShot)
        throw new NotSupportedException("Indexing not supported for snapshots");
      int num1 = 0;
      TreeBag<T>.Node node = this.root;
      while (node != null)
      {
        int num2 = this.comparer.Compare(item, node.item);
        if (num2 < 0)
        {
          node = node.left;
        }
        else
        {
          int size = node.left == null ? 0 : node.left.size;
          if (num2 == 0)
            return !strict ? num1 + size + node.items : num1 + size;
          num1 = num1 + node.items + size;
          node = node.right;
        }
      }
      return num1;
    }

    public bool Cut(
      IComparable<T> c,
      out T low,
      out bool lowIsValid,
      out T high,
      out bool highIsValid)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      TreeBag<T>.Node n1 = this.root;
      TreeBag<T>.Node node1 = (TreeBag<T>.Node) null;
      TreeBag<T>.Node node2 = (TreeBag<T>.Node) null;
      bool flag = false;
      while (n1 != null)
      {
        int num = c.CompareTo(n1.item);
        if (num > 0)
        {
          node1 = n1;
          n1 = this.right(n1);
        }
        else if (num < 0)
        {
          node2 = n1;
          n1 = this.left(n1);
        }
        else
        {
          flag = true;
          TreeBag<T>.Node n2 = this.left(n1);
          while (n2 != null && c.CompareTo(n2.item) == 0)
            n2 = this.left(n2);
          if (n2 != null)
          {
            node1 = n2;
            TreeBag<T>.Node n3 = this.right(n2);
            while (n3 != null)
            {
              if (c.CompareTo(n3.item) > 0)
              {
                node1 = n3;
                n3 = this.right(n3);
              }
              else
                n3 = this.left(n3);
            }
          }
          TreeBag<T>.Node n4 = this.right(n1);
          while (n4 != null && c.CompareTo(n4.item) == 0)
            n4 = this.right(n4);
          if (n4 != null)
          {
            node2 = n4;
            TreeBag<T>.Node n5 = this.left(n4);
            while (n5 != null)
            {
              if (c.CompareTo(n5.item) < 0)
              {
                node2 = n5;
                n5 = this.left(n5);
              }
              else
                n5 = this.right(n5);
            }
            break;
          }
          break;
        }
      }
      high = !(highIsValid = node2 != null) ? default (T) : node2.item;
      low = !(lowIsValid = node1 != null) ? default (T) : node1.item;
      return flag;
    }

    public int CountFrom(T bot)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return this.size - this.countTo(bot, true);
    }

    public int CountFromTo(T bot, T top)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return this.comparer.Compare(bot, top) >= 0 ? 0 : this.countTo(top, true) - this.countTo(bot, true);
    }

    public int CountTo(T top)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      return this.countTo(top, true);
    }

    public void RemoveRangeFrom(T low)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      int num = this.CountFrom(low);
      if (num == 0)
        return;
      this.stackcheck();
      CircularQueue<T> wasRemoved = (this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None ? new CircularQueue<T>() : (CircularQueue<T>) null;
      for (int index = 0; index < num; ++index)
      {
        T obj = this.deleteMax();
        wasRemoved?.Enqueue(obj);
      }
      if (wasRemoved != null)
      {
        this.raiseForRemoveAll((ICollectionValue<T>) wasRemoved);
      }
      else
      {
        if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
          return;
        this.raiseCollectionChanged();
      }
    }

    public void RemoveRangeFromTo(T low, T hi)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      int num = this.CountFromTo(low, hi);
      if (num == 0)
        return;
      CircularQueue<T> wasRemoved = (this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None ? new CircularQueue<T>() : (CircularQueue<T>) null;
      for (int index = 0; index < num; ++index)
      {
        T obj = this.Predecessor(hi);
        this.removeIterative(ref obj, false, out int _);
        wasRemoved?.Enqueue(obj);
      }
      if (wasRemoved != null)
      {
        this.raiseForRemoveAll((ICollectionValue<T>) wasRemoved);
      }
      else
      {
        if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
          return;
        this.raiseCollectionChanged();
      }
    }

    public void RemoveRangeTo(T hi)
    {
      if (!this.isValid)
        throw new ViewDisposedException("Snapshot has been disposed");
      this.updatecheck();
      int num = this.CountTo(hi);
      if (num == 0)
        return;
      this.stackcheck();
      CircularQueue<T> wasRemoved = (this.ActiveEvents & EventTypeEnum.Removed) != EventTypeEnum.None ? new CircularQueue<T>() : (CircularQueue<T>) null;
      for (int index = 0; index < num; ++index)
      {
        T obj = this.deleteMin();
        wasRemoved?.Enqueue(obj);
      }
      if (wasRemoved != null)
      {
        this.raiseForRemoveAll((ICollectionValue<T>) wasRemoved);
      }
      else
      {
        if ((this.ActiveEvents & EventTypeEnum.Changed) == EventTypeEnum.None)
          return;
        this.raiseCollectionChanged();
      }
    }

    private int maxsnapid => this.snapList != null ? this.findLastLiveSnapShot() : -1;

    private int findLastLiveSnapShot()
    {
      if (this.snapList == null)
        return -1;
      TreeBag<T>.SnapRef prev = this.snapList.Prev;
      object obj = (object) null;
      while (prev != null && (obj = prev.Tree.Target) == null)
        prev = prev.Prev;
      if (prev == null)
      {
        this.snapList = (TreeBag<T>.SnapRef) null;
        return -1;
      }
      if (this.snapList.Prev != prev)
      {
        this.snapList.Prev = prev;
        prev.Next = this.snapList;
      }
      return ((TreeBag<T>) obj).generation;
    }

    public void Dispose()
    {
      if (!this.isValid)
        return;
      if (this.isSnapShot)
      {
        this.snapList.Dispose();
        this.snapDispose();
      }
      else
      {
        if (this.snapList != null)
        {
          for (TreeBag<T>.SnapRef prev = this.snapList.Prev; prev != null; prev = prev.Prev)
          {
            if (prev.Tree.Target is TreeBag<T> target)
              target.snapDispose();
          }
        }
        this.snapList = (TreeBag<T>.SnapRef) null;
        this.Clear();
      }
    }

    private void snapDispose()
    {
      this.root = (TreeBag<T>.Node) null;
      this.dirs = (int[]) null;
      this.path = (TreeBag<T>.Node[]) null;
      this.comparer = (IComparer<T>) null;
      this.isValid = false;
      this.snapList = (TreeBag<T>.SnapRef) null;
    }

    public ISorted<T> Snapshot()
    {
      if (this.isSnapShot)
        throw new InvalidOperationException("Cannot snapshot a snapshot");
      TreeBag<T> tree = (TreeBag<T>) this.MemberwiseClone();
      TreeBag<T>.SnapRef snapRef = new TreeBag<T>.SnapRef(tree);
      tree.isReadOnlyBase = true;
      tree.isSnapShot = true;
      tree.snapList = snapRef;
      this.findLastLiveSnapShot();
      if (this.snapList == null)
        this.snapList = new TreeBag<T>.SnapRef(this);
      TreeBag<T>.SnapRef prev = this.snapList.Prev;
      snapRef.Prev = prev;
      if (prev != null)
        prev.Next = snapRef;
      snapRef.Next = this.snapList;
      this.snapList.Prev = snapRef;
      ++this.generation;
      return (ISorted<T>) tree;
    }

    private void minidump(TreeBag<T>.Node n, string space)
    {
      if (n == null)
        return;
      this.minidump(n.right, space + "  ");
      Logger.Log(string.Format("{0} {4} (size={1}, items={8}, h={2}, gen={3}, id={6}){7}", (object) (space + (object) n.item), (object) n.size, (object) 0, (object) n.generation, n.red ? (object) "RED" : (object) "BLACK", (object) 0, (object) 0, n.lastgeneration == -1 ? (object) "" : (object) string.Format(" [extra: lg={0}, c={1}, i={2}]", (object) n.lastgeneration, n.leftnode ? (object) "L" : (object) "R", n.oldref == null ? (object) "()" : (object) string.Concat((object) n.oldref.item)), (object) n.items));
      this.minidump(n.left, space + "  ");
    }

    public void dump() => this.dump("");

    public void dump(string msg)
    {
      Logger.Log(string.Format(">>>>>>>>>>>>>>>>>>> dump {0} (count={1}, blackdepth={2}, depth={3}, gen={4}, uniqueCount={5})", (object) msg, (object) this.size, (object) this.blackdepth, (object) 0, (object) this.generation, (object) this.UniqueCount));
      this.minidump(this.root, "");
      this.check("");
      Logger.Log("<<<<<<<<<<<<<<<<<<<");
    }

    private void dump(string msg, string err)
    {
      Logger.Log(string.Format(">>>>>>>>>>>>>>>>>>> dump {0} (count={1}, blackdepth={2}, depth={3}, gen={4}, uniqueCount={5})", (object) msg, (object) this.size, (object) this.blackdepth, (object) 0, (object) this.generation, (object) this.UniqueCount));
      this.minidump(this.root, "");
      Logger.Log(err);
      Logger.Log("<<<<<<<<<<<<<<<<<<<");
    }

    private bool massert(bool b, TreeBag<T>.Node n, string m)
    {
      if (!b)
        Logger.Log(string.Format("*** Node (item={0}, id={1}): {2}", (object) n.item, (object) 0, (object) m));
      return b;
    }

    private bool rbminicheck(
      TreeBag<T>.Node n,
      bool redp,
      out T min,
      out T max,
      out int blackheight)
    {
      bool flag1 = true;
      bool flag2 = this.massert(!n.red || !redp, n, "RED parent of RED node") && flag1;
      bool flag3 = this.massert(n.left == null || n.right != null || n.left.red, n, "Left child black, but right child empty") && flag2;
      bool flag4 = this.massert(n.right == null || n.left != null || n.right.red, n, "Right child black, but left child empty") && flag3;
      bool flag5 = this.massert(n.size == (n.left == null ? 0 : n.left.size) + (n.right == null ? 0 : n.right.size) + n.items, n, "Bad size") && flag4;
      min = max = n.item;
      int blackheight1 = 0;
      int blackheight2 = 0;
      T y;
      if (n.left != null)
      {
        bool flag6 = this.rbminicheck(n.left, n.red, out min, out y, out blackheight1) && flag5;
        flag5 = this.massert(this.comparer.Compare(n.item, y) > 0, n, "Value not > all left children") && flag6;
      }
      if (n.right != null)
      {
        bool flag7 = this.rbminicheck(n.right, n.red, out y, out max, out blackheight2) && flag5;
        flag5 = this.massert(this.comparer.Compare(n.item, y) < 0, n, "Value not < all right children") && flag7;
      }
      bool flag8 = this.massert(blackheight2 == blackheight1, n, "Different blackheights of children") && flag5;
      blackheight = n.red ? blackheight2 : blackheight2 + 1;
      return flag8;
    }

    private bool rbminisnapcheck(TreeBag<T>.Node n, out int size, out T min, out T max)
    {
      bool flag1 = true;
      min = max = n.item;
      int size1 = 0;
      int size2 = 0;
      TreeBag<T>.Node n1 = n.lastgeneration < this.generation || !n.leftnode ? n.left : n.oldref;
      T y;
      if (n1 != null)
      {
        bool flag2 = this.rbminisnapcheck(n1, out size1, out min, out y) && flag1;
        flag1 = this.massert(this.comparer.Compare(n.item, y) > 0, n, "Value not > all left children") && flag2;
      }
      TreeBag<T>.Node n2 = n.lastgeneration < this.generation || n.leftnode ? n.right : n.oldref;
      if (n2 != null)
      {
        bool flag3 = this.rbminisnapcheck(n2, out size2, out y, out max) && flag1;
        flag1 = this.massert(this.comparer.Compare(n.item, y) < 0, n, "Value not < all right children") && flag3;
      }
      size = n.items + size1 + size2;
      return flag1;
    }

    public bool Check(string name)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!this.check(name))
        return true;
      this.dump(name, stringBuilder.ToString());
      return false;
    }

    public bool Check() => !this.isValid || this.Check("-");

    private bool check(string msg)
    {
      if (this.root == null)
        return false;
      T min;
      T max;
      if (this.isSnapShot)
      {
        int size;
        bool flag = this.rbminisnapcheck(this.root, out size, out min, out max);
        return !this.massert(this.size == size, this.root, "bad snapshot size") || !flag;
      }
      int blackheight;
      bool flag1 = this.rbminicheck(this.root, false, out min, out max, out blackheight);
      bool flag2 = this.massert(blackheight == this.blackdepth, this.root, "bad blackh/d") && flag1;
      bool flag3 = this.massert(!this.root.red, this.root, "root is red") && flag2;
      return !this.massert(this.root.size == this.size, this.root, "count!=root.size") || !flag3;
    }

    [Serializable]
    private class Node
    {
      public bool red = true;
      public T item;
      public TreeBag<T>.Node left;
      public TreeBag<T>.Node right;
      public int size = 1;
      public int items = 1;
      public int generation;
      public int lastgeneration = -1;
      public TreeBag<T>.Node oldref;
      public bool leftnode;

      internal static bool update(
        ref TreeBag<T>.Node cursor,
        bool leftnode,
        TreeBag<T>.Node child,
        int maxsnapid,
        int generation)
      {
        TreeBag<T>.Node node = leftnode ? cursor.left : cursor.right;
        if (child == node)
          return false;
        bool flag = false;
        if (cursor.generation <= maxsnapid)
        {
          if (cursor.lastgeneration == -1)
          {
            cursor.leftnode = leftnode;
            cursor.lastgeneration = maxsnapid;
            cursor.oldref = node;
          }
          else if (cursor.leftnode != leftnode || cursor.lastgeneration < maxsnapid)
          {
            TreeBag<T>.Node.CopyNode(ref cursor, maxsnapid, generation);
            flag = true;
          }
        }
        if (leftnode)
          cursor.left = child;
        else
          cursor.right = child;
        return flag;
      }

      public static bool CopyNode(ref TreeBag<T>.Node cursor, int maxsnapid, int generation)
      {
        if (cursor.generation > maxsnapid)
          return false;
        cursor = (TreeBag<T>.Node) cursor.MemberwiseClone();
        cursor.generation = generation;
        cursor.lastgeneration = -1;
        return true;
      }
    }

    [Serializable]
    internal class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
      private TreeBag<T> tree;
      private bool valid;
      private int stamp;
      private T current;
      private TreeBag<T>.Node cursor;
      private TreeBag<T>.Node[] path;
      private int level;
      private bool disposed;

      public Enumerator(TreeBag<T> tree)
      {
        this.tree = tree;
        this.stamp = tree.stamp;
        this.path = new TreeBag<T>.Node[2 * tree.blackdepth];
        this.cursor = new TreeBag<T>.Node();
        this.cursor.right = tree.root;
      }

      public T Current
      {
        get
        {
          if (this.valid)
            return this.current;
          throw new InvalidOperationException();
        }
      }

      public bool MoveNext()
      {
        this.tree.modifycheck(this.stamp);
        if (this.cursor.right != null)
        {
          this.path[this.level] = this.cursor = this.cursor.right;
          while (this.cursor.left != null)
            this.path[++this.level] = this.cursor = this.cursor.left;
        }
        else
        {
          if (this.level == 0)
            return this.valid = false;
          this.cursor = this.path[--this.level];
        }
        this.current = this.cursor.item;
        return this.valid = true;
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (this.disposed)
          return;
        int num = disposing ? 1 : 0;
        this.current = default (T);
        this.cursor = (TreeBag<T>.Node) null;
        this.path = (TreeBag<T>.Node[]) null;
        this.disposed = true;
      }

      ~Enumerator() => this.Dispose(false);

      object IEnumerator.Current => (object) this.Current;

      bool IEnumerator.MoveNext() => this.MoveNext();

      void IEnumerator.Reset() => throw new NotImplementedException();
    }

    [Serializable]
    internal class SnapEnumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
      private TreeBag<T> tree;
      private bool valid;
      private int stamp;
      private int togo;
      private T current;
      private TreeBag<T>.Node cursor;
      private TreeBag<T>.Node[] path;
      private int level;

      public SnapEnumerator(TreeBag<T> tree)
      {
        this.tree = tree;
        this.stamp = tree.stamp;
        this.path = new TreeBag<T>.Node[2 * tree.blackdepth];
        this.cursor = new TreeBag<T>.Node();
        this.cursor.right = tree.root;
      }

      public bool MoveNext()
      {
        this.tree.modifycheck(this.stamp);
        if (--this.togo > 0)
          return true;
        TreeBag<T>.Node node1 = this.tree.right(this.cursor);
        if (node1 != null)
        {
          this.path[this.level] = this.cursor = node1;
          for (TreeBag<T>.Node node2 = this.tree.left(this.cursor); node2 != null; node2 = this.tree.left(this.cursor))
            this.path[++this.level] = this.cursor = node2;
        }
        else
        {
          if (this.level == 0)
            return this.valid = false;
          this.cursor = this.path[--this.level];
        }
        this.togo = this.cursor.items;
        this.current = this.cursor.item;
        return this.valid = true;
      }

      public T Current
      {
        get
        {
          if (this.valid)
            return this.current;
          throw new InvalidOperationException();
        }
      }

      void IDisposable.Dispose()
      {
        this.tree = (TreeBag<T>) null;
        this.valid = false;
        this.current = default (T);
        this.cursor = (TreeBag<T>.Node) null;
        this.path = (TreeBag<T>.Node[]) null;
      }

      object IEnumerator.Current => (object) this.Current;

      bool IEnumerator.MoveNext() => this.MoveNext();

      void IEnumerator.Reset() => throw new NotImplementedException();
    }

    [Serializable]
    private class Multiplicities : 
      CollectionValueBase<KeyValuePair<T, int>>,
      ICollectionValue<KeyValuePair<T, int>>,
      IEnumerable<KeyValuePair<T, int>>,
      IEnumerable,
      IShowable,
      IFormattable
    {
      private TreeBag<T> treebag;
      private int origstamp;

      internal Multiplicities(TreeBag<T> treebag)
      {
        this.treebag = treebag;
        this.origstamp = treebag.stamp;
      }

      public override KeyValuePair<T, int> Choose()
      {
        return new KeyValuePair<T, int>(this.treebag.root.item, this.treebag.root.items);
      }

      public override IEnumerator<KeyValuePair<T, int>> GetEnumerator()
      {
        return this.getEnumerator(this.treebag.root, this.origstamp);
      }

      private IEnumerator<KeyValuePair<T, int>> getEnumerator(TreeBag<T>.Node node, int origstamp)
      {
        if (node != null)
        {
          if (node.left != null)
          {
            IEnumerator<KeyValuePair<T, int>> child = this.getEnumerator(node.left, origstamp);
            while (child.MoveNext())
            {
              this.treebag.modifycheck(origstamp);
              yield return child.Current;
            }
          }
          yield return new KeyValuePair<T, int>(node.item, node.items);
          if (node.right != null)
          {
            IEnumerator<KeyValuePair<T, int>> child = this.getEnumerator(node.right, origstamp);
            while (child.MoveNext())
            {
              this.treebag.modifycheck(origstamp);
              yield return child.Current;
            }
          }
        }
      }

      public override bool IsEmpty => this.treebag.IsEmpty;

      public override int Count
      {
        get
        {
          int count = 0;
          foreach (KeyValuePair<T, int> keyValuePair in (EnumerableBase<KeyValuePair<T, int>>) this)
            ++count;
          return count;
        }
      }

      public override Speed CountSpeed => Speed.Linear;
    }

    [Serializable]
    private class Interval : 
      DirectedCollectionValueBase<T>,
      IDirectedCollectionValue<T>,
      ICollectionValue<T>,
      IShowable,
      IFormattable,
      IDirectedEnumerable<T>,
      IEnumerable<T>,
      IEnumerable
    {
      private readonly int start;
      private readonly int length;
      private readonly int stamp;
      private readonly bool forwards;
      private readonly TreeBag<T> tree;

      internal Interval(TreeBag<T> tree, int start, int count, bool forwards)
      {
        if (tree.isSnapShot)
          throw new NotSupportedException("Indexing not supported for snapshots");
        this.start = start;
        this.length = count;
        this.forwards = forwards;
        this.tree = tree;
        this.stamp = tree.stamp;
      }

      public override bool IsEmpty => this.length == 0;

      public override int Count => this.length;

      public override Speed CountSpeed => Speed.Constant;

      public override T Choose()
      {
        if (this.length == 0)
          throw new NoSuchItemException();
        return this.tree[this.start];
      }

      public override IEnumerator<T> GetEnumerator()
      {
        this.tree.modifycheck(this.stamp);
        TreeBag<T>.Node cursor = this.tree.root;
        TreeBag<T>.Node[] path = new TreeBag<T>.Node[2 * this.tree.blackdepth];
        int level = 0;
        int totaltogo = this.length;
        int togo;
        if (totaltogo != 0)
        {
          if (this.forwards)
          {
            int i = this.start;
            int size;
            while (true)
            {
              size = cursor.left == null ? 0 : cursor.left.size;
              if (i > size)
              {
                if (cursor.items <= i - size)
                {
                  i -= size + cursor.items;
                  cursor = cursor.right;
                }
                else
                  break;
              }
              else if (i != size)
              {
                path[level++] = cursor;
                cursor = cursor.left;
              }
              else
                goto label_10;
            }
            togo = cursor.items - (i - size);
            goto label_12;
label_10:
            togo = cursor.items;
label_12:
            T current = cursor.item;
            while (totaltogo-- > 0)
            {
              yield return current;
              this.tree.modifycheck(this.stamp);
              if (--togo <= 0)
              {
                if (cursor.right != null)
                {
                  path[level] = cursor = cursor.right;
                  while (cursor.left != null)
                    path[++level] = cursor = cursor.left;
                }
                else
                {
                  if (level == 0)
                    break;
                  cursor = path[--level];
                }
                current = cursor.item;
                togo = cursor.items;
              }
            }
          }
          else
          {
            int i = this.start + this.length - 1;
            int size;
            while (true)
            {
              size = cursor.left == null ? 0 : cursor.left.size;
              if (i > size)
              {
                if (i - size >= cursor.items)
                {
                  i -= size + cursor.items;
                  path[level++] = cursor;
                  cursor = cursor.right;
                }
                else
                  break;
              }
              else if (i != size)
                cursor = cursor.left;
              else
                goto label_31;
            }
            togo = i - size + 1;
            goto label_33;
label_31:
            togo = 1;
label_33:
            T current = cursor.item;
            while (totaltogo-- > 0)
            {
              yield return current;
              this.tree.modifycheck(this.stamp);
              if (--togo <= 0)
              {
                if (cursor.left != null)
                {
                  path[level] = cursor = cursor.left;
                  while (cursor.right != null)
                    path[++level] = cursor = cursor.right;
                }
                else
                {
                  if (level == 0)
                    break;
                  cursor = path[--level];
                }
                current = cursor.item;
                togo = cursor.items;
              }
            }
          }
        }
      }

      public override IDirectedCollectionValue<T> Backwards()
      {
        return (IDirectedCollectionValue<T>) new TreeBag<T>.Interval(this.tree, this.start, this.length, !this.forwards);
      }

      IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
      {
        return (IDirectedEnumerable<T>) this.Backwards();
      }

      public override EnumerationDirection Direction
      {
        get => !this.forwards ? EnumerationDirection.Backwards : EnumerationDirection.Forwards;
      }
    }

    [Serializable]
    private class SnapRef
    {
      public TreeBag<T>.SnapRef Prev;
      public TreeBag<T>.SnapRef Next;
      public WeakReference Tree;

      public SnapRef(TreeBag<T> tree) => this.Tree = new WeakReference((object) tree);

      public void Dispose()
      {
        this.Next.Prev = this.Prev;
        if (this.Prev != null)
          this.Prev.Next = this.Next;
        this.Next = this.Prev = (TreeBag<T>.SnapRef) null;
      }
    }

    [Serializable]
    internal class Range : 
      DirectedCollectionValueBase<T>,
      IDirectedCollectionValue<T>,
      ICollectionValue<T>,
      IShowable,
      IFormattable,
      IDirectedEnumerable<T>,
      IEnumerable<T>,
      IEnumerable
    {
      private int stamp;
      private int size;
      private TreeBag<T> basis;
      private T lowend;
      private T highend;
      private bool haslowend;
      private bool hashighend;
      private EnumerationDirection direction;

      public Range(
        TreeBag<T> basis,
        bool haslowend,
        T lowend,
        bool hashighend,
        T highend,
        EnumerationDirection direction)
      {
        this.basis = basis;
        this.stamp = basis.stamp;
        this.lowend = lowend;
        this.highend = highend;
        this.haslowend = haslowend;
        this.hashighend = hashighend;
        this.direction = direction;
        if (basis.isSnapShot)
          return;
        this.size = haslowend ? (hashighend ? basis.CountFromTo(lowend, highend) : basis.CountFrom(lowend)) : (hashighend ? basis.CountTo(highend) : basis.Count);
      }

      public override T Choose()
      {
        if (this.size == 0)
          throw new NoSuchItemException();
        return this.lowend;
      }

      public override IEnumerator<T> GetEnumerator()
      {
        return (IEnumerator<T>) new TreeBag<T>.Range.Enumerator(this);
      }

      public override EnumerationDirection Direction => this.direction;

      private bool inside(T item)
      {
        if (this.haslowend && this.basis.comparer.Compare(item, this.lowend) < 0)
          return false;
        return !this.hashighend || this.basis.comparer.Compare(item, this.highend) < 0;
      }

      private void checkstamp()
      {
        if (this.stamp < this.basis.stamp)
          throw new CollectionModifiedException();
      }

      private void syncstamp() => this.stamp = this.basis.stamp;

      public override IDirectedCollectionValue<T> Backwards()
      {
        TreeBag<T>.Range range = (TreeBag<T>.Range) this.MemberwiseClone();
        range.direction = this.direction == EnumerationDirection.Forwards ? EnumerationDirection.Backwards : EnumerationDirection.Forwards;
        return (IDirectedCollectionValue<T>) range;
      }

      IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards()
      {
        return (IDirectedEnumerable<T>) this.Backwards();
      }

      public override bool IsEmpty => this.size == 0;

      public override int Count => this.size;

      public override Speed CountSpeed => Speed.Constant;

      [Serializable]
      internal class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
      {
        private bool valid;
        private bool ready = true;
        private IComparer<T> comparer;
        private T current;
        private int togo;
        private TreeBag<T>.Node cursor;
        private TreeBag<T>.Node[] path;
        private int level;
        private TreeBag<T>.Range range;
        private bool forwards;

        public Enumerator(TreeBag<T>.Range range)
        {
          this.comparer = range.basis.comparer;
          this.path = new TreeBag<T>.Node[2 * range.basis.blackdepth];
          this.range = range;
          this.forwards = range.direction == EnumerationDirection.Forwards;
          this.cursor = new TreeBag<T>.Node();
          if (this.forwards)
            this.cursor.right = range.basis.root;
          else
            this.cursor.left = range.basis.root;
          range.basis.modifycheck(range.stamp);
        }

        private int compare(T i1, T i2) => this.comparer.Compare(i1, i2);

        public T Current
        {
          get
          {
            if (this.valid)
              return this.current;
            throw new InvalidOperationException();
          }
        }

        public bool MoveNext()
        {
          this.range.basis.modifycheck(this.range.stamp);
          if (!this.ready)
            return false;
          if (--this.togo > 0)
            return true;
          if (this.forwards)
          {
            if (!this.valid && this.range.haslowend)
            {
              this.cursor = this.cursor.right;
              while (this.cursor != null)
              {
                int num = this.compare(this.cursor.item, this.range.lowend);
                if (num > 0)
                {
                  this.path[this.level++] = this.cursor;
                  this.cursor = this.range.basis.left(this.cursor);
                }
                else if (num < 0)
                {
                  this.cursor = this.range.basis.right(this.cursor);
                }
                else
                {
                  this.path[this.level] = this.cursor;
                  break;
                }
              }
              if (this.cursor == null)
              {
                if (this.level == 0)
                  return this.valid = this.ready = false;
                this.cursor = this.path[--this.level];
              }
            }
            else if (this.range.basis.right(this.cursor) != null)
            {
              this.path[this.level] = this.cursor = this.range.basis.right(this.cursor);
              for (TreeBag<T>.Node node = this.range.basis.left(this.cursor); node != null; node = this.range.basis.left(this.cursor))
                this.path[++this.level] = this.cursor = node;
            }
            else
            {
              if (this.level == 0)
                return this.valid = this.ready = false;
              this.cursor = this.path[--this.level];
            }
            this.current = this.cursor.item;
            if (this.range.hashighend && this.compare(this.current, this.range.highend) >= 0)
              return this.valid = this.ready = false;
            this.togo = this.cursor.items;
            return this.valid = true;
          }
          if (!this.valid && this.range.hashighend)
          {
            this.cursor = this.cursor.left;
            while (this.cursor != null)
            {
              if (this.compare(this.cursor.item, this.range.highend) < 0)
              {
                this.path[this.level++] = this.cursor;
                this.cursor = this.range.basis.right(this.cursor);
              }
              else
                this.cursor = this.range.basis.left(this.cursor);
            }
            if (this.cursor == null)
            {
              if (this.level == 0)
                return this.valid = this.ready = false;
              this.cursor = this.path[--this.level];
            }
          }
          else if (this.range.basis.left(this.cursor) != null)
          {
            this.path[this.level] = this.cursor = this.range.basis.left(this.cursor);
            for (TreeBag<T>.Node node = this.range.basis.right(this.cursor); node != null; node = this.range.basis.right(this.cursor))
              this.path[++this.level] = this.cursor = node;
          }
          else
          {
            if (this.level == 0)
              return this.valid = this.ready = false;
            this.cursor = this.path[--this.level];
          }
          this.current = this.cursor.item;
          if (this.range.haslowend && this.compare(this.current, this.range.lowend) < 0)
            return this.valid = this.ready = false;
          this.togo = this.cursor.items;
          return this.valid = true;
        }

        public void Dispose()
        {
          this.comparer = (IComparer<T>) null;
          this.current = default (T);
          this.cursor = (TreeBag<T>.Node) null;
          this.path = (TreeBag<T>.Node[]) null;
          this.range = (TreeBag<T>.Range) null;
        }

        object IEnumerator.Current => (object) this.Current;

        bool IEnumerator.MoveNext() => this.MoveNext();

        void IEnumerator.Reset() => throw new NotImplementedException();
      }
    }
  }
}
