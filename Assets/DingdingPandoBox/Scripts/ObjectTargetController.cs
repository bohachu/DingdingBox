using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class ObjectTargetController : MonoBehaviour
    {
        public Transform Root;

        public GameObject PresentPrefab;

        public GameObject VideoPrefab;

        private PandoBoxInfo pandoBoxInfo;

        private IDisplayObject displayObject;

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
            if (displayObject != null)
            {
                Close();
            }

            if (!isTriggered)
            {
                showVideo();
            }
            else
            {
                showPresent();
            }
        }

        public void Close()
        {
            if (displayObject != null)
            {
                Destroy(displayObject.gameObject);
                displayObject = null;
            }
        }

        private IDisplayObject showPresent()
        {
            GameObject newObj = Instantiate(PresentPrefab, Root, false);
            displayObject = newObj.GetComponent<IDisplayObject>();
            displayObject.Show();
            return displayObject;
        }

        private IDisplayObject showVideo()
        {
            GameObject newObj = Instantiate(VideoPrefab, Root, false);
            displayObject = newObj.GetComponent<IDisplayObject>();
            Debug.LogFormat("{0}", pandoBoxInfo.videoUrl);
            displayObject.Show(pandoBoxInfo.urlLink, gameObject, "onVideoPlayedCompleted");
            return displayObject;
        }

        private void onVideoPlayedCompleted()
        {
            RecordCenter.Instance.AddScan(pandoBoxInfo.urlLink);
            Show();
        }
    }
}