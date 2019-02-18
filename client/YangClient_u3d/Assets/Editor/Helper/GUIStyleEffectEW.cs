using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;
using UnityEditor;
using UnityEngine;

public class GUIStyleEffectEW : BaseEditorWindow
{
    private static GUIStyleEffectEW mInstance;

    [MenuItem(MenuItemNameDefine.mGUIStyleEffectEW)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<GUIStyleEffectEW>();
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


        mScrollPos = GUILayout.BeginScrollView(mScrollPos, true, true, GUILayout.Width(Screen.width-10), GUILayout.Height(Screen.height-30));
        {
            foreach (var tKv in mStylesDic)
            {
                GUILayout.BeginVertical("GroupBox");
                {
                    GUIStyle tGUIStyle = tKv.Value;

                    EditorGUILayout.TextField("类型名：", tGUIStyle.name);
                    GUILayout.Box("GUILayout.Button", tGUIStyle);
                }
                GUILayout.EndVertical();

                GUILayout.Space(20f);
            }
        }
        GUILayout.EndScrollView();


    }


}