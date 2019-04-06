using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ϵ��µ����У� ���������ҹ��������
/// </summary>
public class UGUIGridArrangeVertical : UGUIGridArrangeBase
{
    public UGUIGridArrangeVertical(UGUIGridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
    }

    public override void AdjustContentSize()
    {
        int tLineCount = Mathf.CeilToInt((float)mGridWrapContent.mConfig.mDataCnt / (float)mGridWrapContent.mVerticalCnt);
        float tWidth = tLineCount * mGridWrapContent.mCellWidth;

        mGridWrapContent.mRectTransform.sizeDelta = new Vector2(tWidth, 0);
        mGridWrapContent.mRectTransform.anchoredPosition = Vector2.zero;
    }

    public override int GetCellsCountByViewSize()
    {
        float tViewPortWidth = mGridWrapContent.mScrollRectTransform.rect.width;

        int tViewLine = Mathf.CeilToInt(tViewPortWidth / mGridWrapContent.mCellWidth);
        int tTotalLine = tViewLine + UGUIGridArrangeBase.mExtraLine;

        int tCount = tTotalLine * mGridWrapContent.mVerticalCnt;

        return tCount;
    }

    public override Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        int tXIndex = GetXIndexByDataIndex(pDataIndex);
        int tYIndex = pDataIndex % mGridWrapContent.mVerticalCnt;

        float tXPos = tXIndex * mGridWrapContent.mCellWidth;
        float tYPos = -tYIndex * mGridWrapContent.mCellHeight;

        return new Vector2(tXPos, tYPos);
    }

    public override int GetNewStartDataIndex()
    {
        float tOffsetX = -mGridWrapContent.mRectTransform.anchoredPosition.x;
        if (tOffsetX < 0)
            tOffsetX = 0;

        int tOffsetLine = Mathf.FloorToInt(tOffsetX / mGridWrapContent.mCellWidth);

        int tLineIndex = tOffsetLine <= 1 ? 0 : tOffsetLine - 1;

        int tStartIndex = tLineIndex * mGridWrapContent.mVerticalCnt;

        return tStartIndex;
    }

    public override List<int> GetNewDataIndexList()
    {
        int tStartDataIndex = GetNewStartDataIndex();
        int tCellCount = GetCellsCountByViewSize();

        int tDataCount = mGridWrapContent.mConfig.mDataCnt;

        List<int> tNewDataIndexList = new List<int>();
        for (int i = 0; i < tCellCount; ++i)
        {
            int tNewDataIndex = tStartDataIndex + i;
            if (tNewDataIndex >= tDataCount)
                break;

            tNewDataIndexList.Add(tNewDataIndex);
        }

        return tNewDataIndexList;
    }

    public override Vector2 GetFixAnchorPos(int pDataIndex, int pPosType)
    {
        int tXIndex = GetXIndexByDataIndex(pDataIndex);
        float tXPos = -tXIndex * mGridWrapContent.mCellWidth;

        switch (pPosType)
        {
            case (int)UGUIGridWrapContent.FixPosType.Center:
                tXPos = (float)(tXPos + mGridWrapContent.mScrollRectTransform.rect.width * 0.5 - mGridWrapContent.mCellWidth * 0.5);
                break;

            case (int)UGUIGridWrapContent.FixPosType.Last:
                tXPos = (float)(tXPos + mGridWrapContent.mScrollRectTransform.rect.width - mGridWrapContent.mCellWidth);
                break;

            default:
                float tOffsetPos = (pPosType - 1) * mGridWrapContent.mCellWidth;
                tXPos = tXPos + tOffsetPos;
                break;
        }

        float tMinXPos = GetMinXPos();
        float tMaxPos = Mathf.Max(tMinXPos, tXPos);

        return new Vector2(tMaxPos, 0);
    }

    public override Vector2 AdjustAnchorPos(Vector2 pAnchorPos)
    {
        float tMinXPos = GetMinXPos();
        Vector2 tAdjustAnchorPos = pAnchorPos;

        if (tAdjustAnchorPos.x < tMinXPos)
            tAdjustAnchorPos.x = tMinXPos;

        return tAdjustAnchorPos;
    }

    public override int GetMaxDragSupplementIndex()
    {
        int tMaxIndex = Mathf.CeilToInt(mGridWrapContent.mRectTransform.rect.width / mGridWrapContent.mViewPortRectTransform.rect.width);

        return tMaxIndex;
    }

    public override Vector2 GetDragSupplemnetAnchorPos(int pDragSuppleMentIndex)
    {
        float tXPos = -pDragSuppleMentIndex * mGridWrapContent.mViewPortRectTransform.rect.width;
        float tMinXPos = GetMinXPos();

        float tTargetXPos = Mathf.Max(tXPos, tMinXPos);

        return new Vector2(tTargetXPos, 0f);
    }

    #region ��������

    /// <summary>
    /// ��������������ȡ y λ�õ�����
    /// </summary>
    private int GetXIndexByDataIndex(int pDataIndex)
    {
        int tXIndex = Mathf.FloorToInt(pDataIndex / mGridWrapContent.mVerticalCnt);

        return tXIndex;
    }

    /// <summary>
    /// ��ȡ�����õ���С x ֵ
    /// </summary>
    /// <returns></returns>
    public float GetMinXPos()
    {
        float tDataWidth = mGridWrapContent.mRectTransform.rect.width;
        float tViewWidth = mGridWrapContent.mScrollRectTransform.rect.width;

        if (tViewWidth > tDataWidth)
            return 0;

        float tMinXPos = -tDataWidth + mGridWrapContent.mScrollRectTransform.rect.width;

        return tMinXPos;
    }

    #endregion
}
