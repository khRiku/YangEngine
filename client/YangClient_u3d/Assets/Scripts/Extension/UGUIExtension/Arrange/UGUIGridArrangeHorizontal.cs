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
        float tWidth = mGridWrapContent.mViewPortRectTransform.rect.width;

        int tLineCount = Mathf.CeilToInt((float) mGridWrapContent.mConfig.mDataCnt / (float) mGridWrapContent.mHorizontalCnt);
        float tHeight = tLineCount * mGridWrapContent.mCellHeight;

        mGridWrapContent.mRectTransform.sizeDelta = new Vector2(tWidth, tHeight);
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
        int tYIndex = Mathf.FloorToInt(pDataIndex / mGridWrapContent.mHorizontalCnt);

        float tXPos = tXIndex * mGridWrapContent.mCellWidth + mGridWrapContent.mOffsetX;
        float tYPos = -tYIndex * mGridWrapContent.mCellHeight + mGridWrapContent.mOffsetY;

        return new Vector2(tXPos, tYPos);
    }

    public override int GetNewStartDataIndex()
    {
        float tOffsetY = mGridWrapContent.mViewPortRectTransform.anchoredPosition.y;
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
}