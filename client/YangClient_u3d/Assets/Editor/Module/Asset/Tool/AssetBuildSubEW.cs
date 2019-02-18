using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 资源打包界面
/// </summary>
public class AssetBuildSubEW : BaseEditorWindow
{
    //不需要用到
    //private static AssetBuildSubEW mInstance;
    //[MenuItem(MenuItemNameDefine.mMenuItemOccupyName)]
    //private static void OpenWindow()
    //{
    //    if (mInstance)
    //        return;

    //    mInstance = EditorWindow.CreateInstance<AssetBuildSubEW>();
    //    mInstance.Show();

    //}

    public enum Platform
    {
        Windows,
        IOS,
        Android
    }

    private Platform mPlatform = Platform.Windows;
    private Dictionary<object, bool> mBuileAbOptionDic = new Dictionary<object, bool>();

    private Dictionary<Platform, BuildTarget> mPlatformDic = new Dictionary<Platform, BuildTarget>();

    protected override void Init()
    {
        base.Init();
        InitBuildAssetBUndleOptionsData(); 

        mPlatformDic = new Dictionary<Platform, BuildTarget>()
        {
            {Platform.Windows, BuildTarget.StandaloneWindows },
            {Platform.Android, BuildTarget.Android },
            {Platform.IOS, BuildTarget.iOS },
        };
    }


    private void InitBuildAssetBUndleOptionsData()
    {
        mBuileAbOptionDic = new Dictionary<object, bool>();

        Array tBuildAbOption = Enum.GetValues(typeof(BuildAssetBundleOptions));
        foreach (var tData in tBuildAbOption)
        {
            mBuileAbOptionDic.Add(tData, false);

        }

    }

    protected override void DrawGUI()
    {
        base.DrawGUI();

        mPlatform = (Platform)EditorGUILayout.EnumPopup("目标平台", mPlatform);


        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("打包选项：");

            GUILayout.BeginVertical();
            {
                
                foreach (var tKv in mBuileAbOptionDic.ToList())
                {
                    object tKey = tKv.Key;
                    bool tValue = tKv.Value;

                    string tName = Enum.GetName(typeof(BuildAssetBundleOptions), tKv.Key);
                    mBuileAbOptionDic[tKv.Key] = EditorGUILayout.ToggleLeft(tName, tValue);
                   
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndHorizontal();

        string tExportPath = string.Format("{0}/../AssetBundle/{1}/Res",
                  Application.dataPath,
                  Enum.GetName(typeof(Platform), mPlatform));

        EditorGUILayout.TextField("导出路径：", tExportPath);

        if (GUILayout.Button("生成 Ab 包"))
        {
            BuildAssetBundleOptions tBuildAbOption = BuildAssetBundleOptions.None;

            foreach (var tKv in mBuileAbOptionDic)
            {
                object tKey = tKv.Key;
                bool tValue = tKv.Value;

                if(tValue == false)
                    continue;

                tBuildAbOption |= (BuildAssetBundleOptions)tKey;

            }

            if (Directory.Exists(tExportPath) == false)
                Directory.CreateDirectory(tExportPath);

            BuildPipeline.BuildAssetBundles(tExportPath, tBuildAbOption, mPlatformDic[mPlatform]);
        }
    }


}


