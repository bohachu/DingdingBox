using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameo;
using System;

namespace Cameo.PandoBox
{
    public class PreloaderSceneController : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            DataCenter.Instance.OnDataReady += onLoadCompleted;
            DataCenter.Instance.OnDataFail += onLoadFailed;
            DataCenter.Instance.LoadCgg();
        }

        private void OnDestroy()
        {
            DataCenter.Instance.OnDataReady -= onLoadCompleted;
            DataCenter.Instance.OnDataFail -= onLoadFailed;
        }

        private void onLoadFailed()
        {
            DebugLogger.Instance.LogError("Data load error!");
        }

        private void onLoadCompleted()
        {
            SceneNavigator.Instance.LoadScene("FloorList");
        }

    }

}
