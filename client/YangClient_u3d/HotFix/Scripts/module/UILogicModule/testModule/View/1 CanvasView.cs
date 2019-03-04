using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class 1 CanvasView
{
    public GameObject mRootGo { get; private set; }
    public Transform mRootTransform { get; private set; }

    public string mPath = "40 ui/0 test/1 Canvas.prefab";

    public void SetUp(GameObject pRootGo)
    {
        mRootGo = pRootGo;
        mRootTransform = pRootGo.transform;

#if DEBUG || UNITY_EDITOR
        CheckNull();
#endif
        BindElement();
    }

    CanvasRenderer  Text_CanvasRenderer;
    Text  Text_Text;

    
    /// <summary>
    /// 绑定元素
    /// </summary>
    private void BindElement()
    {
        Text_CanvasRenderer = mRootTransform.Find("Text").GetComponent<CanvasRenderer>();
        Text_Text = mRootTransform.Find("Text").GetComponent<Text>();

    }

#if DEBUG || UNITY_EDITOR
    /// <summary>
    /// 检查绑定的元素是否为空
    /// </summary>
    private void CheckNull()
    {
        //key: path, value; tyep name
        Dictionary<string, string> tElementDic = new Dictionary<string, string>()
        {//CheckNullElementStartTag

             {"Text","CanvasRenderer"},
             {"Text","Text"},
   
            
        };//CheckNullElementEndTag

        StringBuilder tSb = new StringBuilder();
        foreach (var tKv in tElementDic)
        {
            string tPath = tKv.Key;
            string tTypeName = tKv.Value;

            Transform tTransform = mRootTransform.Find(tPath);
            if (tTransform == null)
            {
                tSb.AppendLine(string.Format("{0} 的GameObject 找不到", tPath));
                continue;               
            }

            if(tTypeName == "GameObject")
                continue;

            Component tComponent = tTransform.GetComponent(tTypeName);
            if (tComponent == null)
            {
                tSb.AppendLine(string.Format("{0} 上找不到组件 {1}", tPath, tTypeName));
            }
        }

        string tErrorInfo = tSb.ToString();
        if (string.IsNullOrEmpty(tErrorInfo))
        {
            Debug.LogError(string.Format("{0}\n{1}", "1 CanvasView 绑定的元素丢失, 请打开工具检查， 问题元素如下：", tErrorInfo));
        }
    }
#endif
}
