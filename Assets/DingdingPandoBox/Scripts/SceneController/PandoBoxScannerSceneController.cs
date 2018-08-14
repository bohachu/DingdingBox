using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class PandoBoxScannerSceneController : ISceneController
    {
        [SerializeField]
        private List<TrackerInfo> trackerInfos;

        private ObjectTargetController objectTargetController;
        private PandoBoxInfo pandoBoxInfo;

        public override IEnumerator InitializeCoroutine()
        {
            pandoBoxInfo = (PandoBoxInfo)ParamMapping["info"];
            for (int i = 0; i < trackerInfos.Count; ++i)
            {
                if(trackerInfos[i].Id == pandoBoxInfo.picTypeId)
                {
                    GameObject obj = Instantiate(trackerInfos[i].Prefab) as GameObject;
                    objectTargetController = obj.GetComponent<ObjectTargetController>();
                    objectTargetController.Init(pandoBoxInfo);
                    break;
                }
            }
            yield return null;
        }

        public override IEnumerator ReleaseCoroutine()
        {
            yield return null;
        }

        public void OnBackButtonClicked()
        {
            SceneNavigator.Instance.LoadScene("FloorList");
        }
    }

    [Serializable]
    public class TrackerInfo
    {
        public string Id;
        public GameObject Prefab;
    }
}