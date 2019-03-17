using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//左右排列， 页的形式， 跟手机的一样， 适用于左右滚动的情况
public class UGUIGridArrangeHorizontalPage : UGUIGridArrangeBase
{
    public int mPageCellCount { get; private set; }      //每页有多少个cell

    //有多少页
    public int mPageCount
    {
        get
        {
            if (mGridWrapContent == null || mGridWrapContent.mConfig == null || mGridWrapContent.mConfig.mDataCnt <= 0)
                return 0;

            return Mathf.CeilToInt(mGridWrapContent.mConfig.mDataCnt / mPageCellCount);
        }
    }       

    public UGUIGridArrangeHorizontalPage(UGUIGridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
        mPageCellCount = mGridWrapContent.mVerticalCnt * mGridWrapContent.mHorizontalCnt;
    }

    public override void AdjustContentSize()
    {
        int tLineCount = mPageCount * mGridWrapContent.mHorizontalCnt;
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
       //TODO:YXX  做到这个函数， 后面开始处理这个函数
        int tXIndex = pDataIndex % mGridWrapContent.mHorizontalCnt;
        int tYIndex = GetYindexByDataIndex(pDataIndex);

        float tXPos = tXIndex * mGridWrapContent.mCellWidth + mGridWrapContent.mOffsetX;
        float tYPos = -tYIndex * mGridWrapContent.mCellHeight + mGridWrapContent.mOffsetY;

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

        float tMaxYPos = mGridWrapContent.mRectTransform.rect.height
                         - mGridWrapContent.mScrollRectTransform.rect.height;

        float tMinPos = Mathf.Min(tMaxYPos, tYPos);

        return new Vector2(0, tMinPos);
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

    #endregion
}
