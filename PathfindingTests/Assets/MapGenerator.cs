using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapData mapData;


    public int seed = 0;


    public float frequency = 5.0f;
    

    public int width = 512;


    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        mapData.SetMap(width, frequency, seed);
    }




 
}