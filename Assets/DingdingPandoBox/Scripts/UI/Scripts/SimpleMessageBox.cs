using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameo;
using UnityEngine.UI;

namespace Cameo.PandoBox
{
    public class SimpleMessageBox : BaseMessageBox
    {
        [SerializeField]
        private RectTransform rectTrans;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private float animTime = 0.25f;

        protected override void playCloseAnimation()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, animTime);
            LeanTween.scale(rectTrans.gameObject, 0.5f * Vector3.one, animTime).setOnComplete(onBoxClosed);
        }

        protected override void playOpenAnimation()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
            rectTrans.localScale = 0.5f * Vector3.one;
            canvasGroup.alpha = 0;
            LeanTween.alphaCanvas(canvasGroup, 1, animTime);
            LeanTween.scale(rectTrans.gameObject, Vector3.one, animTime).setOnComplete(onOpened).setEaseOutBack();
        }
    }

}
