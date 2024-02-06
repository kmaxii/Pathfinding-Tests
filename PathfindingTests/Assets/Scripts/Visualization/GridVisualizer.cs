using System.Collections.Generic;
using System.Diagnostics;
using MaxisGeneralPurpose.Scriptable_objects;
using QuickEye.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

namespace Visualization
{
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private GameEvent setMapEvent;


        [Header("TileMap Stuff")] [SerializeField]
        private Tile _walkableTile;

        [SerializeField] private Tile _obstacleTile;
        [SerializeField] private Tile _correctRouteTile;
        [SerializeField] private Tile _startEndTile;
        [SerializeField] private Tilemap _tilemap;

        [Header("Scriptable Objects")] [FormerlySerializedAs("mapDataSO")] [SerializeField]
        private MapData mapDataSo;

        [Header("Sizes and Variables")] 
        [SerializeField] private int _gridSize;

        [SerializeField] [Range(0, 100)] private int _obsRate;

        [Header("Events")] [SerializeField] private GameEventWithVector2Int _changeToRed;

        [SerializeField] private UnityDictionary<GameEventWithVector2Int, Tile> setColor;

        private void OnEnable() {
            _changeToRed.RegisterListener(ChangeToCorrectPath);
            setMapEvent.RegisterListener(VisualizeGrid);
            foreach (var keyValuePair in setColor)
            {
                
                keyValuePair.Key.RegisterListener((pos) =>
                {
                    Vector3Int newPos = new Vector3Int(pos.x, pos.y, 0);

                    _tilemap.SetTile(newPos, keyValuePair.Value);
                });
            }
        }

        private void OnDisable() {
            _changeToRed.UnregisterListener(ChangeToCorrectPath);
            setMapEvent.UnregisterListener(VisualizeGrid);
        }

        private void ChangeToCorrectPath(Vector2Int pos) {
            Vector3Int newPos = new Vector3Int(pos.x, pos.y, 0);
            _tilemap.SetTile(newPos, _correctRouteTile);
        }


        private Dictionary<Vector2Int, SpriteRenderer> tileMap;

        public void Start() {
            //  Measure the time for this function to run
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            
            VisualizeGrid();
            
            //  debug.log the measured time
            functionTimer.Stop();
            Debug.Log("Map visualization time: " + functionTimer.Elapsed.TotalSeconds);
        }

        public void VisualizeGrid() {
            //  Take the data from mapDataSO and visualize the bools in different colors
            for (int i = 0; i < mapDataSo.map.GetLength(0); i++) {
                for (int j = 0; j < mapDataSo.map.GetLength(1); j++) {
                    if (mapDataSo.map[i, j]) {
                        //SpawnTileAndChangeColor(new Vector2Int(i, j), _colorSpace[0]);
                        SetTileOnTilemap(new Vector3Int(i, j, 0), _walkableTile);
                    }
                    else {
                        //SpawnTileAndChangeColor(new Vector2Int(i, j), _colorSpace[1]);
                        SetTileOnTilemap(new Vector3Int(i, j, 0), _obstacleTile);
                    }
                }
            }
            SetTileOnTilemap(new Vector3Int(mapDataSo.startPos.x, mapDataSo.startPos.y, 0), _startEndTile);
            SetTileOnTilemap(new Vector3Int(mapDataSo.endPos.x, mapDataSo.endPos.y, 0), _startEndTile);
        }

        public void ResetGrid() {
            //  reset the grid to its default state
            _tilemap.ClearAllTiles();
        }

        private void SetTileOnTilemap(Vector3Int pos, Tile tile) {
            _tilemap.SetTile(pos, tile);
        }
    }
}