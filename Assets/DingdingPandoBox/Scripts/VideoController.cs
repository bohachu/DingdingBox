using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using YoutubeLight;

namespace Cameo.PandoBox
{
    public class VideoController : MonoBehaviour
    {
        [SerializeField]
        private VideoPlayer unityVideoPlayer;

        [SerializeField]
        private YoutubePlayer youtubePlayer;

        public Action VideoFinishedCallback = delegate { };

        private RequestResolver resolver;

        private float time = 0;

        private bool isPrepared;

        public void Start()
        {
            isPrepared = false;
            youtubePlayer.enabled = false;
            resolver = gameObject.AddComponent<RequestResolver>();
        }

        public void Play(string youtubeID)
        {
            Debug.Log(youtubeID);
            youtubePlayer.videoUrl = youtubeID;
            unityVideoPlayer.loopPointReached += UnityVideoPlayer_LoopPointReached;
            youtubePlayer.enabled = true;
        }

        public void Stop()
        {
            unityVideoPlayer.loopPointReached -= UnityVideoPlayer_LoopPointReached;
            unityVideoPlayer.Stop();
        }

        private void UnityVideoPlayer_LoopPointReached(VideoPlayer source)
        {
            Debug.Log("Video is played completly...");
            unityVideoPlayer.loopPointReached -= UnityVideoPlayer_LoopPointReached;
            VideoFinishedCallback.Invoke();
            VideoFinishedCallback = delegate { };
        }
    }
}


