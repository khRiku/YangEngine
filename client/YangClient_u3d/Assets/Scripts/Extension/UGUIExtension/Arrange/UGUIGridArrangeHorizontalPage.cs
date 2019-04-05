using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//左右排列， 页的形式， 跟手机的一样， 适用于左右滚动的情况
public class UGUIGridArrangeHorizontalPage : UGUIGridArrangeBase
{
    public int mPageCellCount { get; private set; }      //每页有多少个cell
    public int mPageCount;              //有多少页
    public Vector2 mPageSize;           //每一页的长宽


    public UGUIGridArrangeHorizontalPage(UGUIGridWrapContent pGridWrapContent) : base(pGridWrapContent)
    {
        mPageCellCount = mGridWrapContent.mVerticalCnt * mGridWrapContent.mHorizontalCnt;
        mPageCount = Mathf.CeilToInt((float)mGridWrapContent.mConfig.mDataCnt / (float)mPageCellCount);
        mPageSize = new Vector2( mGridWrapContent.mHorizontalCnt * mGridWrapContent.mCellWidth,mGridWrapContent.mVerticalCnt * mGridWrapContent.mCellHeight );
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

        int tCount = tTotalLine * mGridWrapContent.mVerticalCnt;

        return tCount;
    }

    public override Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        int tPageIndex = Mathf.CeilToInt(pDataIndex / mPageCellCount);       //页索引
        int tLastPageXIndex = pDataIndex % mGridWrapContent.mHorizontalCnt;  //以最后一页为基准的 X 索引
        int tLastPageYIndex = Mathf.CeilToInt((pDataIndex - tPageIndex * mPageCellCount)/mGridWrapContent.mHorizontalCnt);       //以最后一页为基准的 Y 索引

        float tXPos = tPageIndex * mPageSize.x + tLastPageXIndex * mGridWrapContent.mCellWidth;
        float tYPos = -tLastPageYIndex * mGridWrapContent.mCellHeight;

        return new Vector2(tXPos, tYPos);
    }

    public override int GetNewStartDataIndex()
    {
        float tOffsetX = -mGridWrapContent.mRectTransform.anchoredPosition.x;
        if (tOffsetX < 0)
            tOffsetX = 0;

        int tOffsetLine = Mathf.FloorToInt(tOffsetX / mGridWrapContent.mCellWidth);
        if (tOffsetLine <= 1)
            tOffsetLine = 0;

        int tOffsetPageIndex = Mathf.FloorToInt(tOffsetLine / mGridWrapContent.mHorizontalCnt);
        int tLastPageXIndex = tOffsetLine % mGridWrapContent.mHorizontalCnt;


        int tStartIndex = tOffsetPageIndex * mPageCellCount + tLastPageXIndex;

        return tStartIndex;
    }

    public override List<int> GetNewDataIndexList()
    {
        int tDataCount = mGridWrapContent.mConfig.mDataCnt;

        int tStartDataIndex = GetNewStartDataIndex();
        Vector2 tStartAnchorPos = GetAnchorPosByDataIndex(tStartDataIndex);
        Debug.LogError("起始index = " + tStartDataIndex);
        int tCellCount = GetCellsCountByViewSize();
        List<int> tNewDataIndexList = new List<int>();
        for (int i = 0; i < tCellCount; ++i)
        {
            int tXAddPos = Mathf.FloorToInt((float)i / (float)mGridWrapContent.mVerticalCnt);
            int tYAddPos = i % mGridWrapContent.mVerticalCnt;

            tStartAnchorPos.x += tXAddPos * mGridWrapContent.mCellWidth;
            tStartAnchorPos.y += tYAddPos * mGridWrapContent.mCellHeight;

            int tNewDataIndex = GetDataIndexByPos(tStartAnchorPos);
          
            if (tNewDataIndex >= tDataCount)
            {
                Debug.LogError("超过的 dataindex = " + tNewDataIndex);
                break;
            }
            Debug.LogError("i = " + i + "新的 dataindex = " + tNewDataIndex + "   tStartAnchorPos = " + tStartAnchorPos);

            tNewDataIndexList.Add(tNewDataIndex);
        }
        return tNewDataIndexList;
    }

    public int GetDataIndexByPos(Vector2 pAnchorPos)
    {
        int tPageCount = Mathf.FloorToInt(pAnchorPos.x / mPageSize.x);
        int tLastPageXCount =Mathf.FloorToInt((pAnchorPos.x - tPageCount * mPageSize.x)/mGridWrapContent.mCellWidth);
        int tLastPageYCount =Mathf.FloorToInt(pAnchorPos.y /mGridWrapContent.mCellHeight);

        int tNewDataIndex = tPageCount * mPageCount + tLastPageXCount + tLastPageYCount * mGridWrapContent.mHorizontalCnt;

        return tNewDataIndex;
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

    #region 辅助函数

    /// <summary>
    /// 根据数据索引获取 y 位置的索引
    /// </summary>
    private int GetXIndexByDataIndex(int pDataIndex)
    {
        int tPageIndex = Mathf.FloorToInt(pDataIndex / mPageCellCount);
        int tXIndex = tPageIndex * mGridWrapContent.mHorizontalCnt + pDataIndex % mGridWrapContent.mHorizontalCnt;

        return tXIndex;
    }

    /// <summary>
    /// 获取可设置的最小 x 值
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

