using UnityEngine;

namespace MaxisGeneralPurpose.Scriptable_objects
{
    [System.Serializable]
    public abstract class AudioEvent : ScriptableObject
    {
        
        public GameEventWithAudioEvent gameEventWithAudio;

        public abstract void Play(AudioSource audioSource);

        // This method is called when the script is loaded or a value is changed in the inspector
        private void OnEnable()
        {
            // Assign a default GameEventWithAudioEvent if none is assigned
            if (gameEventWithAudio == null)
            {
                 gameEventWithAudio = Resources.Load<GameEventWithAudioEvent>("SendAudioEvent");
            }
        }

        // You can add a method to raise the event
        public void RaiseEvent()
        {
            if (gameEventWithAudio != null)
            {
                gameEventWithAudio.Raise(this);
            }
            else
            {
                Debug.LogWarning("GameEventWithAudioEvent is not assigned to this AudioEvent.");
            }
        }
    }
}