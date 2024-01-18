using UnityEngine;

public class ShootRandomDirection : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    [Header("x is right, y is up, z is down and w is left")] [SerializeField]
    private Vector4 maxPosition;

    private Vector3 _direction;


    private void Awake()
    {
        _direction = Random.insideUnitCircle;
    }

    // Start is called before the first frame update
    void Start()
    {
      //  GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * speed);
    }

    private void Update()
    {
        transform.position += _direction * speed; //* Time.deltaTime;

        //Is outside on x axis
        if (transform.position.x > maxPosition.x || transform.position.x < maxPosition.w)
            HandleOutsideX();
        

        //Is outside on y axis
        if (transform.position.y > maxPosition.y || transform.position.y < maxPosition.z)
            HandleOutsideY();
    }

    private void HandleOutsideX()
    {
        //On right side
        if (transform.position.x > maxPosition.x)
        {
            //Flip the x direction if it is still going away
            if (_direction.x > 0)
            {
                _direction.x = -_direction.x;
            }

            return;
        }

        //Flip the x direction if it is still going away
        if (_direction.x < 0)
        {
            _direction.x = -_direction.x;
        }
    }
    
    private void HandleOutsideY()
    {
        //On right side
        if (transform.position.y > maxPosition.y)
        {
            //Flip the x direction if it is still going away
            if (_direction.y > 0)
            {
                _direction.y = -_direction.y;
            }

            return;
        }

        //Flip the x direction if it is still going away
        if (_direction.y < 0)
        {
            _direction.y = -_direction.y;
        }
    }
}