using MaxisGeneralPurpose.Scriptable_objects;
using Scriptable_objects;
using UnityEngine;
using UnityEngine.Events;

namespace MonoBehaviors
{
    public class GameEventListenerWithAudioEvent : MonoBehaviour
    {
        public GameEventWithAudioEvent @event;
        public AudioEventEvent response;

        private void OnEnable()
        {
            @event.RegisterListener(this);
        }

        private void OnDisable()
        {
            @event.UnregisterListener(this);
        }

        public void OnEventRaised(AudioEvent audioEvent)
        {
            if (enabled)
                response.Invoke(audioEvent);
        }
    }
}
