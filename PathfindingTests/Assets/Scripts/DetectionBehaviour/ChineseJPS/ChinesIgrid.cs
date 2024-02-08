using System;
using UnityEngine;

namespace DetectionBehaviour.ChineseJPS
{
    [Serializable]
    public class ChinesIgrid: IGrid
    {
        [SerializeField] private MapData mapData;
        public bool IsMovable(Vector2Int pos)
        {
            return mapData.CheckCoordinate(pos.x, pos.y);
        }

        public bool IsWall(Vector2Int pos)
        {
            return !mapData.CheckCoordinate(pos.x, pos.y);
        }
    }
}