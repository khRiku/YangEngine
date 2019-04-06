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

        EditorGUILayout.HelpBox("�������Cell�������Ǵ� Cell�ĸ��ڵ� �����Ͻǿ�ʼ��, ��Ҫ����λ�����ƶ������� ScrollRect �� GameObject", MessageType.Info);

        tGridWrapContent.mCellWidth = EditorGUILayout.IntField("Cell Width", tGridWrapContent.mCellWidth);
        tGridWrapContent.mCellHeight = EditorGUILayout.IntField("Cell Height", tGridWrapContent.mCellHeight);

        GUI.enabled = Application.isPlaying ? false : true;
        {
            tGridWrapContent.mArrangeType = (UGUIGridWrapContent.ArrangeType)EditorGUILayout.EnumPopup("���з�ʽ(ArrangeType)", tGridWrapContent.mArrangeType);
        }
        GUI.enabled = true;

        if (tGridWrapContent.mArrangeType == UGUIGridWrapContent.ArrangeType.Horizontal)
        {
            GUILayout.Label("�ѽ� ScrooRect ����Ϊ ���»���");
            tGridWrapContent.mScrollRect.vertical = true;
            tGridWrapContent.mScrollRect.horizontal = false;
        }
        else
        {
            GUILayout.Label("�ѽ� ScrooRect ����Ϊ ���һ���");
            tGridWrapContent.mScrollRect.vertical = false;
            tGridWrapContent.mScrollRect.horizontal = true;
        }

        switch (tGridWrapContent.mArrangeType)
        {
            case UGUIGridWrapContent.ArrangeType.Vertical:
                tGridWrapContent.mVerticalCnt = EditorGUILayout.IntField(new GUIContent("Vertical Count", "ˮƽ����ÿ���м���"), tGridWrapContent.mVerticalCnt);
                break;

            case UGUIGridWrapContent.ArrangeType.Horizontal:
                tGridWrapContent.mHorizontalCnt = EditorGUILayout.IntField(new GUIContent("Horizontal Count", "��ֱ����ÿ���м���"), tGridWrapContent.mHorizontalCnt);
                break;

            case UGUIGridWrapContent.ArrangeType.HorizontalPage:
                tGridWrapContent.mVerticalCnt = EditorGUILayout.IntField(new GUIContent("Vertical Count", "ˮƽ����ÿ���м���"), tGridWrapContent.mVerticalCnt);
                tGridWrapContent.mHorizontalCnt = EditorGUILayout.IntField(new GUIContent("Horizontal Count", "��ֱ����ÿ���м���"), tGridWrapContent.mHorizontalCnt);

                break;
        }

        tGridWrapContent.mVerticalCnt = tGridWrapContent.mVerticalCnt < 0 ? 0: tGridWrapContent.mVerticalCnt;
        tGridWrapContent.mHorizontalCnt = tGridWrapContent.mHorizontalCnt < 0 ? 0 : tGridWrapContent.mHorizontalCnt;

        GUILayout.Space(8f);

        tGridWrapContent.mOffsetX = EditorGUILayout.IntField("X Offset", tGridWrapContent.mOffsetX);
        tGridWrapContent.mOffsetY = EditorGUILayout.IntField("Y Offset", tGridWrapContent.mOffsetY);

        tGridWrapContent.mEnalbDragSupplement = EditorGUILayout.Toggle("������������", tGridWrapContent.mEnalbDragSupplement);
        if (tGridWrapContent.mEnalbDragSupplement)
        {
            tGridWrapContent.mDrageSupplementVelocity = EditorGUILayout.Slider("���������ٶ�", tGridWrapContent.mDrageSupplementVelocity,0f, 1f);
            tGridWrapContent.mDragSupplementViewSizeScale = EditorGUILayout.Slider("�������ӷ�Χ����", tGridWrapContent.mDragSupplementViewSizeScale,0f, 1f);
        }

        if (GUI.changed == true)
        {
            if (Application.isPlaying)
                return;

            tGridWrapContent.RepositionCellInEditor();
        }

        if (GUILayout.Button("��������"))
            tGridWrapContent.RepositionCellInEditor();
    }
}
