using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;

namespace Scriptable_objects
{
    [CreateAssetMenu(menuName = "Custom/Audio/Simple")]
    public class SimpleAudioPlayer : AudioEvent
    {
        public List<AudioClip> audioClips;

        [Range(0, 2)] public float minimumVolume = 1;
        [Range(0, 2)] public float maximumVolume = 1;

        [Range(0, 2)] public float minimumPitch = 1;
        [Range(0, 2)] public float maximumPitch = 1;
        

        public override void Play(AudioSource audioSource)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Count - 1)];
            audioSource.volume = Random.Range(minimumVolume, maximumVolume);
            audioSource.pitch = Random.Range(minimumPitch, maximumPitch);
            audioSource.Play();
        }
    }
}
