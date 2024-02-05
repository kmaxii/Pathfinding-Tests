using System;
using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Visualization
{
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private GameEvent setMapEvent;
        
        
        [Header("TileMap Stuff")] 
        [SerializeField] private Tile _walkableTile;
        [SerializeField] private Tile _obstacleTile;
        [SerializeField] private Tile _correctRouteTile;
        [SerializeField] private Tilemap _tilemap;
        
        [Header("Scriptable Objects")]
        [FormerlySerializedAs("mapDataSO")] [SerializeField]
        private MapData mapDataSo;

        [Header("Sizes and Variables")]
        [SerializeField] [Range(1, 1000)] private int _gridSize;
        [SerializeField] [Range(0, 100)] private int _obsRate;

        [Header("Events")]
        [SerializeField] private GameEventWithVector2Int _changeToRed;
        
        
        

        private void OnEnable() {
            _changeToRed.RegisterListener(ChangeToCorrectPath);
            setMapEvent.RegisterListener(VisualizeGrid);
        }

        private void OnDisable() {
            setMapEvent.UnregisterListener(VisualizeGrid);
            _changeToRed.UnregisterListener(ChangeToCorrectPath);
        }
        private void ChangeToCorrectPath(Vector2Int pos) {
            Vector3Int newPos = new Vector3Int(pos.x, pos.y, 0);
            _tilemap.SetTile(newPos, _correctRouteTile);
        }


        private Dictionary<Vector2Int, SpriteRenderer> tileMap;

        public void Start() {
            //Save current time in millisec
       //     long start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
      //      mapDataSo.SetMap(_gridSize, _obsRate);
            
            //Print time it took to set map in seconds
     //       Debug.Log("Time to set map: " + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - start) / 1000.0);
            
            //Save time
            float start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            
            VisualizeGrid();
            
            //Print time it took to visualize grid in seconds
            Debug.Log("Time to visualize grid: " + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - start) / 1000.0);
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
                        //SpawnTileAndChangeColor(new Vector2Int(i, j), _colorSpace[0]);
                        SetTileOnTilemap(new Vector3Int(i, j, 0), _walkableTile);
                    }
                    else
                    {
                        //SpawnTileAndChangeColor(new Vector2Int(i, j), _colorSpace[1]);
                        SetTileOnTilemap(new Vector3Int(i, j, 0), _obstacleTile);
                    }
                }
            }
        }

        private void SetTileOnTilemap(Vector3Int pos, Tile tile) {
            _tilemap.SetTile(pos, tile);
        }
    }
}