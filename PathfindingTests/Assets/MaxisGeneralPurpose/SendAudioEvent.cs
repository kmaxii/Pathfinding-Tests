using System.Collections;
using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using Scriptable_objects;
using UnityEngine;

public class SendAudioEvent : MonoBehaviour
{
    [SerializeField] private GameEventWithAudioEvent @event;
    [SerializeField] private AudioEvent audioEvent;



    public void TriggerAudioEvent()
    {
        @event.Raise(audioEvent);
    }
}
