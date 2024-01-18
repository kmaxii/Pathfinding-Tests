using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DetectionBehaviour
{
    
    [CreateAssetMenu(menuName = "Custom/Pathfinding/Astar")]
    public class AStar : DetectionBehaviour
    {
        private readonly List<PathNode> _open = new List<PathNode>();
        private readonly Dictionary<Transform, PathNode> _openDictionary = new Dictionary<Transform, PathNode>();
        private readonly Dictionary<Transform, PathNode> _closed  = new Dictionary<Transform, PathNode>();

        private Transform _start;
        private Transform _goal;
        private SphereTransforms _sphereTransforms;

        [Header("Neighbours")]
        [SerializeField] private bool colorNeighbours;
        [SerializeField] private Color neighbourColor = Color.yellow;
        
        [Header("Text")]
        [SerializeField] private bool showText;
        
        [Header("Searched nodes")]
        [SerializeField] private bool colorSearchedNodes;
        [SerializeField] private Color searchedNodesColor = Color.red;


        public override LinkedList<Transform> GetShortestPath(Transform start, Transform end, SphereTransforms sphereTransforms)
        {
            _start = start;
            _goal = end;
            _open.Clear();
            _closed.Clear();
            _openDictionary.Clear();
            _sphereTransforms = sphereTransforms;

            
            return GetPath();
        }

        private LinkedList<Transform> GetPath()
        {
            RunAlgorithm();

            LinkedList<Transform> bestPath = new LinkedList<Transform>();

            Transform previous = _goal;
            while (true)
            {
                PathNode node = _closed[previous];
                bestPath.AddLast(node.TilePosition);

                if (node.Previous == null)
                {
                    break;
                }
                previous = node.Previous.TilePosition;
            }

            return bestPath;
        }


        private void RunAlgorithm()
        {
            AddStartNode();

            while (_open.Count > 0)
            {
                PathNode bestNode = GetBestNode();
                MoveToClosed(bestNode);
                if (colorSearchedNodes)
                    bestNode.TilePosition.GetComponent<SpriteRenderer>().color = searchedNodesColor;


                if (bestNode.TilePosition == _goal)
                    break;

                CheckNeighbours(bestNode);
            }
        }
        
        private PathNode GetBestNode()
        {
            PathNode bestNode = _open[0];
            for (var i = 1; i < _open.Count; i++)
            {
                if (_open[i].EstimatedCost < bestNode.EstimatedCost)
                {
                    bestNode = _open[i];
                }
            }
            
            return bestNode;
        }


        private void MoveToClosed(PathNode node)
        {
            _open.Remove(node);
            _openDictionary.Remove(node.TilePosition);
            _closed.Add(node.TilePosition, node);

        }
        private void CheckNeighbours(PathNode node)
        {

            foreach (var neighbour in GetNeighbours(node.TilePosition))
            {
                float distance = GetDistance(neighbour.position, node.TilePosition.position);

                //This node neighbour node has already been checked
                if (_closed.ContainsKey(neighbour))
                {
                    PathNode closedNeighbour = _closed[neighbour];

                    //If this way to the node is faster then the current recorded one
                    if (node.DistanceTraveled + distance < closedNeighbour.DistanceTraveled)
                    {
                        closedNeighbour.DistanceTraveled = node.DistanceTraveled + distance;
                        closedNeighbour.Previous = node;
                        
                        MoveFromClosedToOpen(closedNeighbour);
                    }

                    continue;
                }

                if (!_openDictionary.ContainsKey(neighbour))
                {
                    PathNode toAdd = new PathNode(
                        neighbour, 
                        node.DistanceTraveled + distance, 
                        GetHeuristic(neighbour), node);
                    _open.Add(toAdd);
                    _openDictionary.Add(neighbour, toAdd);
                    
                    if (colorNeighbours)
                        neighbour.GetComponent<SpriteRenderer>().color = neighbourColor;

                    if (showText)
                    {
                        foreach (var sphere1 in _sphereTransforms.AllColliders.Where(sphere =>
                            sphere.transform == neighbour))
                        {
                            if (sphere1.textMeshPro.text == "")
                            {
                                sphere1.textMeshPro.text =
                                    $"G: {toAdd.DistanceTraveled}, H: {GetHeuristic(neighbour)}, F: {toAdd.EstimatedCost}";
                            }
                        }
                    }
                    continue;
                }

                PathNode neighbourNode = _openDictionary[neighbour];

                //If this way to the node is faster then the current recorded one
                if (node.DistanceTraveled + distance < neighbourNode.DistanceTraveled)
                {
                    neighbourNode.DistanceTraveled = node.DistanceTraveled + distance;
                    neighbourNode.Previous = node;
                }
            }

        }
        
        


        private void MoveFromClosedToOpen(PathNode node)
        {
            _closed.Remove(node.TilePosition);
            _open.Add(node);
            _openDictionary.Add(node.TilePosition, node);
        }
        

        private void AddStartNode()
        {
            PathNode startNode = new PathNode(_start, 0, GetHeuristic(_start), null);

            _open.Add(startNode);
            _openDictionary.Add(_start, startNode);
        }
        



        

        
        private float GetHeuristic(Transform position)
        {
             return GetDistance(_goal.position, position.position);
        }


    }
}