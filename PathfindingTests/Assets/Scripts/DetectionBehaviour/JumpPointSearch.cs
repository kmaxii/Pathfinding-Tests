using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/JumpPointSearch")]
    public class JumpPointSearch : DetectionBehaviour
    {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var nodes = new Dictionary<Vector2Int, Node>();
            var frontier = new SimplePriorityQueue <Vector2Int>();

            int explored = 0;

            Node startNode = new Node(start.x, start.y, true);
            nodes[start] = startNode;
            frontier.Enqueue(start, 0);
            startNode.isOpened = true;

            while (frontier.Count > 0)
            {
                explored++;
                var currentPos = frontier.Dequeue();
                
                Node currentNode = nodes[currentPos];
                currentNode.isClosed = true;
                
                if (visualize)
                {
                    color1.Raise(currentPos);
                }

                if (currentPos.Equals(end))
                    break;
                
                IdentifySuccessors(currentPos, frontier, nodes);
            }
            return (BuildPath(nodes, start, end), explored);
        }
        
        private static LinkedList<Vector2Int> BuildPath(Dictionary<Vector2Int, Node> nodes, Vector2Int start, Vector2Int end)
        {
            var path = new LinkedList<Vector2Int>();
            var current = end;

            if (!nodes.ContainsKey(current))
                return path; // No path exists

            while (!current.Equals(start))
            {
                path.AddFirst(current);

                Node parent = nodes[current].parent;
                Vector2Int next = new Vector2Int(parent.x, parent.y);

                // Add intermediate nodes if necessary
                AddIntermediateNodes(path, current, next);

                current = next;
            }

            path.AddFirst(start); // Add the start node

            return path;
        }
        
        
        private static void AddIntermediateNodes(LinkedList<Vector2Int> path, Vector2Int from, Vector2Int to)
        {
            // Calculate differences in both axes
            int dx = Math.Sign(to.x - from.x);
            int dy = Math.Sign(to.y - from.y);

            // Move diagonally
            Vector2Int current = from;
            while (current.x != to.x || current.y != to.y)
            {
                if (current.x != to.x) current.x += dx;
                if (current.y != to.y) current.y += dy;

                // Add the node before reaching 'to' to include all intermediate nodes
                if (current != to)
                {
                    path.AddFirst(current);
                }
            }
        }
         protected void IdentifySuccessors(Vector2Int iPos, 
             SimplePriorityQueue <Vector2Int> frontier,
             Dictionary<Vector2Int, Node> nodes)
         {
             int tEndX = mapData.endPos.x;
             int tEndY = mapData.endPos.y;
             
             Node iNode = nodes[iPos];

            var tNeighbors = FindNeighbors(iNode);
            foreach (var tNeighbor in tNeighbors)
            {

                var jumpResult = Jump(tNeighbor.x, tNeighbor.y, iNode.x, iNode.y);

                if (jumpResult == null) continue;

                Vector2Int tJumpPoint = jumpResult.Value;

                if (!nodes.TryGetValue(tJumpPoint, out var tJumpNode))
                {
                    tJumpNode = new Node(tJumpPoint.x, tJumpPoint.y, true);
                    nodes[tJumpPoint] = tJumpNode;
                }

                if (tJumpNode.isClosed)
                {
                    continue;
                }

                float tCurNodeToJumpNodeLen =
                    Euclidean(Math.Abs(tJumpPoint.x - iNode.x), Math.Abs(tJumpPoint.y - iNode.y));
                float tStartToJumpNodeLen =
                    iNode.startToCurNodeLen + tCurNodeToJumpNodeLen; 

                if (tJumpNode.isOpened && !(tStartToJumpNodeLen < tJumpNode.startToCurNodeLen)) continue;

                tJumpNode.startToCurNodeLen = tStartToJumpNodeLen;
                tJumpNode.heuristicCurNodeToEndLen ??= Euclidean(Math.Abs(tJumpPoint.x - tEndX), Math.Abs(tJumpPoint.y - tEndY));
                tJumpNode.heuristicStartToEndLen =
                    tJumpNode.startToCurNodeLen + tJumpNode.heuristicCurNodeToEndLen.Value;
                tJumpNode.parent = iNode;

                if (!tJumpNode.isOpened)
                {
                    frontier.EnqueueWithoutDuplicates(new Vector2Int(tJumpNode.x, tJumpNode.y), 
                        tJumpNode.heuristicStartToEndLen);
                    
                    tJumpNode.isOpened = true;
                }
            }
        }



        private Vector2Int? Jump(int iX, int iY, int iPx, int iPy)
        {
            if (!mapData.CheckCoordinate(iX, iY))
            {
                return null;
            }
            else if (iX == mapData.endPos.x && iY == mapData.endPos.y)
            {
                return mapData.endPos;
            }

            int tDx = iX - iPx;
            int tDy = iY - iPy;
            // check for forced neighbors
            // along the diagonal
            if (tDx != 0 && tDy != 0)
            {
                if ((mapData.CheckCoordinate(iX - tDx, iY + tDy) &&
                     !mapData.CheckCoordinate(iX - tDx, iY)) ||
                    (mapData.CheckCoordinate(iX + tDx, iY - tDy) &&
                     !mapData.CheckCoordinate(iX, iY - tDy)))
                {
                    return new Vector2Int(iX, iY);
                }
            }
            // horizontally/vertically
            else
            {
                if (tDx != 0)
                {
                    // moving along x
                    if ((mapData.CheckCoordinate(iX + tDx, iY + 1) &&
                         !mapData.CheckCoordinate(iX, iY + 1)) ||
                        (mapData.CheckCoordinate(iX + tDx, iY - 1) &&
                         !mapData.CheckCoordinate(iX, iY - 1)))
                    {
                        return new Vector2Int(iX, iY);
                    }
                }
                else
                {
                    if ((mapData.CheckCoordinate(iX + 1, iY + tDy) &&
                         !mapData.CheckCoordinate(iX + 1, iY)) ||
                        (mapData.CheckCoordinate(iX - 1, iY + tDy) &&
                         !mapData.CheckCoordinate(iX - 1, iY)))
                    {
                        return new Vector2Int(iX, iY);
                    }
                }
            }

            // when moving diagonally, must check for vertical/horizontal jump points
            if (tDx != 0 && tDy != 0)
            {
                if (Jump(iX + tDx, iY, iX, iY) != null)
                {
                    return new Vector2Int(iX, iY);
                }

                if (Jump(iX, iY + tDy, iX, iY) != null)
                {
                    return new Vector2Int(iX, iY);
                }
            }

            // moving diagonally, must make sure one of the vertical/horizontal
            // neighbors is open to allow the path
            if (mapData.CheckCoordinate(iX + tDx, iY) || mapData.CheckCoordinate(iX, iY + tDy))
            {
                return Jump(iX + tDx, iY + tDy, iX, iY);
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<Vector2Int> FindNeighbors(Node iNode)
        {
  
            int tX = iNode.x;
            int tY = iNode.y;

            if (iNode.parent == null)
            {
                return GetNeighbours(new Vector2Int(iNode.x, iNode.y));
            }

            Node tParent = iNode.parent;
            
            List<Vector2Int> tNeighbors = new();

            // directed pruning: can ignore most neighbors, unless forced.
            var tPx = tParent.x;
            var tPy = tParent.y;
            // get the normalized direction of travel
            var tDx = (tX - tPx) / Math.Max(Math.Abs(tX - tPx), 1);
            var tDy = (tY - tPy) / Math.Max(Math.Abs(tY - tPy), 1);

            // search diagonally
            if (tDx != 0 && tDy != 0)
            {
                if (mapData.CheckCoordinate(tX, tY + tDy))
                {
                    tNeighbors.Add(new Vector2Int(tX, tY + tDy));
                }

                if (mapData.CheckCoordinate(tX + tDx, tY))
                {
                    tNeighbors.Add(new Vector2Int(tX + tDx, tY));
                }

                if (mapData.CheckCoordinate(tX + tDx, tY + tDy))
                {
                    if (mapData.CheckCoordinate(tX, tY + tDy) ||
                        mapData.CheckCoordinate(tX + tDx, tY))
                    {
                        tNeighbors.Add(new Vector2Int(tX + tDx, tY + tDy));
                    }
                }

                if (mapData.CheckCoordinate(tX - tDx, tY + tDy) &&
                    !mapData.CheckCoordinate(tX - tDx, tY))
                {
                    if (mapData.CheckCoordinate(tX, tY + tDy))
                    {
                        tNeighbors.Add(new Vector2Int(tX - tDx, tY + tDy));
                    }
                }

                if (mapData.CheckCoordinate(tX + tDx, tY - tDy) &&
                    !mapData.CheckCoordinate(tX, tY - tDy))
                {
                    if (mapData.CheckCoordinate(tX + tDx, tY))
                    {
                        tNeighbors.Add(new Vector2Int(tX + tDx, tY - tDy));
                    }
                }
            }
            // search horizontally/vertically
            else
            {
                if (tDx != 0)
                {
                    if (mapData.CheckCoordinate(tX + tDx, tY))
                    {
                        tNeighbors.Add(new Vector2Int(tX + tDx, tY));

                        if (mapData.CheckCoordinate(tX + tDx, tY + 1) &&
                            !mapData.CheckCoordinate(tX, tY + 1))
                        {
                            tNeighbors.Add(new Vector2Int(tX + tDx, tY + 1));
                        }

                        if (mapData.CheckCoordinate(tX + tDx, tY - 1) &&
                            !mapData.CheckCoordinate(tX, tY - 1))
                        {
                            tNeighbors.Add(new Vector2Int(tX + tDx, tY - 1));
                        }
                    }
                }
                else
                {
                    if (mapData.CheckCoordinate(tX, tY + tDy))
                    {
                        tNeighbors.Add(new Vector2Int(tX, tY + tDy));

                        if (mapData.CheckCoordinate(tX + 1, tY + tDy) &&
                            !mapData.CheckCoordinate(tX + 1, tY))
                        {
                            tNeighbors.Add(new Vector2Int(tX + 1, tY + tDy));
                        }

                        if (mapData.CheckCoordinate(tX - 1, tY + tDy) &&
                            !mapData.CheckCoordinate(tX - 1, tY))
                        {
                            tNeighbors.Add(new Vector2Int(tX - 1, tY + tDy));
                        }
                    }
                }
            }


            return tNeighbors;
        }

        private static float Euclidean(int iDx, int iDy)
        {
            float tFdx = iDx;
            float tFdy = iDy;
            return (float)Math.Sqrt(tFdx * tFdx + tFdy * tFdy);
        }
    }
}