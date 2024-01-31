using UnityEngine;

public class RaycastIgnoreSetting : MonoBehaviour
{
    [SerializeField] private LayerMask inputLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().eventMask = inputLayerMask;
    }

}
