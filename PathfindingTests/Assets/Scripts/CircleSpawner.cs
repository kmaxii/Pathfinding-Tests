using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    [Header("Basic Setup")]
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private Transform topRight;
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform bottomLeft;
    
    [Header("Circle count")]
    [SerializeField] private int startAmount;
    [SerializeField] private int maxAmount;
    [SerializeField] private int spawnEachFixedUpdate;

    private int _spawned;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startAmount; i++)
        {
            SpawnOneCircle();
        }
    }

    public void SpawnCircles()
    {
        for (int i = 0; i < spawnEachFixedUpdate; i++)
        {
            SpawnOneCircle();
        }
    }

    private void SpawnOneCircle()
    {
        if (_spawned >= maxAmount)
            return;
        _spawned++;
        Instantiate(circlePrefab, RandomPosInSquare(), Quaternion.identity);
    }

    private Vector3 RandomPosInSquare()
    {
        var topLeftPosition = topLeft.position;
        return new Vector3(
            Random.Range(topLeftPosition.x, topRight.position.x),
            Random.Range(topLeftPosition.y, bottomLeft.position.y), 0);
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
