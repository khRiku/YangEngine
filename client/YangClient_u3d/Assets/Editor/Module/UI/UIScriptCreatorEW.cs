using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// UI脚本创建器
/// @author:yxx
/// date: 2019/2/14
/// </summary>
public class UIScriptCreatorEW : BaseEditorWindow
{
    private static UIScriptCreatorEW mInstance;
    UIScriptCreatorManager mManager;

    [MenuItem(MenuItemNameDefine.mUIScriptCreator)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<UIScriptCreatorEW>();
        mInstance.Show();
    }

    protected override void Init()
    {
        mManager = UIScriptCreatorManager.Instance;

    }

    private Vector2 mScrollPos = Vector2.zero;

    private bool mShowCreateToggle = true;

    protected override void DrawGUI()
    {
        mScrollPos = GUILayout.BeginScrollView(mScrollPos, true, true, GUILayout.Width(Screen.width - 10),GUILayout.Height(Screen.height - 30));
        {
            DrawCreateToggle();

            GUILayout.Label("", "dockareaStandalone");

            DrawUIPrefabInfo();
        }
        GUILayout.EndScrollView();
    }

    private void DrawCreateToggle()
    {
        string tTriangleStr = mShowCreateToggle ? "▼" : "▶";
        if (GUILayout.Button(string.Format("{0}脚本生成", tTriangleStr), "dragtabdropwindow"))
            mShowCreateToggle = !mShowCreateToggle;

        if (mShowCreateToggle == false)
            return;

        mManager.mCreateViewScript = EditorGUILayout.ToggleLeft("创建View代码", mManager.mCreateViewScript);

        GUILayout.Button("创建脚本");
        GUILayout.Space(6f);

    }

    public Rect windowRect = new Rect(0, 0, 200, 200);

    private void DrawUIPrefabInfo()
    {
        GUILayout.Label("▼UI元素选择", "dragtabdropwindow");

        Object tObject = EditorGUILayout.ObjectField("UI Prefab", mManager.mUiPrefab, typeof(GameObject));
        bool tResult = mManager.SetUIPrefab(tObject);
        if(tResult == false && tObject != null)
            ShowNotification(new GUIContent("请设置 Prefab 游戏对象"));

        string tPrefabPath = mManager.mUiPrefab == null
            ? ""
            : PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(mManager.mUiPrefab);
        EditorGUILayout.TextField("Prefab 位置：", tPrefabPath);

        BeginWindows();

        if (windowRect.x == 0)
        {
            windowRect = GUILayoutUtility.GetRect(1,1);
            windowRect.width = 200;
        }
        // All GUI.Window or GUILayout.Window must come inside here
        windowRect = GUILayout.Window(1, windowRect, DoWindow, "Hi There");
        Debug.LogError(windowRect);
        EndWindows();
    }

    void DoWindow(int unusedWindowID)
    {
        GUILayout.Button("Hi");
        GUI.DragWindow();
    }
}
