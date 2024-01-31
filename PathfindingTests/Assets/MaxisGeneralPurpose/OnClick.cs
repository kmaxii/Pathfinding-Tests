using UnityEngine;
using UnityEngine.Events;

public class OnClick : MonoBehaviour
{

    [SerializeField] private UnityEvent _triggerOnClicK;

    private bool _skipNextTrigger;
    
    public bool SkipNextTrigger
    {
        private get
        {
            return _skipNextTrigger;
        }
        set
        {
            _skipNextTrigger = value;
        }
    }
  
    private void OnMouseUp()
    {
        if (SkipNextTrigger)
        {
            SkipNextTrigger = !SkipNextTrigger;
            return;
        }
        Debug.Log(gameObject.name);
        _triggerOnClicK.Invoke();
    }
}
