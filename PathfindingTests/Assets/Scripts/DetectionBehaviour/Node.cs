/*!
@file BaseGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
        <http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief BaseGrid Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the BaseGrid Class.

*/

using System;

public class Node : IComparable<Node>
{
    public int x;
    public int y;
    public bool walkable;
    public float heuristicStartToEndLen; // which passes current node
    public float startToCurNodeLen;
    public float? heuristicCurNodeToEndLen;
    public bool isOpened;
    public bool isClosed;
    public Node parent;

    public Node(int iX, int iY, bool? iWalkable = null)
    {
        this.x = iX;
        this.y = iY;
        this.walkable = (iWalkable.HasValue ? iWalkable.Value : false);
        this.heuristicStartToEndLen = 0;
        this.startToCurNodeLen = 0;
        // this must be initialized as null to verify that its value never initialized
        // 0 is not good candidate!!
        this.heuristicCurNodeToEndLen = null;
        this.isOpened = false;
        this.isClosed = false;
        this.parent = null;
    }

    public int CompareTo(Node iObj)
    {
        float result = this.heuristicStartToEndLen - iObj.heuristicStartToEndLen;
        if (result > 0.0f)
            return 1;
        else if (result == 0.0f)
            return 0;
        return -1;
    }


    public override int GetHashCode()
    {
        return x ^ y;
    }

    public override bool Equals(Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Node p = obj as Node;
        if ((Object) p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }
    
    public static bool operator ==(Node a, Node b)
    {
        // If both are null, or both are same instance, return true.
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object) a == null) || ((object) b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Node a, Node b)
    {
        return !(a == b);
    }
}