using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class ObjectTargetController : MonoBehaviour
    {
        [SerializeField]
        private VideoController videoController;

        [SerializeField]
        private GameObject objectPlayer;

        [SerializeField]
        private GameObject objectPresent;

        private PandoBoxInfo pandoBoxInfo;

        private bool isTriggered
        {
            get
            {
                return (pandoBoxInfo != null) ? RecordCenter.Instance.CheckScanCompleted(pandoBoxInfo.urlLink) : false;
            }
        }

        public void Init(PandoBoxInfo pandoBoxInfo)
        {
            this.pandoBoxInfo = pandoBoxInfo;
        }

        public void Show()
        {
            objectPlayer.SetActive(!isTriggered);
            objectPresent.SetActive(isTriggered);

            if(!isTriggered)
            {
                videoController.VideoFinishedCallback += onVideoPlayedCompleted;
                videoController.Play(pandoBoxInfo.urlLink);
            }
        }

        public void Hide()
        {
            objectPlayer.SetActive(false);
            objectPresent.SetActive(false);
            videoController.Stop();
        }

        private void onVideoPlayedCompleted()
        {
            RecordCenter.Instance.AddScan(pandoBoxInfo.urlLink);
            objectPlayer.SetActive(false);
            objectPresent.SetActive(true);
        }
    }
}