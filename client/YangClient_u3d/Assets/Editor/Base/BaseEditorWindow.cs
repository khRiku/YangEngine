using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;
using UnityEditor;
using UnityEngine;

public class BaseEditorWindow : EditorWindow
{
    private System.Object mInitObject;

    public void OnGUI()
    {
        if (mInitObject == null)
        {
            mInitObject = new System.Object();
            Init();

        }
         

        DrawGUI();
    }

    /// <summary>
    /// 初始化界面相关设置
    /// </summary>
    protected virtual void Init()
    {

    }

    /// <summary>
    /// 绘制UI
    /// </summary>
    protected virtual void DrawGUI()
    {

    }

}
