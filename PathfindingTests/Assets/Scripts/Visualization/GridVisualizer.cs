using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Visualization
{
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer baseTilePrefab;
        [SerializeField] private Color[] colorSpace;

        [FormerlySerializedAs("mapDataSO")] [SerializeField]
        private MapData mapDataSo;

        [SerializeField] [Range(1, 1000)] private int gridSize;
        [SerializeField] [Range(1, 100)] private int obsRate;

        [SerializeField] private GameEventWithVector2Int changeToRed;

        private void OnEnable()
        {
            changeToRed.RegisterListener(ChangeToRed);
        }

        private void OnDisable()
        {
            changeToRed.UnregisterListener(ChangeToRed);
        }

        private void ChangeToRed(Vector2Int pos)
        {
            ChangeColorOnPosition(pos, Color.red);
        }


        private Dictionary<Vector2Int, SpriteRenderer> tileMap;

        public void Awake()
        {
            tileMap = new();
            mapDataSo.SetMap(gridSize, obsRate);
            VisualizeGrid();
        }

        public void VisualizeGrid()
        {
            //  Take the data from mapDataSO and visualize the bools in different colors
            for (int i = 0; i < mapDataSo.map.GetLength(0); i++)
            {
                for (int j = 0; j < mapDataSo.map.GetLength(1); j++)
                {
                    if (mapDataSo.map[i, j])
                    {
                        SpawnTileAndChangeColor(new Vector2Int(i, j), colorSpace[0]);
                    }
                    else
                    {
                        SpawnTileAndChangeColor(new Vector2Int(i, j), colorSpace[1]);
                    }
                }
            }
        }

        private void SpawnTileAndChangeColor(Vector2Int pos, Color color)
        {
            Vector3 tilePos = new Vector3(pos.x, pos.y, 0);
            var temp = Instantiate(baseTilePrefab, tilePos, Quaternion.identity, transform);
            //  Add the spriterenderer of this object as a value to tileMap and the position should be the key
            temp.transform.name = $"{tilePos}";
            temp.color = color;
            tileMap.Add(pos, temp);
        }

        private void ChangeColorOnPosition(Vector2Int pos, Color color)
        {
            // change the color of the tile in tileMap on the correct position.
            tileMap[pos].color = color;
        }
    }
}