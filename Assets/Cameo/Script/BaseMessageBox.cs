using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Cameo
{
	[RequireComponent(typeof(CanvasGroup))]
	public class BaseMessageBox : MonoBehaviour 
	{
        protected Dictionary<string, object> dicParams = null;
        protected Action<BaseMessageBox> onCloseCallback = delegate { };

        public void Open(Action<BaseMessageBox> onCloseCallback, Dictionary<string, object> dicParams)
		{
            this.onCloseCallback += onCloseCallback;
			this.dicParams = dicParams;

			onOpen ();
			playOpenAnimation ();
		}

		public void Close()
		{
			onClose ();
			playCloseAnimation ();
		}

		protected void onBoxClosed()
		{
			onClosed ();
            onCloseCallback(this);
		}

		protected virtual void playOpenAnimation()
		{

		}

		protected virtual void playCloseAnimation()
		{

		}

		protected virtual void onOpen()
		{

		}

		protected virtual void onClose()
		{

		}

		protected virtual void onOpened()
		{
			
		}

		protected virtual void onClosed()
		{
			
		}
	}

	[System.Serializable]
	public class MessageBoxInfo
	{
		public string TypeName;
		public BaseMessageBox MessageBox;
	}
}
