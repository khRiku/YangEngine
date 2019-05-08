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

        GUI.enabled = Application.isPlaying ? false : true;
        {
            tGridWrapContent.mArrangeType = (UGUIGridWrapContent.ArrangeType) EditorGUILayout.EnumPopup("排列方式(ArrangeType)", tGridWrapContent.mArrangeType);
        }
        GUI.enabled = true;

        if (tGridWrapContent.mArrangeType == UGUIGridWrapContent.ArrangeType.Horizontal)
        {
            GUILayout.Label("已将 ScrooRect 调整为 上下滑动");
            if (Application.isPlaying == false)
            {
                tGridWrapContent.mScrollRect.vertical = true;
                tGridWrapContent.mScrollRect.horizontal = false;
            }
        }
        else
        {
            GUILayout.Label("已将 ScrooRect 调整为 左右滑动");
            if (Application.isPlaying == false)
            {
                tGridWrapContent.mScrollRect.vertical = false;
                tGridWrapContent.mScrollRect.horizontal = true;
            }

        }


        switch (tGridWrapContent.mArrangeType)
        {
            case UGUIGridWrapContent.ArrangeType.Vertical:
                tGridWrapContent.mVerticalCnt = EditorGUILayout.IntField(new GUIContent("Vertical Count", "水平方向，每行有几个"), tGridWrapContent.mVerticalCnt);
                break;

            case UGUIGridWrapContent.ArrangeType.Horizontal:
                tGridWrapContent.mHorizontalCnt = EditorGUILayout.IntField(new GUIContent("Horizontal Count", "垂直方向，每列有几个"), tGridWrapContent.mHorizontalCnt);
                break;

            case UGUIGridWrapContent.ArrangeType.HorizontalPage:
                tGridWrapContent.mVerticalCnt = EditorGUILayout.IntField(new GUIContent("Vertical Count", "水平方向，每行有几个"), tGridWrapContent.mVerticalCnt);
                tGridWrapContent.mHorizontalCnt = EditorGUILayout.IntField(new GUIContent("Horizontal Count", "垂直方向，每列有几个"), tGridWrapContent.mHorizontalCnt);

                break;
        }

        tGridWrapContent.mVerticalCnt = tGridWrapContent.mVerticalCnt < 0 ? 0 : tGridWrapContent.mVerticalCnt;
        tGridWrapContent.mHorizontalCnt = tGridWrapContent.mHorizontalCnt < 0 ? 0 : tGridWrapContent.mHorizontalCnt;

        GUILayout.Space(8f);

        tGridWrapContent.mOffsetX = EditorGUILayout.IntField("X Offset", tGridWrapContent.mOffsetX);
        tGridWrapContent.mOffsetY = EditorGUILayout.IntField("Y Offset", tGridWrapContent.mOffsetY);

        tGridWrapContent.mEnalbDragSupplement = EditorGUILayout.Toggle("开启滑动补足", tGridWrapContent.mEnalbDragSupplement);
        if (tGridWrapContent.mEnalbDragSupplement)
        {
            tGridWrapContent.mDrageSupplementVelocity = EditorGUILayout.Slider("滑动补足速度", tGridWrapContent.mDrageSupplementVelocity, 0f, 1f);
            tGridWrapContent.mDragSupplementViewSizeScale = EditorGUILayout.Slider("滑动可视范围缩放", tGridWrapContent.mDragSupplementViewSizeScale, 0f, 1f);
        }

        if (GUI.changed == true)
        {
            if (Application.isPlaying)
                return;

            tGridWrapContent.RepositionCellInEditor();
        }

        if (GUILayout.Button("重新排序"))
            tGridWrapContent.RepositionCellInEditor();
    }
}
