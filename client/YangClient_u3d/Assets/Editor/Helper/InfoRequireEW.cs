using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

public class InfoRequireEW : BaseEditorWindow
{
    private static InfoRequireEW mInstance;

    [MenuItem(MenuItemNameDefine.mInfoRequireEW)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<InfoRequireEW>();
        mInstance.Show();

    }

    private Vector2 mScrollPos = Vector2.zero;

    private Dictionary<string, Func<string>> mApiTestDic;

    protected override void Init()
    {
        base.Init();

         mApiTestDic = new Dictionary<string, Func<string>>()
        {
            {"Application.dataPath", ()=>{return Application.dataPath; }},
            {"Application.persistentDataPath", () => { return Application.persistentDataPath;}},
            {"Application.temporaryCachePath", () => { return Application.temporaryCachePath;}},
            {"Application.streamingAssetsPath", () => { return Application.streamingAssetsPath;}},

            {"EditorApplication.applicationPath", () => { return EditorApplication.applicationPath;}},
        };

    }

    protected override  void DrawGUI()
    {
    
        base.DrawGUI();


        mScrollPos = GUILayout.BeginScrollView(mScrollPos, true, true, GUILayout.Width(Screen.width-10), GUILayout.Height(Screen.height-30));
        {
            ShowPathInfo();

        }
        GUILayout.EndScrollView();


    }

    /// <summary>
    /// 显示路径信息
    /// </summary>
    private void ShowPathInfo()
    {
        GUILayout.Label("路径信息");

        foreach (var tKv in mApiTestDic)
        {
            EditorGUILayout.TextField("API：", tKv.Key);
            EditorGUILayout.TextField("效果：", tKv.Value());
            GUILayout.Label("分隔", "ChannelStripAttenuationBar");  //用于分隔用
            GUILayout.Space(3f);
        }


    }

}