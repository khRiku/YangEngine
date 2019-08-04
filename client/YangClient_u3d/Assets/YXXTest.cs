using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.UI;

public class YXXTest : MonoBehaviour
{
    [ContextMenu("输出坐标位置")]
    void PrintInfo()
    {
        RectTransform tRectTransform = transform as RectTransform;

        Debug.LogError(tRectTransform.position);
        Debug.LogError(tRectTransform.localPosition);
        Debug.LogError(tRectTransform.anchoredPosition);
        Debug.LogError(tRectTransform.anchoredPosition3D);
        Debug.LogError("*********************************");
    }

    [ContextMenu("输出长宽")]
    void PrintSize()
    {
        RectTransform tRectTransform = transform as RectTransform;

        Debug.LogError(tRectTransform.sizeDelta);
        Debug.LogError(tRectTransform.localPosition);
        Debug.LogError(tRectTransform.anchoredPosition);
        Debug.LogError(tRectTransform.anchoredPosition3D);
        Debug.LogError("*********************************");
    }
}
