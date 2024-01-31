using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using Scriptable_objects;
using UnityEngine;

public class AudioReceiver : MonoBehaviour
{

    private readonly List<AudioSource> _audioSources = new List<AudioSource>();
    

    // Update is called once per frame
    void Update()
    {
        if (_audioSources.Count > 10)
        {
            AudioSource audioSource = _audioSources[10];
            if (!audioSource.isPlaying)
            {
                Destroy(audioSource);
                _audioSources.RemoveAt(10);
            }
        }
    }


    public void OnReceiveAudio(AudioEvent audioEvent)
    {
        //Searches for a not playing audio source
        for (int i = 0; i < _audioSources.Count; i++)
        {
            AudioSource audioSource = _audioSources[i];
            if (audioSource.isPlaying)
                continue;
            //Found unused audio source. Playing using that
            audioEvent.Play(audioSource);
            return;
        }
        
        //No unused audio source was found. So adding a new one.
        _audioSources.Add(gameObject.AddComponent<AudioSource>());
        audioEvent.Play(_audioSources[^1]);
    }
}
