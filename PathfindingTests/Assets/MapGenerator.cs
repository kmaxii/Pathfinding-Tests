using ProceduralNoiseProject;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MapData mapData;
    public NOISE_TYPE noiseType = NOISE_TYPE.PERLIN;


    public int seed = 0;


    public float frequency = 5.0f;
    
    public int jitter = 1;


    public int width = 512;


    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        Noise noise = GetNoise();
        mapData.SetMap(width, noise);
    }


    private Noise GetNoise()
    {
        switch (noiseType)
        {
            case NOISE_TYPE.PERLIN:
                return new PerlinNoise(seed, frequency);

            case NOISE_TYPE.VALUE:
                return new ValueNoise(seed, frequency);

            case NOISE_TYPE.SIMPLEX:
                return new SimplexNoise(seed, frequency);

            case NOISE_TYPE.VORONOI:
                return new VoronoiNoise(seed, frequency);

            case NOISE_TYPE.WORLEY:
                return new WorleyNoise(seed, frequency, jitter);

            default:
                return new PerlinNoise(seed, frequency);
        }
    }

 
}