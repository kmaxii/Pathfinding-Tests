using System.Collections.Generic;
using MonoBehaviors;
using UnityEngine;

namespace MaxisGeneralPurpose.Scriptable_objects
{
    [CreateAssetMenu(menuName = "Custom/Audio/AudioGameEvent")]
    public class GameEventWithAudioEvent : ScriptableObject
    {
        private readonly List<GameEventListenerWithAudioEvent> _listeners = new List<GameEventListenerWithAudioEvent>();
        
    
            public void Raise(AudioEvent audioEvent){
                for (int i = _listeners.Count - 1; i >= 0; i--)
                {
                    _listeners[i].OnEventRaised(audioEvent);
                }
            }

            public void RegisterListener(GameEventListenerWithAudioEvent listener)
            {
                _listeners.Add(listener);
            }

            public void UnregisterListener(GameEventListenerWithAudioEvent listener)
            {
                _listeners.Remove(listener);
            }
        }
}
