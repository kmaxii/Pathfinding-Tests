using UnityEngine;


[CreateAssetMenu(menuName = "Custom/MapData")]

public class MapData : ScriptableObject
{
    public bool[,] map;
    
    //Property to get the map size
    public int MapSize => map.GetLength(0);

    //Method to check a coordinate
    public bool CheckCoordinate(int x, int y)
    {
        return map[x, y];
    }
    
    

    //Method to set the map to x size and percent chance for each to be false
    public void SetMap(int x, int percent)
    {
        map = new bool[x, x];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < x; j++)
            {
                if (Random.Range(0, 100) < percent)
                {
                    map[i, j] = false;
                }
                else
                {
                    map[i, j] = true;
                }
            }
        }

        Debug.Log(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        SetMap(10, 10);
    }


    public override string ToString()
    {
        string mapString = "";
        for (int i = 0; i < map.GetLength(0); i++)
        {
            mapString += "\n";
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j])
                {
                    mapString += "_";
                }
                else
                {
                    mapString += "X";
                }
            }
        }

        return mapString;
    }
}