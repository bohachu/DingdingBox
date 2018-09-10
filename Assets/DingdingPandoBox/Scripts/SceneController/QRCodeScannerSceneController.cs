using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class QRCodeScannerSceneController : ISceneController
    {
        [SerializeField]
        private QRCodeDecodeController qrController;

        [SerializeField]
        private string correctUrl = "https://mractivitiesuat.sogo.com.tw/cart/myorder";

        public override IEnumerator InitializeCoroutine()
        {
            qrController.onQRScanFinished += qrScanFinished;
            yield return null;
        }

        public override IEnumerator ReleaseCoroutine()
        {
            qrController.onQRScanFinished -= qrScanFinished;
            yield return null;
        }

        public void OnBackButtonClicked()
        {
            SceneNavigator.Instance.LoadScene("FloorList");
        }

        public void OnResetButton()
        {
            qrController.Reset();
            qrController.StartWork();
        }

        private void qrScanFinished(string dataText)
        {
            if (Utility.CheckIsUrlFormat(dataText))
            {
                if (!dataText.Contains("http://") && !dataText.Contains("https://"))
                {
                    dataText = "http://" + dataText;
                }
                Debug.Log(dataText);

                if(dataText == correctUrl)
                {
                    RecordCenter.Instance.SetExchangedCompleted();   
                    MessageBoxManager.Instance.ShowMessageBox("exchanging");
                }
            }
        }
    }
}