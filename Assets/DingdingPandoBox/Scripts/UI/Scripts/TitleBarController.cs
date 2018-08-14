using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameo;

public class TitleBarController : MonoBehaviour {

    [SerializeField]
    private RectTransform menuListRectTran;

    [SerializeField]
    private float moveTime = 0.25f;

    [SerializeField]
    private float autoCloseTime = 4.5f;

    private static bool isOpened = false;

    private void Start()
    {
        LeanTween.moveLocalY(menuListRectTran.gameObject, (isOpened) ? 0 : menuListRectTran.sizeDelta.y, 0);

        if (isOpened)
        {
            Invoke("ChangeOpenStatus", autoCloseTime);
        }
    }

    public void ChangeOpenStatus()
    {
        CancelInvoke();

        isOpened = !isOpened;
        LeanTween.moveLocalY(menuListRectTran.gameObject, (isOpened) ? 0 : menuListRectTran.sizeDelta.y, moveTime);

        if(isOpened)
        {
            Invoke("ChangeOpenStatus", autoCloseTime);
        }
    }

    public void OnFloorButtonClicked()
    {
        SceneNavigator.Instance.LoadScene("FloorList", null, false);        
    }

    public void OnBadgeButtonClicked()
    {
        SceneNavigator.Instance.LoadScene("BadgeLIst", null, false);    
    }

    public void OnInfoButtonClicked()
    {
        
    }
}
