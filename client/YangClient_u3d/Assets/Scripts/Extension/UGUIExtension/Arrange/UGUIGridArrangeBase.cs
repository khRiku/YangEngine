using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UGUIGridArrangeBase
{
    public static int mExtraLine = 2;  //�ڿ�������������л��еĻ���������չ���л�����

    public UGUIGridWrapContent mGridWrapContent;

    public UGUIGridArrangeBase(UGUIGridWrapContent pGridWrapContent)
    {
        mGridWrapContent = pGridWrapContent;
    }

    /// <summary>
    /// �����������򳤿�
    /// </summary>
    public abstract void AdjustContentSize();

    /// <summary>
    /// ���ݿ�������Ĵ�С�� �������С
    /// </summary>
    public abstract int GetCellsCountByViewSize();


    /// <summary>
    /// �������ݵ������� ���λ��
    /// </summary>
    public abstract Vector2 GetAnchorPosByDataIndex(int pDataIndex);

    /// <summary>
    /// ���� Content ��λ��ƫ�ƣ� ��ȡ�µ� ��ʼ��������
    /// </summary>
    public abstract int GetNewStartDataIndex();

    /// <summary>
    /// ���� Content ��λ��ƫ�ƣ� ��ȡ�µ����������б�
    /// </summary>
    public abstract List<int> GetNewDataIndexList();

    /// <summary>
    /// ��ȡ Content ��λ�ã���λ��ָ����dataIndex��
    /// </summary>
    public abstract Vector2 GetFixAnchorPos(int pDataIndex, int pPosType);

    /// <summary>
    /// ��������λ��, ȷ��λ�ò���������ֵ
    /// </summary>
    public abstract Vector2 AdjustAnchorPos(Vector2 pAnchorPos);

    /// <summary>
    /// ��ȡ�ɻ�������󻬶���������
    /// </summary>
    public abstract int GetMaxDragSupplementIndex();

    /// <summary>
    /// ��ȡ������Ӧ��λ��
    /// </summary>
    public abstract Vector2 GetDragSupplemnetAnchorPos(int pDragSuppleMentIndex);

    //����λ��λ�õ���Ϊ���������λ��
    public abstract int GetDragSupplementIndexByFixPos(Vector2 pFixAnchorPos);
}