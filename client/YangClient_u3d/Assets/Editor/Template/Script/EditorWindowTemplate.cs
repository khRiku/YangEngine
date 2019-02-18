using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorWindowTemplate : BaseEditorWindow {

    private static EditorWindowTemplate mInstance;

    [MenuItem(MenuItemNameDefine.mOccupyName)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<EditorWindowTemplate>();
        mInstance.Show();
    }


    protected override void Init()
    {
        base.Init();


    }

    protected override void DrawGUI()
    {
        base.DrawGUI();


    }
}
