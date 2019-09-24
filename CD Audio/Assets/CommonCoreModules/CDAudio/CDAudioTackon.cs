using CommonCore.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.CDAudio
{

    /// <summary>
    /// Component that can be attached to audio sources to override and provide CD audio
    /// </summary>
    public class CDAudioTackon : MonoBehaviour
    {
        [SerializeField]
        private bool RandomTrack;
        [SerializeField]
        private int Track;

        private void Start()
        {
            if(!CDAudioModule.Instance.Ready)
            {
                Debug.LogWarning($"CDAudioTackon on {name} ignored because CD Audio is not available/not ready!");
                return;
            }

            AudioSource audioSource = GetComponent<AudioSource>();

            if(audioSource == null)
            {
                Debug.LogError($"CDAudioTackon on {name} ignored because no audio source is attached!");
                return;
            }

            //disable audio source and run CD audio
            audioSource.Stop();

            if(RandomTrack)
            {
                CDAudioModule.Instance.PlayNext(audioSource.loop, ConfigState.Instance.MusicVolume);
            }
            else if(Track != 0)
            {
                CDAudioModule.Instance.Play(Track, audioSource.loop, ConfigState.Instance.MusicVolume);
            }
            else
            {
                CDAudioModule.Instance.Stop();
            }

        }


    }
}