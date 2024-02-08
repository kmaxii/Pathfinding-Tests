using System.Collections.Generic;
using DefaultNamespace;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour.ChineseJPS
{
    public class ChineseJPS{

        private Dictionary<Vector2Int, JpsNode> nodesData;
        private SimplePriorityQueue<JpsNode> nodes;
        private Vector2Int start;
        private Vector2Int end;
        private IGrid data;
        public ChineseJPS(){
            JpsUtils.Init();
            nodesData = new Dictionary<Vector2Int, JpsNode>();
            nodes = new SimplePriorityQueue<JpsNode>();
        }
        public Vector2Int[] Find(IGrid env, Vector2Int start, Vector2Int end){

            this.nodesData.Clear();
            this.nodes.Clear();
            this.data = env;
            this.start = start;
            this.end = end;

            this.nodesData.Add(start, new JpsNode(start, start, new Vector2Int[0], 0));            // Directly add the starting point to the lookup table

            // The starting point is a special jump point, which is the only one to detect in all directions; other jump points have at most three directions
            Vector2Int[] dirs = new Vector2Int[]{
                JpsUtils.Up,
                JpsUtils.Down,
                JpsUtils.Left,
                JpsUtils.Right,
                JpsUtils.UpLeft,
                JpsUtils.UpRight,
                JpsUtils.DownLeft,
                JpsUtils.DownRight,
            };
            JpsNode S = new JpsNode(start, start, dirs, 0);
            nodes.EnqueueWithoutDuplicates(S, 0);

            while(nodes.Count > 0){
                JpsNode node = nodes.Dequeue();
                if(node.pos == this.end)
                    return CompletePath();
                foreach(Vector2Int d in node.direactions){
                    if(JpsUtils.IsLineDireaction(d)){
                        TestLine(node.pos, d, node.cost);
                    }else{
                        TestDiagonal(node.pos, node.pos, d, node.cost);
                    }
                }
            }

            return null;
        }
        public Vector2Int[] CompletePath(){


            
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
            Queue<JpsNode> openSet = new Queue<JpsNode>();
            
            openSet.Enqueue(nodesData[end]);
            while(openSet.Count > 0){
                
                JpsNode node = openSet.Dequeue();
                closedSet.Add(node.pos);
                
                
                foreach(Vector2Int pos in node.parents){
                    if(closedSet.Contains(pos))
                        continue;


                    if (!cameFrom.TryAdd(node.pos, pos))
                    {
                        cameFrom[node.pos] = pos;
                    }
                    if(pos == start)
                        return _trace(cameFrom);
                    openSet.Enqueue(nodesData[pos]);

                }
            }
            return _trace(cameFrom);
        }
        private Vector2Int[] _trace(Dictionary<Vector2Int, Vector2Int> cameFrom){
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int current = end;
            
            
            while(current != start){
                path.Add(current);
                if (!cameFrom.ContainsKey(current))
                    break;
                current = cameFrom[current];
            }
            path.Add(start);
            return path.ToArray();
        }
        private void AddPoint(Vector2Int parent, Vector2Int p, Vector2Int[] dirs, int fcost){
            /* Add a new jump point */

            if(nodesData.ContainsKey(p)){
                nodesData[p].parents.Add(parent);
            }else{
                JpsNode node = new JpsNode(parent, p, dirs, fcost);
                nodesData.Add(p, node);
                nodes.EnqueueWithoutDuplicates(node, fcost + JpsUtils.EucDistance(p, end));
            }
        }
        private void TestDiagonal(Vector2Int parent, Vector2Int p, Vector2Int d, int fcost){
            /* Diagonal jump preparation, mainly to check if jumping is possible and whether jump points should be searched at the current position */
            /* Check if there are no obstacles on both the x and y components, if none, then no need to search for forced neighbors diagonally */

            // Calculate the positions of obstacles 1 and 2
            Vector2Int b1 = new Vector2Int(p.x + d.x, p.y);
            Vector2Int b2 = new Vector2Int(p.x, p.y + d.y);
            if(data.IsMovable(b1)){
                if(data.IsMovable(b2)){
                    /* Case 1, both B1 and B2 are empty, movement is possible and this move does not require checking for diagonal jump points */
                    p += d;
                    if(data.IsMovable(p)){
                        // New position is not an obstacle
                        fcost++;
                        if(p == end){
                            AddPoint(parent, p, null, fcost);
                            return;
                        }
                        if(DiagonalExplore(p, d, fcost)){
                            AddPoint(parent, p, new Vector2Int[]{d}, fcost);
                            return;
                        }
                        TestDiagonal(parent, p, d, fcost);          // Recurse this function
                    }
                }else{
                    // Case 3, b1 is movable while b2 is not
                    p += d;
                    if(data.IsMovable(p)){
                        fcost++;
                        if(p == end){
                            AddPoint(parent, p, null, fcost);
                            return;
                        }
                        List<Vector2Int> dirs = TestForceNeighborsInDiagonal(p, b2, d, Vector2Int.up);
                        if(DiagonalExplore(p, d, fcost) || dirs.Count > 0){
                            dirs.Add(d);
                            AddPoint(parent, p, dirs.ToArray(), fcost);
                            return;
                        }
                        TestDiagonal(parent, p, d, fcost);
                    }
                }
            }else{
                if(data.IsMovable(b2)){
                    // Case 4, b2 is movable while b1 is not

                    p += d;
                    if(data.IsMovable(p)){
                        fcost++;
                        if(p == end){
                            AddPoint(parent, p, null, fcost);
                            return;
                        }
                        List<Vector2Int> dirs = TestForceNeighborsInDiagonal(p, b1, d, Vector2Int.right);
                        if(DiagonalExplore(p, d, fcost) || dirs.Count > 0){
                            dirs.Add(d);
                            AddPoint(parent, p, dirs.ToArray(), fcost);
                            return;
                        }
                        TestDiagonal(parent, p, d, fcost);
                    }
                }else{
                    // Case 2, both are not movable, do nothing
                    // code..
                }
            }
        }
        private List<Vector2Int> TestForceNeighborsInDiagonal(Vector2Int X, Vector2Int B, Vector2Int D, Vector2Int mask){
            /* Check for forced neighbors given the target point and direction, this function is only suitable for diagonal search
           As soon as one side is detected, the function can exit, because only one side is possible
           @X: point X moved to,
           @B: obstacle on the side of point X,
           @D: X - parent
           @mask: direction mask */

            List<Vector2Int> directions = new List<Vector2Int>();
            B += D * mask;
            if(data.IsMovable(B)){
                directions.Add(B - X);
            }
            return directions;
        }
        private bool DiagonalExplore(Vector2Int p, Vector2Int d, int cost){
            /* Explore towards the corner's component directions */
            bool _1 = TestLine(p, new Vector2Int(d.x, 0), cost);
            bool _2 = TestLine(p, new Vector2Int(0, d.y), cost);
            return _1 || _2;
        }
        private bool TestLine(Vector2Int parent, Vector2Int d, int fcost){
            /* Start jumping from point p in the straight line direction d, if a jump point is encountered, return true, otherwise false
           This function assumes that the node parent has been visited */

            Vector2Int p = parent + d;
            while(data.IsMovable(p)){
                if(p == end){
                    /* When the end point is found, add it to the open set */
                    AddPoint(parent, p, new Vector2Int[0], 0);
                    return true;
                }
                fcost++;
                List<Vector2Int> directions = TestForceNeighborsInLine(p, d);
                if(directions.Count > 0){
                    directions.Add(d);
                    AddPoint(parent, p, directions.ToArray(), fcost);
                    return true;
                }
                p += d;
            }
            return false;
        }
        private List<Vector2Int> TestForceNeighborsInLine(Vector2Int p, Vector2Int d){
            /* Check for forced neighbors given the target point and direction, this function is only suitable for horizontal and vertical search
           @p: point X
           @d: direction PX, P being X's parent node */

            List<Vector2Int> directions = new List<Vector2Int>();
            foreach(Vector2Int vd in JpsUtils.verticalDirLut[d]){
                Vector2Int blockPt = vd + p;
                if(data.IsWall(blockPt) && data.IsMovable(blockPt + d))directions.Add(vd + d);
            }
            return directions;
        }
    }
}
