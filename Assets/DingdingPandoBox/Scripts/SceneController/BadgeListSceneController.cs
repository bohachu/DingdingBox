using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class BadgeListSceneController : ISceneController
    {
        [SerializeField]
        FadePageAnimator fadePageAnimator;

        [SerializeField]
        private RectTransform contentRectTrans;

        [SerializeField]
        private GameObject badgeControllerPrefab;

        [SerializeField]
        private RectTransform exchangeButtonRectTrans;

        [SerializeField]
        private float moveTime = 0.25f;

        private List<BadgeButtonController> badgeButtonControllers;
        private List<PandoBoxInfo> selectedInfo = new List<PandoBoxInfo>();


        public override IEnumerator InitializeCoroutine()
        {
            badgeButtonControllers = new List<BadgeButtonController>();

            for (int i = 0; i < DataCenter.Instance.InfoList.Count; ++i)
            {
                GameObject newObj = Instantiate(badgeControllerPrefab);

                BadgeButtonController badgeButtonController = newObj.GetComponent<BadgeButtonController>();
                badgeButtonController.Init(DataCenter.Instance.InfoList[i],onButtonClicked);
                badgeButtonControllers.Add(badgeButtonController);

                newObj.transform.SetParent(contentRectTrans, false);
            }

            setExchangeButton();

            yield return StartCoroutine(fadePageAnimator.FadeInCoroutine());
            yield return null;
        }

        public override IEnumerator ReleaseCoroutine()
        {
            yield return StartCoroutine(fadePageAnimator.FadeOutCoroutine());
            yield return null;
        }

        public void OnChangePointClicked()
        {
            Dictionary<string, object> paramMapping = new Dictionary<string, object>();
            paramMapping.Add("selectedInfo", selectedInfo);
            SceneNavigator.Instance.LoadScene("QRCodeScanner", paramMapping);
        }

        private void onButtonClicked(PandoBoxInfo pandoBoxInfo, bool isSelected)
        {
            if(isSelected && !selectedInfo.Contains(pandoBoxInfo))
            {
                selectedInfo.Add(pandoBoxInfo);
            }

            if(!isSelected && selectedInfo.Contains(pandoBoxInfo))
            {
                selectedInfo.Remove(pandoBoxInfo);
            }
            setExchangeButton(moveTime);
        }

        private void setExchangeButton(float time = 0)
        {
            LeanTween.moveY(exchangeButtonRectTrans.gameObject, (selectedInfo.Count > 0) ? 0 : -exchangeButtonRectTrans.sizeDelta.y, moveTime);
        }
    }

}