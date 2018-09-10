using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cameo.PandoBox
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField]
        private Button prevFloorBtn;

        [SerializeField]
        private Button nextFloorBtn;

        [SerializeField]
        private RawImage floorImage;

        [SerializeField]
        private Image warningLabel;

        [SerializeField]
        private Text floorText;

        List<PandoBoxInfo> pandoBoxInfos;
        private int shopIndex = 0;

        public void Init(List<PandoBoxInfo> pandoBoxInfos)
        {
            this.pandoBoxInfos = pandoBoxInfos;
            floorText.text = (pandoBoxInfos[0].weights > 0) ? string.Format("{0}F", pandoBoxInfos[0].weights) : string.Format("B{0}F", Mathf.Abs(pandoBoxInfos[0].weights));

            shopIndex = 0;
            updateUI();
        }

        public void OnButtonClicked()
        {
            Dictionary<string, object> paramMapping = new Dictionary<string, object>();
            paramMapping.Add("info", pandoBoxInfos[shopIndex]);
            SceneNavigator.Instance.LoadScene("PandoBoxScanner", paramMapping);
        }

        public void OnPrevFloorBtnClicked()
        {
            shopIndex--;
            updateUI();
        }

        public void OnNextFloorBtnClicked()
        {
            shopIndex++;
            updateUI();
        }

        void updateUI()
        {
            StartCoroutine("loadImage");
          

            warningLabel.gameObject.SetActive(false);
            prevFloorBtn.gameObject.SetActive(shopIndex > 0);
            nextFloorBtn.gameObject.SetActive(shopIndex < pandoBoxInfos.Count - 1);
        }

        private IEnumerator loadImage()
        {
            WWW www = new WWW(pandoBoxInfos[shopIndex].photoUrl);
            yield return www;

            if(string.IsNullOrEmpty(www.error))
            {
                floorImage.texture = www.texture;

            }

        }
    }

}
