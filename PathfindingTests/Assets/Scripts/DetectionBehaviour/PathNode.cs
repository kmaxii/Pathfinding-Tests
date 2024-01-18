using UnityEngine;

namespace DetectionBehaviour
{
    public class PathNode
    {

        public readonly Transform TilePosition;
        public float DistanceTraveled;
        private readonly float _estimatedDistanceRemaining;
        public PathNode Previous;

        public float EstimatedCost => _estimatedDistanceRemaining + DistanceTraveled;

        public PathNode(Transform tilePosition, float distanceTraveled, float estimatedDistanceRemaining, PathNode previous)
        {
            TilePosition = tilePosition;
            DistanceTraveled = distanceTraveled;
            _estimatedDistanceRemaining = estimatedDistanceRemaining;
            Previous = previous;
        }
        
        public PathNode(Transform tilePosition, float distanceTraveled, PathNode previous)
        {
            TilePosition = tilePosition;
            DistanceTraveled = distanceTraveled;
            Previous = previous;
        }
    }
}