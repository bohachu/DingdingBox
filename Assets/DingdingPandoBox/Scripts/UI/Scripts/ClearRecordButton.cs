using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// clickTime內按超過clickCount才會觸發
/// </summary>
public class ClearRecordButton : MonoBehaviour {

    [SerializeField]
    private UnityEvent onClicked;

    [SerializeField]
    private int clickCount = 3;

    [SerializeField]
    private float clickTime = 1;

    private int curClickCount = 0;

    public void OnClicked()
    {
        if(curClickCount == 0)
        {
            Invoke("clear", clickTime);
        }
        curClickCount++;

        if(curClickCount >= clickCount)
        {
            onClicked.Invoke();
        }
    }

    private void clear()
    {
        curClickCount = 0;
    }
}
