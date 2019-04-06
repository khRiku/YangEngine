using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 从左到右排序， 到了限定的每行数目就过行
/// 用于 ScrollRect 为上下 滑动的情况 
/// </summary>
public class UGUIGridArrangeHorizontal : UGUIGridArrangeBase
{
    public UGUIGridArrangeHorizontal(UGUIGridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
    }

    public override void AdjustContentSize()
    {
        int tLineCount = Mathf.CeilToInt((float)mGridWrapContent.mConfig.mDataCnt / (float)mGridWrapContent.mHorizontalCnt);
        float tHeight = tLineCount * mGridWrapContent.mCellHeight;

        mGridWrapContent.mRectTransform.sizeDelta = new Vector2(0, tHeight);
        mGridWrapContent.mRectTransform.anchoredPosition = Vector2.zero;
    }

    public override int GetCellsCountByViewSize()
    {
        float tViewPortHeight = mGridWrapContent.mScrollRectTransform.rect.height;

        int tViewLine = Mathf.CeilToInt(tViewPortHeight / mGridWrapContent.mCellHeight);
        int tTotalLine = tViewLine + UGUIGridArrangeBase.mExtraLine;

        int tCount = tTotalLine * mGridWrapContent.mHorizontalCnt;

        return tCount;
    }

    public override Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        int tXIndex = pDataIndex % mGridWrapContent.mHorizontalCnt;
        int tYIndex = GetYindexByDataIndex(pDataIndex);

        float tXPos = tXIndex * mGridWrapContent.mCellWidth;
        float tYPos = -tYIndex * mGridWrapContent.mCellHeight;

        return new Vector2(tXPos, tYPos);
    }

    public override int GetNewStartDataIndex()
    {
        float tOffsetY = mGridWrapContent.mRectTransform.anchoredPosition.y;
        if (tOffsetY < 0)
            tOffsetY = 0;

        int tOffsetLine = Mathf.FloorToInt(tOffsetY / mGridWrapContent.mCellHeight);

        int tLineIndex = tOffsetLine <= 1 ? 0 : tOffsetLine - 1;

        int tStartIndex = tLineIndex * mGridWrapContent.mHorizontalCnt;

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
        int tYIndex = GetYindexByDataIndex(pDataIndex);
        float tYPos = tYIndex * mGridWrapContent.mCellHeight;

        switch (pPosType)
        {
            case (int)UGUIGridWrapContent.FixPosType.Center:
                tYPos = (float)(tYPos - mGridWrapContent.mScrollRectTransform.rect.height * 0.5 + mGridWrapContent.mCellHeight * 0.5);
                break;

            case (int)UGUIGridWrapContent.FixPosType.Last:
                tYPos = (float)(tYPos - mGridWrapContent.mScrollRectTransform.rect.height + mGridWrapContent.mCellHeight);
                break;

            default:
                float tOffsetPos = (pPosType - 1) * mGridWrapContent.mCellHeight;
                tYPos = tYPos - tOffsetPos;
                break;
        }

        float tMaxYPos = GetMaxYPos();
        float tMinPos = Mathf.Min(tMaxYPos, tYPos);

        return new Vector2(0, tMinPos);
    }

    public override Vector2 AdjustAnchorPos(Vector2 pAnchorPos)
    {
        float tMaxYPos = GetMaxYPos();
        Vector2 tAdjustAnchorPos = pAnchorPos;

        if (tAdjustAnchorPos.y > tMaxYPos)
            tAdjustAnchorPos.y = tMaxYPos;

        return tAdjustAnchorPos;
    }

    public override int GetMaxDragSupplementIndex()
    {
        int tCount = Mathf.CeilToInt(mGridWrapContent.mRectTransform.rect.height / mGridWrapContent.mViewPortRectTransform.rect.height);
        int tMaxIndex = tCount - 1;

        return tMaxIndex;
    }

    public override Vector2 GetDragSupplemnetAnchorPos(int pDragSuppleMentIndex)
    {
        float tYPos = pDragSuppleMentIndex * mGridWrapContent.mViewPortRectTransform.rect.height;
        float tMaxYPos = GetMaxYPos();

        float tTargetYPos = Mathf.Min(tYPos, tMaxYPos);

        return new Vector2(0f, tTargetYPos);
    }

    public override int GetDragSupplementIndexByFixPos(Vector2 pFixAnchorPos)
    {
        float tAbsY = Mathf.Abs(pFixAnchorPos.y);

        int tIndex = Mathf.FloorToInt(tAbsY / mGridWrapContent.mViewHeight);

        return tIndex;
    }

    #region 辅助函数

    /// <summary>
    /// 根据数据索引获取 y 位置的索引
    /// </summary>
    private int GetYindexByDataIndex(int pDataIndex)
    {
        int tYIndex = Mathf.FloorToInt(pDataIndex / mGridWrapContent.mHorizontalCnt);

        return tYIndex;
    }

    /// <summary>
    /// 获取可设置的最小 x 值
    /// </summary>
    /// <returns></returns>
    public float GetMaxYPos()
    {
        float tDataHeight = mGridWrapContent.mRectTransform.rect.height;
        float tViewHeight = mGridWrapContent.mScrollRectTransform.rect.height;

        if (tViewHeight > tDataHeight)
            return 0;

        float tMaxYPos = tDataHeight - tViewHeight;

        return tMaxYPos;
    }
    #endregion
}
