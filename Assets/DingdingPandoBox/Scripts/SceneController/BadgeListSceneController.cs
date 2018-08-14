using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class BadgeListSceneController : ISceneController
    {
        [SerializeField]
        FadePageAnimator fadePageAnimator;

        public override IEnumerator InitializeCoroutine()
        {
            //TODO: load data from api

            //TODO: construct ui

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
            SceneNavigator.Instance.LoadScene("QRCodeScanner");
        }
    }

}