using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cameo.PandoBox
{
    public class CompaignBtnController : MonoBehaviour
    {
        [SerializeField]
        private Text floorText;

        [SerializeField]
        private RawImage rawImage;

        [SerializeField]
        private GameObject completedObj;

        [SerializeField]
        private float spaceWidth = 5f;

        private PandoBoxInfo pandoBoxInfo;

        public void Init(PandoBoxInfo info)
        {
            pandoBoxInfo = info;
            floorText.text = (pandoBoxInfo.weights > 0) ? string.Format("{0}F", pandoBoxInfo.weights) : string.Format("B{0}F", Mathf.Abs(pandoBoxInfo.weights));
            completedObj.SetActive(RecordCenter.Instance.CheckScanCompleted(pandoBoxInfo.urlLink));
            StartCoroutine("loadImage");
        }

        public void OnButtonClicked()
        {
            Dictionary<string, object> paramMapping = new Dictionary<string, object>();
            paramMapping.Add("info", pandoBoxInfo);
            SceneNavigator.Instance.LoadScene("PandoBoxScanner", paramMapping);
        }

        private IEnumerator loadImage()
        {
            WWW www = new WWW(pandoBoxInfo.photoUrl);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                rawImage.texture = www.texture;
                RectTransform rectTransform = GetComponent<RectTransform>();
                setImageSize(rectTransform, rawImage.rectTransform, rawImage.texture.width, rawImage.texture.height, spaceWidth);
            }
        }

        private static void setImageSize(RectTransform outterRectTrans, RectTransform innerRectTrans, float texWidth, float texHeight, float spaceWidth)
        {
            float outterRatio = outterRectTrans.sizeDelta.y / outterRectTrans.sizeDelta.x;
            float innerRatio = texHeight / texWidth;

            float innerWidth = outterRectTrans.sizeDelta.x;
            float innerHeight = outterRectTrans.sizeDelta.y;

            if (innerRatio > outterRatio)
            {
                innerHeight = outterRectTrans.sizeDelta.y;
                innerWidth = innerHeight / innerRatio;
            }
            else if (innerRatio < outterRatio)
            {
                innerWidth = outterRectTrans.sizeDelta.x;
                innerHeight = innerWidth * innerRatio;
            }

            innerRectTrans.sizeDelta = new Vector2(innerWidth - 2 * spaceWidth, innerHeight - 2 * spaceWidth);
        }
    } 
}

