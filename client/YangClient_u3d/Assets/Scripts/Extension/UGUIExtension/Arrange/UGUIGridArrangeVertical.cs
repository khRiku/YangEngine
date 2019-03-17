using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 从上到下的排列， 适用于左右滚动的情况
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

        int tCount = tTotalLine * mGridWrapContent.mHorizontalCnt;

        return tCount;
    }

    public override Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        int tXIndex = GetXIndexByDataIndex(pDataIndex);
        int tYIndex = pDataIndex % mGridWrapContent.mVerticalCnt;

        float tXPos = tXIndex * mGridWrapContent.mCellWidth + mGridWrapContent.mOffsetX;
        float tYPos = -tYIndex * mGridWrapContent.mCellHeight + mGridWrapContent.mOffsetY;

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

        float tMinXPos = -mGridWrapContent.mRectTransform.rect.width
                         + mGridWrapContent.mScrollRectTransform.rect.width;

        float tMaxPos = Mathf.Max(tMinXPos, tXPos);

        return new Vector2(tMaxPos, 0);
    }

    #region 辅助函数

    /// <summary>
    /// 根据数据索引获取 y 位置的索引
    /// </summary>
    private int GetXIndexByDataIndex(int pDataIndex)
    {
        int tXIndex = Mathf.FloorToInt(pDataIndex / mGridWrapContent.mVerticalCnt);

        return tXIndex;
    }

    #endregion
}
