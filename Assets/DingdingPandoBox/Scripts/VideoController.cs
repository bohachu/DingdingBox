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

        public Action VideoFinishedCallback = delegate { };

        private RequestResolver resolver;

        private float time = 0;

        private bool isPrepared;

        public void Start()
        {
            isPrepared = false;
            resolver = gameObject.AddComponent<RequestResolver>();
        }

        public void Play(string youtubeID)
        {
            if (!isPrepared)
                StartCoroutine(resolver.GetDownloadUrls(FinishLoadingUrls, youtubeID, false));
            else
                unityVideoPlayer.Play();
                
        }

        public void Pause()
        {
            if(unityVideoPlayer.isPlaying)
                unityVideoPlayer.Pause();
        }

        public void Stop()
        {
            unityVideoPlayer.Stop();
        }

        public void Reset()
        {
            if (unityVideoPlayer.isPlaying)
                unityVideoPlayer.Stop();
            unityVideoPlayer.time = 0;
        }

        private void FinishLoadingUrls()
        {
            List<VideoInfo> videoInfos = resolver.videoInfos;
            foreach (VideoInfo info in videoInfos)
            {
                if (info.VideoType == VideoType.Mp4 && info.Resolution == (360))
                {
                    if (info.RequiresDecryption)
                    {
                        StartCoroutine(resolver.DecryptDownloadUrl(prepareVideoPlayer, info));
                        break;
                    }
                    else
                    {
                        prepareVideoPlayer(info.DownloadUrl);
                    }
                    break;
                }
            }
        }

        private void prepareVideoPlayer(string url)
        {
            unityVideoPlayer.source = VideoSource.Url;
            unityVideoPlayer.url = url;
            unityVideoPlayer.Prepare();
            StartCoroutine("playCoroutine");
        }

        private IEnumerator playCoroutine()
        {
            WaitForSeconds waitTime = new WaitForSeconds(0.1f);
            while (!unityVideoPlayer.isPrepared)
            {
                Debug.Log("Preparing Video");
                yield return waitTime;
                break;
            }
            isPrepared = true;
            unityVideoPlayer.Play();
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


