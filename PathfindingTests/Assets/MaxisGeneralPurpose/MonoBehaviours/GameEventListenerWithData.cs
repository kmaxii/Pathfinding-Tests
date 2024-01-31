using MaxisGeneralPurpose.Event;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;

namespace MaxisGeneralPurpose.MonoBehaviours
{
    public class GameEventListenerWithData : MonoBehaviour
    {
        public GameEventWithData @event;
        public DataEventEvent response;

        private void OnEnable()
        {
            @event.RegisterListener(OnEventRaised);
        }

        private void OnDisable()
        {
            @event.UnregisterListener(OnEventRaised);
        }

        public void OnEventRaised(DataCarrier data)
        {
            if (enabled)
                response.Invoke(data);
        }
    }
}
