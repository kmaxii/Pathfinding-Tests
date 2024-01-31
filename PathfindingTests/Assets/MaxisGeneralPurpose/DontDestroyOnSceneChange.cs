using UnityEngine;

namespace EveryProject
{
    public class DontDestroyOnSceneChange : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        
    }
}
