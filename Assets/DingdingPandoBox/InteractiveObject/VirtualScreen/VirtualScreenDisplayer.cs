using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class VirtualScreenDisplayer : IDisplayObject
    {
        [SerializeField]
        private YoutubePlayer youtubePlayer;

        private GameObject completedGameObject;
        private string completedMethodName;

        public override void Show(params object[] param)
        {
            string youtubeUrl = param[0].ToString();
            completedGameObject = (GameObject)param[1];
            completedMethodName = param[2].ToString();

            youtubePlayer.youtubeUrl = youtubeUrl;
            youtubePlayer.videoPlayer.loopPointReached += VideoPlayer_LoopPointReached;
        }

        void VideoPlayer_LoopPointReached(UnityEngine.Video.VideoPlayer source)
        {
            completedGameObject.SendMessage(completedMethodName);
        }
    }
}