using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UGUIGridWrapContent))]
public class UGUIGridWrapContentInspector : Editor
{
    public override void OnInspectorGUI()
    {       
        UGUIGridWrapContent tGridWrapContent = target as UGUIGridWrapContent;
        tGridWrapContent.InspectorInit();

        GUILayout.Space(10f);

        EditorGUILayout.HelpBox("该组件对Cell的排序是从 Cell的父节点 的左上角开始的, 若要调整位置请移动挂载了 ScrollRect 的 GameObject", MessageType.Info);

        tGridWrapContent.mCellWidth = EditorGUILayout.IntField("Cell Width", tGridWrapContent.mCellWidth);
        tGridWrapContent.mCellHeight = EditorGUILayout.IntField("Cell Height", tGridWrapContent.mCellHeight);


        tGridWrapContent.mArrangeType = (UGUIGridWrapContent.ArrangeType)EditorGUILayout.EnumPopup("排列方式(ArrangeType)", tGridWrapContent.mArrangeType);
        if (tGridWrapContent.mArrangeType == UGUIGridWrapContent.ArrangeType.Horizontal)
        {
            GUILayout.Label("已将 ScrooRect 调整为 上下滑动");
            tGridWrapContent.mScrollRect.vertical = true;
            tGridWrapContent.mScrollRect.horizontal = false;
        }
        else
        {
            GUILayout.Label("已将 ScrooRect 调整为 左右滑动");
            tGridWrapContent.mScrollRect.vertical = false;
            tGridWrapContent.mScrollRect.horizontal = true;
        }

        switch (tGridWrapContent.mArrangeType)
        {
            case UGUIGridWrapContent.ArrangeType.Vertical:
                tGridWrapContent.mVerticalCnt = EditorGUILayout.IntField(new GUIContent("Vertical Count", "水平方向，每行有几个"), tGridWrapContent.mVerticalCnt);
                break;

            case UGUIGridWrapContent.ArrangeType.Horizontal:
                tGridWrapContent.mHorizontalCnt = EditorGUILayout.IntField(new GUIContent("Horizontal Count", "垂直方向，每列有几个"), tGridWrapContent.mHorizontalCnt);
                break;

            case UGUIGridWrapContent.ArrangeType.VerticalPage:
                tGridWrapContent.mVerticalCnt = EditorGUILayout.IntField(new GUIContent("Vertical Count", "水平方向，每行有几个"), tGridWrapContent.mVerticalCnt);
                tGridWrapContent.mHorizontalCnt = EditorGUILayout.IntField(new GUIContent("Horizontal Count", "垂直方向，每列有几个"), tGridWrapContent.mHorizontalCnt);

                break;
        }
        tGridWrapContent.mVerticalCnt = tGridWrapContent.mVerticalCnt < 0 ? 0: tGridWrapContent.mVerticalCnt;
        tGridWrapContent.mHorizontalCnt = tGridWrapContent.mHorizontalCnt < 0 ? 0 : tGridWrapContent.mHorizontalCnt;

        //tGridWrapContent.mDragStepSize = EditorGUILayout.IntField("Drag Step Size", tGridWrapContent.mDragStepSize);
        //if (tGridWrapContent.mDragStepSize < 0)
        //    tGridWrapContent.mDragStepSize = 0;

        GUILayout.Space(8f);

        tGridWrapContent.mOffsetX = EditorGUILayout.IntField("X Offset", tGridWrapContent.mOffsetX);
        tGridWrapContent.mOffsetY = EditorGUILayout.IntField("Y Offset", tGridWrapContent.mOffsetY);


        if (GUI.changed == true)
        {
            tGridWrapContent.RepositionCellInEditor();
        }

        if (GUILayout.Button("重新排序"))
            tGridWrapContent.RepositionCellInEditor();
    }
}
