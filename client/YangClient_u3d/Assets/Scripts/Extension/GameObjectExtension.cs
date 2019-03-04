using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class  GameObjectExtension 
{
    /// <summary>
    /// 获取到指定父节点的位置
    /// </summary>
    /// <returns></returns>
    public static string GetParenRelativePath(this GameObject pChild, GameObject pParent, bool pPathContainParenName)
    {
        if (pChild == null || pParent == null)
            return null;

        List<Transform> tTransformList = new List<Transform>();
        Transform tCurTransform = pChild.transform;
        Transform tParentTransform = pParent.transform;

        while (tCurTransform != tParentTransform)
        {
            tTransformList.Add(tCurTransform);
            tCurTransform = tCurTransform.parent;

            if (tCurTransform == null)
                return null;
        }

        if (tTransformList.Count <= 0)
            return "";

        if(pPathContainParenName)
            tTransformList.Add(tParentTransform);

        StringBuilder tSb = new StringBuilder();
        for (int i = tTransformList.Count - 1; i > 0;  --i)
        {

            tSb.Append(tTransformList[i].name);
            tSb.Append("/");
        }

        tSb.Append(tTransformList[0].name);

        return tSb.ToString();
    }
}
