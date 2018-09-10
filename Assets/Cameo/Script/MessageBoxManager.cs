using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Cameo
{
	public class MessageBoxManager : Singleton<MessageBoxManager>
	{
		public Image Background;
		public MessageBoxInfo[] MessageBoxInfoList;
		public float FadeTime = 0.2f;

        public CanvasGroup canvasGroup;

        private RectTransform rectTran;
        private List<BaseMessageBox> curOpendMessageBoxs = new List<BaseMessageBox> ();
        private BaseMessageBox baseMessageBox = null;
        private Dictionary<string, object> dicParams;
        private Dictionary<string, BaseMessageBox> msgBoxInfoMap;

		void Awake()
		{
			rectTran = GetComponent<RectTransform> ();
            Background.enabled = false;
			msgBoxInfoMap = new Dictionary<string, BaseMessageBox> ();
			for (int i = 0; i < MessageBoxInfoList.Length; ++i) 
			{
				msgBoxInfoMap.Add (MessageBoxInfoList [i].TypeName, MessageBoxInfoList [i].MessageBox);
			}
		}

		public BaseMessageBox ShowMessageBox(string TypeName, Dictionary<string, object> dicParams = null)
		{
            Background.enabled = true;
			this.dicParams = dicParams;

			GameObject msgBox = Instantiate (msgBoxInfoMap [TypeName].gameObject);
			msgBox.GetComponent<RectTransform> ().SetParent (transform, false);
			msgBox.SetActive (false);
			baseMessageBox = msgBox.GetComponent<BaseMessageBox> ();
				
			if (curOpendMessageBoxs.Count == 0) 
			{
				CancelInvoke ();

                LeanTween.alphaCanvas(canvasGroup, 1, FadeTime).setOnComplete(onFadeInFinished);
			} 
			else 
			{
				onFadeInFinished ();
			}

			curOpendMessageBoxs.Add (baseMessageBox);
			return baseMessageBox;
		}

        private void onFadeOutFinished()
        {
            Background.enabled = false;
        }

        private void onFadeInFinished()
        {
            baseMessageBox.gameObject.SetActive(true);
            baseMessageBox.Open(onMessageBoxClosed, dicParams);
        }

        private void onMessageBoxClosed(BaseMessageBox msgBox)
		{
			curOpendMessageBoxs.Remove (msgBox);
		    Destroy (msgBox.gameObject);

			if (curOpendMessageBoxs.Count == 0) 
			{
                LeanTween.alphaCanvas(canvasGroup, 0, FadeTime).setOnComplete(onFadeOutFinished);
			}
		}
	}
}
