using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cameo
{
    [RequireComponent(typeof(Image))]
    public class MaskUtility : Singleton<MaskUtility>
    {
        [SerializeField]
        private Image maskImage;

        [SerializeField]
        private Color defaultColor = Color.black;

        [SerializeField]
        private float defaultTime = 2f;

        private Action onShowCompletedAction = delegate {};
        private Action onHideCompletedAction = delegate { };

		private void Start()
		{
            maskImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
		}

		public void Show(Action onCompleted, Color color, float time)
        {
            Debug.Log(color);
            onShowCompletedAction = (onCompleted == null) ? (delegate{ }) : onCompleted;
            maskImage.color = new Color(color.r, color.g, color.b, 0);
            Debug.Log(maskImage.color);
            LeanTween.alpha(maskImage.rectTransform, 1, time).setOnComplete(onShowCompleted);
        }

        private void onShowCompleted()
        {
            maskImage.raycastTarget = true;
            onShowCompletedAction.Invoke();
            onShowCompletedAction = delegate { };
        }

        public void Show(Action onCompleted)
        {
            Show(onCompleted, defaultColor, defaultTime);
        }

        public void Show(Action onComplete, float time)
        {
            Show(onComplete, defaultColor, time);
        }

        public void Show(float time)
        {
            Show(null, defaultColor, time);
        }

        public void Show()
        {
            Show(null, defaultColor, defaultTime);
        }

        public void Hide(Action onCompleted, float time)
        {
            Debug.Log(maskImage.color);
            onHideCompletedAction = (onCompleted == null) ? (delegate { }) : onCompleted;
            LeanTween.alpha(maskImage.rectTransform, 0, time).setOnComplete(onHideCompleted);
        }

        private void onHideCompleted()
        {
            maskImage.raycastTarget = false;
            onHideCompletedAction.Invoke();
            onHideCompletedAction = delegate { };
        }

        public void Hide(float time)
        {
            Hide(null, time);
        }

        public void Hide()
        {
            Hide(null, defaultTime);
        }
    }
}

