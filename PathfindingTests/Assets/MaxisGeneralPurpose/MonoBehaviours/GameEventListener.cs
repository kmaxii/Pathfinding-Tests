using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;
using UnityEngine.Events;

namespace MaxisGeneralPurpose.MonoBehaviours
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent @event;
        public UnityEvent response;

        private void OnEnable()
        {
            @event.RegisterListener(OnEventRaised);
        }

        private void OnDisable()
        {
            @event.UnregisterListener(OnEventRaised);
        }

        public void OnEventRaised()
        {
            if (enabled)
                response.Invoke();
        }
    }
}
