using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

public class AssetMainAssetEW : BaseEditorWindow
{
    private static AssetMainAssetEW mInstance;

    [MenuItem(MenuItemNameDefine.mAssetEW)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<AssetMainAssetEW>();
        mInstance.Show();
    }

    public enum ContentType
    {
        Build = 0,
        Info,
        Update,
    }

    private string[] mContentTypeNameArr;
    private int mTopTabSlectIndex = 0;

    private Dictionary<ContentType, BaseEditorWindow> mSubEWDic = new Dictionary<ContentType, BaseEditorWindow>();
    protected override void Init()
    {
        base.Init();

        mContentTypeNameArr = Enum.GetNames(typeof(ContentType));

        mSubEWDic.Add(ContentType.Build, new AssetBuildSubEW());
        mSubEWDic.Add(ContentType.Info, null);
        mSubEWDic.Add(ContentType.Update, null);


    }

    protected override void DrawGUI()
    {
        base.DrawGUI();

        mTopTabSlectIndex = GUILayout.Toolbar(mTopTabSlectIndex, mContentTypeNameArr);

        ShowSubView();
    }

    protected void ShowSubView()
    {

        ContentType tContentType = (ContentType) mTopTabSlectIndex;
        BaseEditorWindow tSubEW = mSubEWDic[tContentType];


        if (tSubEW == null)
        {
            GUILayout.Label("相关功能未实现,待扩展", "NotificationBackground");
            return;

        }

        tSubEW.OnGUI();
    }


}


