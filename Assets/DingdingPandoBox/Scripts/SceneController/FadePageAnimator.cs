using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePageAnimator : MonoBehaviour 
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float Time;

    private void Start()
    {
        canvasGroup.interactable = true;
    }

    public IEnumerator FadeInCoroutine()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, Time);
        yield return new WaitForSeconds(Time);
        canvasGroup.interactable = true;
    }

    public IEnumerator FadeOutCoroutine()
    {
        canvasGroup.interactable = false;
        LeanTween.alphaCanvas(canvasGroup, 0, Time);
        yield return new WaitForSeconds(Time);
    }
}
