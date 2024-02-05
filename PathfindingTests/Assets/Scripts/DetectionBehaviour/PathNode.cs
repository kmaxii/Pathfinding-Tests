using System;
using UnityEngine;

namespace DetectionBehaviour
{
    public class PathNode  : IComparable<PathNode>
    {

        public readonly Vector2Int TilePosition;
        public float DistanceTraveled;
        private readonly float _estimatedDistanceRemaining;
        public PathNode Previous;

        public float EstimatedCost => _estimatedDistanceRemaining + DistanceTraveled;

        public PathNode(Vector2Int tilePosition, float distanceTraveled, float estimatedDistanceRemaining, PathNode previous)
        {
            TilePosition = tilePosition;
            DistanceTraveled = distanceTraveled;
            _estimatedDistanceRemaining = estimatedDistanceRemaining;
            Previous = previous;
        }
        
        public PathNode(Vector2Int tilePosition, float distanceTraveled, PathNode previous)
        {
            TilePosition = tilePosition;
            DistanceTraveled = distanceTraveled;
            Previous = previous;
        }
        
        public int CompareTo(PathNode other)
        {
            if (other == null) return 1;
            return DistanceTraveled.CompareTo(other.DistanceTraveled);
        }
    }
}