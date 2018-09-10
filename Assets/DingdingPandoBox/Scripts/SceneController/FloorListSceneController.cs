using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class FloorListSceneController : ISceneController
    {
        [SerializeField]
        private FadePageAnimator fadePageAnimator;

        [SerializeField]
        private RectTransform contentRectTrans;

        [SerializeField]
        private GameObject floorCompaignBtnPrefab;

        [SerializeField]
        private RectTransform exchangeButtonRectTrans;

        [SerializeField]
        private int enableExchageCount = 3;

        [SerializeField]
        private float exchangeButtonTime = 0.5f;

        [SerializeField]
        private int scanCompletedCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < DataCenter.Instance.InfoList.Count; ++i)
                {
                    if(RecordCenter.Instance.CheckScanCompleted(DataCenter.Instance.InfoList[i].urlLink))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public override IEnumerator InitializeCoroutine()
        {
            for(int i = 0; i < DataCenter.Instance.InfoList.Count; ++i)
            {
                PandoBoxInfo info = DataCenter.Instance.InfoList[i];

                GameObject campaignBtnObj = Instantiate(floorCompaignBtnPrefab) as GameObject;
                CompaignBtnController compaignBtnController = campaignBtnObj.GetComponent<CompaignBtnController>();
                campaignBtnObj.transform.SetParent(contentRectTrans, false);
                compaignBtnController.Init(info);
            }

            yield return StartCoroutine(fadePageAnimator.FadeInCoroutine());

            setExchangeButton();
        }

        public override IEnumerator ReleaseCoroutine()
        {
            yield return StartCoroutine(fadePageAnimator.FadeOutCoroutine());
            yield return null;
        }

        public void OnClearBtnClicked()
        {
            RecordCenter.Instance.Clear();
            Debug.Log("Clear user record!");
        }

        public void OnInfoButtonClicked()
        {
            MessageBoxManager.Instance.ShowMessageBox("Info");
        }

        public void OnChangeButtonClicked()
        {
            if (RecordCenter.Instance.CheckExchangedCompleted())
            {
                MessageBoxManager.Instance.ShowMessageBox("exchanged");
            }
            else
            {
                SceneNavigator.Instance.LoadScene("QRCodeScanner");
            }
        }

        private void setExchangeButton()
        {
            exchangeButtonRectTrans.gameObject.SetActive(scanCompletedCount >= enableExchageCount);
        }
    }

}
