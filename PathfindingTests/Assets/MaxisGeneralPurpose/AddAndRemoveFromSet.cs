using Scriptable_objects;
using UnityEngine;

public class AddAndRemoveFromSet : MonoBehaviour
{

    [SerializeField] private SetReference setReference;
    
    // Start is called before the first frame update
    void Start()
    {
        setReference.Add(gameObject);
    }

    private void OnDestroy()
    {
        setReference.Remove(gameObject);
    }
}
