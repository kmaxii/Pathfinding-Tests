using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisualizer : MonoBehaviour {
    [SerializeField] private SpriteRenderer baseTilePrefab;
    [SerializeField] private Color[] colorSpace;
    [SerializeField] private MapData mapDataSO;
    [SerializeField][Range(1, 1000)] private int gridSize;
    [SerializeField][Range(1, 100)] private int obsRate;

    private Dictionary<Vector2Int, SpriteRenderer> tileMap;

    public void Awake() {
        tileMap = new();
        mapDataSO.SetMap(gridSize, obsRate);
        VisualizeGrid();
        ChangeColorOnPosition(new Vector2Int(10, 10), colorSpace[2]);
    }    
    public void VisualizeGrid() {
        //  Take the data from mapDataSO and visualize the bools in different colors
        for (int i = 0; i < mapDataSO.map.GetLength(0); i++) {
            for (int j = 0; j < mapDataSO.map.GetLength(1); j++) {
                if (mapDataSO.map[i, j]) {
                    SpawnTileAndChangeColor(new Vector2Int(i, j), colorSpace[0]);
                }
                else {
                    SpawnTileAndChangeColor(new Vector2Int(i, j), colorSpace[1]);
                }
            }
        }
    }

    private void SpawnTileAndChangeColor(Vector2Int pos, Color color) {
        
        Vector3 tilePos = new Vector3(pos.x, pos.y, 0);
        var temp = Instantiate(baseTilePrefab, tilePos, Quaternion.identity, transform);
        //  Add the spriterenderer of this object as a value to tileMap and the position should be the key
        temp.transform.name = $"{tilePos}";
        temp.color = color;
        tileMap.Add(pos, temp);
        
    }

    public void ChangeColorOnPosition(Vector2Int pos, Color color) {
        // change the color of the tile in tileMap on the correct position.
        tileMap[pos].color = color;
        Debug.Log(tileMap[pos].color);
        
    }
}
