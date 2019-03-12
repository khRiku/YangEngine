using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//锚点类型
public enum RectTransformAnchorType
{
    None, 

    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomeCenter,
    BottomRight,

    TopStretch,
    MiddleStretch,
    BottomStretch,

    StretchLeft,
    StretchCenter,
    StretchRight,

    StretchStretch,  

}

public static class RectTransformExtension
{
    //value: key: AnchorMin , Vlaue: AnchorMax
    public static Dictionary<RectTransformAnchorType, KeyValuePair<Vector2, Vector2>> mAnchorTypeDic =
        new Dictionary<RectTransformAnchorType, KeyValuePair<Vector2, Vector2>>()
        {
            //None
            {
                RectTransformAnchorType.None,
                new KeyValuePair<Vector2, Vector2>(new Vector2(float.MaxValue, float.MaxValue), new Vector2(float.MaxValue, float.MaxValue))
            },

            //Top
            {
                RectTransformAnchorType.TopLeft,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 1), new Vector2(0, 1))
            },
            {
                RectTransformAnchorType.TopCenter,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0.5f, 1), new Vector2(0.5f, 1))
            },
            {
                RectTransformAnchorType.TopRight,
                new KeyValuePair<Vector2, Vector2>(new Vector2(1, 1), new Vector2(1, 1))

            },

            //Middle
            {
                RectTransformAnchorType.MiddleLeft,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0.5f), new Vector2(0, 0.5f))
            },
            {
                RectTransformAnchorType.MiddleCenter,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f))
            },
            {
                RectTransformAnchorType.MiddleRight,
                new KeyValuePair<Vector2, Vector2>(new Vector2(1, 0.5f), new Vector2(1, 0.5f))
            },

            //Bottom
            {
                RectTransformAnchorType.BottomLeft,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 0))
            },
            {
                RectTransformAnchorType.BottomeCenter,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0.5f, 0), new Vector2(0.5f, 0))
            },
            {
                RectTransformAnchorType.BottomRight,
                new KeyValuePair<Vector2, Vector2>(new Vector2(1, 0), new Vector2(1, 0))
            },


            //XXXStretch
            {
                RectTransformAnchorType.TopStretch,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 1), new Vector2(1, 1))
            },
            {
                RectTransformAnchorType.MiddleStretch,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0.5f), new Vector2(1, 0.5f))
            },
            {
                RectTransformAnchorType.BottomStretch,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0), new Vector2(1, 0))
            },

            //StretchXXX
            {
                RectTransformAnchorType.StretchLeft,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 1))
            },
            {
                RectTransformAnchorType.StretchCenter,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0.5f, 0), new Vector2(0.5f, 1))
            },
            {
                RectTransformAnchorType.StretchRight,
                new KeyValuePair<Vector2, Vector2>(new Vector2(1, 0), new Vector2(1, 1))
            },

            //Stretch Stretch
            {
                RectTransformAnchorType.StretchStretch,
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0), new Vector2(1, 1))
            },

        };

    /// <summary>
    /// 获取锚点
    /// </summary>
    public static RectTransformAnchorType GetAnchorType(this RectTransform pRectTransform)
    {
        if (pRectTransform == null)
            return RectTransformAnchorType.None;

        foreach (var tKv in mAnchorTypeDic)
        {
            RectTransformAnchorType tAnchorType = tKv.Key;

            Vector2 tAnchorMin = tKv.Value.Key;
            Vector2 tAnchorMax = tKv.Value.Value;

            if (pRectTransform.anchorMin == tAnchorMin && pRectTransform.anchorMax == tAnchorMax)
                return tAnchorType;
        }

        return RectTransformAnchorType.None;
    }

    /// <summary>
    /// 设置锚点
    /// </summary>
    public static void SetAnchorType(this RectTransform pRectTransform,RectTransformAnchorType pAnchorType)
    {
        if (pRectTransform == null)
            return;

        KeyValuePair<Vector2, Vector2> tKv = mAnchorTypeDic[pAnchorType];

        Vector2 tAnchorMin = tKv.Key;
        Vector2 tAnchorMax = tKv.Value;

        pRectTransform.anchorMin = tAnchorMin;
        pRectTransform.anchorMax = tAnchorMax;
    }

    /// <summary>
    /// 获取锚点位置
    /// </summary>
    public static KeyValuePair<Vector2, Vector2> GetAnchorTypeValue( RectTransformAnchorType pAnchorType)
    {
        foreach (var tKv in mAnchorTypeDic)
        {
            RectTransformAnchorType tAnchorType = tKv.Key;
            if (tAnchorType != pAnchorType)
                continue;

            return tKv.Value;
        }

        return mAnchorTypeDic[RectTransformAnchorType.None];
    }

    /// <summary>
    /// 是否是标准锚点， 就是那九个点
    /// </summary>
    public static bool IsStandAnchorType(this RectTransform pRectTransform)
    {
        RectTransformAnchorType tAnchorType = pRectTransform.GetAnchorType();

        if (tAnchorType == RectTransformAnchorType.TopLeft
            || tAnchorType == RectTransformAnchorType.TopCenter
            || tAnchorType == RectTransformAnchorType.TopRight
            || tAnchorType == RectTransformAnchorType.MiddleLeft
            || tAnchorType == RectTransformAnchorType.MiddleCenter
            || tAnchorType == RectTransformAnchorType.MiddleRight
            || tAnchorType == RectTransformAnchorType.BottomLeft
            || tAnchorType == RectTransformAnchorType.BottomeCenter
            || tAnchorType == RectTransformAnchorType.BottomRight
        )
            return true;

        return false;
    }
}
