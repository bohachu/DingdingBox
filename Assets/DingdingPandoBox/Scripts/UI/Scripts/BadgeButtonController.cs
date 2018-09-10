using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cameo.PandoBox
{
    public class BadgeButtonController : MonoBehaviour
    {
        private PandoBoxInfo info;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Image selectLabel;

        [SerializeField]
        private Image finishedLabel;

        [SerializeField]
        private Button badgeButton;

        [SerializeField]
        private Color enableColor;

        [SerializeField]
        private Color disableColor;

        private Action<PandoBoxInfo, bool> onButtonClickedCallback = delegate { };

        private bool isExchangeCompleted
        {
            get
            {
                return RecordCenter.Instance.CheckExchangedCompleted();
            }
        }

        private bool isScanCompleted
        {
            get
            {
                return RecordCenter.Instance.CheckScanCompleted(info.urlLink);
            }
        }

        public void Init(PandoBoxInfo info, Action<PandoBoxInfo, bool> onButtonClickedCallback)
        {
            this.info = info;
            this.onButtonClickedCallback = onButtonClickedCallback;

            badgeButton.interactable = true;//!isExchangeCompleted && isScanCompleted;
            backgroundImage.color = (badgeButton.interactable) ? enableColor : disableColor;
            finishedLabel.gameObject.SetActive(isExchangeCompleted);
            selectLabel.gameObject.SetActive(false);
        }

        public void OnButtonClicked()
        {
            selectLabel.gameObject.SetActive(!selectLabel.gameObject.activeSelf);
            onButtonClickedCallback(info, selectLabel.gameObject.activeSelf);
        }
    }

}
