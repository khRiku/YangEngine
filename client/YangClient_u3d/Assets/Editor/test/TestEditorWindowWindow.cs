using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;
using UnityEditor;
using UnityEngine;

public class TestEW : BaseEditorWindow
{
    private static TestEW mInstance;

    [MenuItem(MenuItemNameDefine.mTestEW)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<TestEW>();
        mInstance.Show();

    }

    private Dictionary<string, GUIStyle> mStylesDic = new Dictionary<string, GUIStyle>();
    private Vector2 mScrollPos = Vector2.zero;

    protected override void Init()
    {
        base.Init();

        GUISkin tSkin = GUI.skin;
        mStylesDic = tSkin.GetFieldByReflection<Dictionary<string, GUIStyle>>("m_Styles");

    }

    protected override  void DrawGUI()
    {
    
        base.DrawGUI();


    }


}