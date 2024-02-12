using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/BiDirectionalJumpPointSearch")]
    public class BiDirectionalJumpPointSearch : JumpPointSearch
    {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var nodesStart = new Dictionary<Vector2Int, Node>();
            var nodesEnd = new Dictionary<Vector2Int, Node>();
            var frontierStart = new SimplePriorityQueue<Vector2Int>();
            var frontierEnd = new SimplePriorityQueue<Vector2Int>();

            int explored = 0;

            Node startNode = new Node(start.x, start.y, true);
            Node endNode = new Node(end.x, end.y, true);
            nodesStart[start] = startNode;
            nodesEnd[end] = endNode;
            frontierStart.Enqueue(start, 0);
            frontierEnd.Enqueue(end, 0);
            startNode.isOpened = true;
            endNode.isOpened = true;

            while (frontierStart.Count > 0 && frontierEnd.Count > 0)
            {
                explored++;
                explored++;

                var posFromStart = frontierStart.Dequeue();
                var posFromEnd = frontierEnd.Dequeue();

                Node currentStart = nodesStart[posFromStart];
                Node currentEnd = nodesEnd[posFromEnd];

                currentEnd.isClosed = true;
                currentStart.isClosed = true;

                if (visualize)
                {
                    color1.Raise(posFromStart);
                    color2.Raise(posFromEnd);
                }

                if (nodesStart.ContainsKey(posFromEnd))
                {
                    return (BuildBidirectionalPath(nodesStart, nodesEnd, start, end, posFromEnd), explored);
                }

                if (nodesEnd.ContainsKey(posFromStart))
                {
                    return (BuildBidirectionalPath(nodesStart, nodesEnd, start, end, posFromStart), explored);
                }


                IdentifySuccessors(posFromStart, frontierStart, nodesStart);
                IdentifySuccessors(posFromEnd, frontierEnd, nodesEnd);
            }


            return (new LinkedList<Vector2Int>(), explored);
        }


        private static LinkedList<Vector2Int> BuildBidirectionalPath(Dictionary<Vector2Int, Node> nodesStart,
            Dictionary<Vector2Int, Node> nodesEnd,
            Vector2Int start,
            Vector2Int end,
            Vector2Int meetingPos)
        {
            var path = new LinkedList<Vector2Int>();

            // Trace path from start to meeting point
            var currentNode = nodesStart[meetingPos];
            while (currentNode != null)
            {
                path.AddFirst(new Vector2Int(currentNode.x, currentNode.y));
                currentNode = currentNode.parent;
            }

            // Trace path from meeting point to end (excluding meeting point itself)
            currentNode = nodesEnd[meetingPos].parent;
            while (currentNode != null)
            {
                path.AddLast(new Vector2Int(currentNode.x, currentNode.y));
                currentNode = currentNode.parent;
            }

            return path;
        }
    }
}