/*!
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
        <http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Jump Point Search Algorithm Interface
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

An Interface for the Jump Point Search Algorithm Class.

*/

using System;
using System.Collections.Generic;
using C5;
using EpPathFinding.cs;
using PathFinder.Grid;

namespace PathFinder
{
    public class JumpPointParam : ParamBase
    {
        public JumpPointParam(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos,
            DiagonalMovement iDiagonalMovement = DiagonalMovement.Always, HeuristicMode iMode = HeuristicMode.EUCLIDEAN)
            : base(iGrid, iStartPos, iEndPos, iDiagonalMovement, iMode)
        {
            openList = new IntervalHeap<Node>();
        }

        internal override void _reset(GridPos iStartPos, GridPos iEndPos, BaseGrid iSearchGrid = null)
        {
            openList = new IntervalHeap<Node>();
        }


        //public List<Node> openList;
        public IntervalHeap<Node> openList;
    }

    public class JumpPointFinder
    {
        public static List<GridPos> FindPath(JumpPointParam iParam)
        {
            IntervalHeap<Node> tOpenList = iParam.openList;
            Node tStartNode = iParam.StartNode;
            Node tEndNode = iParam.EndNode;
            Node tNode;

            // set the `g` and `f` value of the start node to be 0
            tStartNode.startToCurNodeLen = 0;
            tStartNode.heuristicStartToEndLen = 0;

            // push the start node into the open list
            tOpenList.Add(tStartNode);
            tStartNode.isOpened = true;


            // while the open list is not empty
            while (tOpenList.Count > 0)
            {
                // pop the position of node which has the minimum `f` value.
                tNode = tOpenList.DeleteMin();
                tNode.isClosed = true;

                if (tNode.Equals(tEndNode))
                {
                    return Node.Backtrace(tNode); // rebuilding path
                }

                IdentifySuccessors(iParam, tNode);
            }


            // fail to find the path
            return new List<GridPos>();
        }

        private static void IdentifySuccessors(JumpPointParam iParam, Node iNode)
        {
            HeuristicDelegate tHeuristic = iParam.HeuristicFunc;
            IntervalHeap<Node> tOpenList = iParam.openList;
            int tEndX = iParam.EndNode.x;
            int tEndY = iParam.EndNode.y;
            GridPos tNeighbor;
            GridPos tJumpPoint;
            Node tJumpNode;

            List<GridPos> tNeighbors = FindNeighbors(iParam, iNode);
            for (int i = 0; i < tNeighbors.Count; i++)
            {
                tNeighbor = tNeighbors[i];

                tJumpPoint = Jump(iParam, tNeighbor.x, tNeighbor.y, iNode.x, iNode.y);

                if (tJumpPoint == null) continue;

                tJumpNode = iParam.SearchGrid.GetNodeAt(tJumpPoint.x, tJumpPoint.y);
                if (tJumpNode == null)
                {
                    if (iParam.EndNode.x == tJumpPoint.x && iParam.EndNode.y == tJumpPoint.y)
                        tJumpNode = iParam.SearchGrid.GetNodeAt(tJumpPoint);
                }

                if (tJumpNode.isClosed)
                {
                    continue;
                }

                // include distance, as parent may not be immediately adjacent:
                float tCurNodeToJumpNodeLen =
                    tHeuristic(Math.Abs(tJumpPoint.x - iNode.x), Math.Abs(tJumpPoint.y - iNode.y));
                float tStartToJumpNodeLen =
                    iNode.startToCurNodeLen + tCurNodeToJumpNodeLen; // next `startToCurNodeLen` value

                if (tJumpNode.isOpened && !(tStartToJumpNodeLen < tJumpNode.startToCurNodeLen)) continue;

                tJumpNode.startToCurNodeLen = tStartToJumpNodeLen;
                tJumpNode.heuristicCurNodeToEndLen = (tJumpNode.heuristicCurNodeToEndLen == null
                    ? tHeuristic(Math.Abs(tJumpPoint.x - tEndX), Math.Abs(tJumpPoint.y - tEndY))
                    : tJumpNode.heuristicCurNodeToEndLen);
                tJumpNode.heuristicStartToEndLen =
                    tJumpNode.startToCurNodeLen + tJumpNode.heuristicCurNodeToEndLen.Value;
                tJumpNode.parent = iNode;

                if (!tJumpNode.isOpened)
                {
                    tOpenList.Add(tJumpNode);
                    tJumpNode.isOpened = true;
                }
            }
        }


        private static GridPos Jump(JumpPointParam iParam, int iX, int iY, int iPx, int iPy)
        {
            if (!iParam.SearchGrid.IsWalkableAt(iX, iY))
            {
                return null;
            }
            else if (iParam.SearchGrid.GetNodeAt(iX, iY).Equals(iParam.EndNode))
            {
                return new GridPos(iX, iY);
            }

            int tDx = iX - iPx;
            int tDy = iY - iPy;
            // check for forced neighbors
            // along the diagonal
            if (tDx != 0 && tDy != 0)
            {
                if ((iParam.SearchGrid.IsWalkableAt(iX - tDx, iY + tDy) &&
                     !iParam.SearchGrid.IsWalkableAt(iX - tDx, iY)) ||
                    (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY - tDy) &&
                     !iParam.SearchGrid.IsWalkableAt(iX, iY - tDy)))
                {
                    return new GridPos(iX, iY);
                }
            }
            // horizontally/vertically
            else
            {
                if (tDx != 0)
                {
                    // moving along x
                    if ((iParam.SearchGrid.IsWalkableAt(iX + tDx, iY + 1) &&
                         !iParam.SearchGrid.IsWalkableAt(iX, iY + 1)) ||
                        (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY - 1) &&
                         !iParam.SearchGrid.IsWalkableAt(iX, iY - 1)))
                    {
                        return new GridPos(iX, iY);
                    }
                }
                else
                {
                    if ((iParam.SearchGrid.IsWalkableAt(iX + 1, iY + tDy) &&
                         !iParam.SearchGrid.IsWalkableAt(iX + 1, iY)) ||
                        (iParam.SearchGrid.IsWalkableAt(iX - 1, iY + tDy) &&
                         !iParam.SearchGrid.IsWalkableAt(iX - 1, iY)))
                    {
                        return new GridPos(iX, iY);
                    }
                }
            }

            // when moving diagonally, must check for vertical/horizontal jump points
            if (tDx != 0 && tDy != 0)
            {
                if (Jump(iParam, iX + tDx, iY, iX, iY) != null)
                {
                    return new GridPos(iX, iY);
                }

                if (Jump(iParam, iX, iY + tDy, iX, iY) != null)
                {
                    return new GridPos(iX, iY);
                }
            }

            // moving diagonally, must make sure one of the vertical/horizontal
            // neighbors is open to allow the path
            if (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY) || iParam.SearchGrid.IsWalkableAt(iX, iY + tDy))
            {
                return Jump(iParam, iX + tDx, iY + tDy, iX, iY);
            }
            else if (iParam.DiagonalMovement == DiagonalMovement.Always)
            {
                return Jump(iParam, iX + tDx, iY + tDy, iX, iY);
            }
            else
            {
                return null;
            }
        }

        private static List<GridPos> FindNeighbors(JumpPointParam iParam, Node iNode)
        {
            Node tParent = (Node) iNode.parent;
            int tX = iNode.x;
            int tY = iNode.y;
            int tPx, tPy, tDx, tDy;
            List<GridPos> tNeighbors = new List<GridPos>();
            List<Node> tNeighborNodes;
            Node tNeighborNode;

            // directed pruning: can ignore most neighbors, unless forced.
            if (tParent != null)
            {
                tPx = tParent.x;
                tPy = tParent.y;
                // get the normalized direction of travel
                tDx = (tX - tPx) / Math.Max(Math.Abs(tX - tPx), 1);
                tDy = (tY - tPy) / Math.Max(Math.Abs(tY - tPy), 1);

                // search diagonally
                if (tDx != 0 && tDy != 0)
                {
                    if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                    {
                        tNeighbors.Add(new GridPos(tX, tY + tDy));
                    }

                    if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                    {
                        tNeighbors.Add(new GridPos(tX + tDx, tY));
                    }

                    if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + tDy))
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy) ||
                            iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
                        }
                    }

                    if (iParam.SearchGrid.IsWalkableAt(tX - tDx, tY + tDy) &&
                        !iParam.SearchGrid.IsWalkableAt(tX - tDx, tY))
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                        {
                            tNeighbors.Add(new GridPos(tX - tDx, tY + tDy));
                        }
                    }

                    if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - tDy) &&
                        !iParam.SearchGrid.IsWalkableAt(tX, tY - tDy))
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            tNeighbors.Add(new GridPos(tX + tDx, tY - tDy));
                        }
                    }
                }
                // search horizontally/vertically
                else
                {
                    if (tDx != 0)
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            tNeighbors.Add(new GridPos(tX + tDx, tY));

                            if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) &&
                                !iParam.SearchGrid.IsWalkableAt(tX, tY + 1))
                            {
                                tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
                            }

                            if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) &&
                                !iParam.SearchGrid.IsWalkableAt(tX, tY - 1))
                            {
                                tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
                            }
                        }
                    }
                    else
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                        {
                            tNeighbors.Add(new GridPos(tX, tY + tDy));

                            if (iParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) &&
                                !iParam.SearchGrid.IsWalkableAt(tX + 1, tY))
                            {
                                tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
                            }

                            if (iParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) &&
                                !iParam.SearchGrid.IsWalkableAt(tX - 1, tY))
                            {
                                tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
                            }
                        }
                        else if (iParam.DiagonalMovement == DiagonalMovement.Always)
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) &&
                                !iParam.SearchGrid.IsWalkableAt(tX + 1, tY))
                            {
                                tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
                            }

                            if (iParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) &&
                                !iParam.SearchGrid.IsWalkableAt(tX - 1, tY))
                            {
                                tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
                            }
                        }
                    }
                }
            }
            // return all neighbors if the node doesnt have a parent (aka is the start node)
            else
            {
                tNeighborNodes = iParam.SearchGrid.GetNeighbors(iNode, iParam.DiagonalMovement);
                for (int i = 0; i < tNeighborNodes.Count; i++)
                {
                    tNeighborNode = tNeighborNodes[i];
                    tNeighbors.Add(new GridPos(tNeighborNode.x, tNeighborNode.y));
                }
            }

            return tNeighbors;
        }
    }
}