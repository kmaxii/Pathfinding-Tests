using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisualizer : MonoBehaviour {
    [SerializeField] private GameObject baseTilePrefab;
    [SerializeField] private Color[] colorSpace;
    [SerializeField] private MapData mapDataSO;
    [SerializeField][Range(1, 1000)] private int gridSize;
    [SerializeField][Range(1, 100)] private int obsRate;

    public void Awake() {
        mapDataSO.SetMap(gridSize, obsRate);
        VisualizeGrid();
    }    
    public void VisualizeGrid() {
        //  Take the data from mapDataSO and visualize the bools as a white or black square
        for (int i = 0; i < mapDataSO.map.GetLength(0); i++) {
            for (int j = 0; j < mapDataSO.map.GetLength(1); j++) {
                if (mapDataSO.map[i, j]) {
                    SpawnTileAndChangeColor(new Vector2(i, j), colorSpace[0]);
                }
                else {
                    SpawnTileAndChangeColor(new Vector2(i, j), colorSpace[1]);
                }
            }
        }
    }

    private void SpawnTileAndChangeColor(Vector2 pos, Color color) {
        GameObject tile = baseTilePrefab;
        tile.GetComponent<SpriteRenderer>().color = color;
        Vector3 tilePos = new Vector3(pos.x, pos.y, 0);
        //  Move the tile so that it centers the tilegrid as a whole
        tilePos -= new Vector3(mapDataSO.map.GetLength(0) / 2f, mapDataSO.map.GetLength(1) / 2f, 0);
        
        
        Instantiate(baseTilePrefab, tilePos, Quaternion.identity, transform);
    }
}
