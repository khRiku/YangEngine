using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.UI;


/*默认ui排版要求
   ScrollView  对齐点无要求
     Viewport  被ScrollRect 设置了， 匹配 ScrollView 的长宽， 
               如果挂有滚动条, ViewPort 的长宽会减去， 滚动条占用的空间， 保证滚动条不被裁剪
       Content   锚点不被限制到， 但请设置为左上角

    Ps: UGUIGridWrapContent 脚本挂在Content 上， 上面的Go, 名字自己随便改， 上面的只是 ugui 创建时的默认命名
*/

public class UGUIGridWrapContent : MonoBehaviour
{
    #region  排版相关

    public int mOffsetX = 0;           //左上角 x 位置偏移
    public int mOffsetY = 0;           //左上角 y 位置偏移

    public int mCellWidth = 100;       //单个小UI 宽度
    public int mCellHeight = 100;      //单个小UI 高度

    public int mHorizontalCnt = 0;     //水平方向， 每行有几个
    public int mVerticalCnt = 0;       //垂直方向， 每列有几个


    //每个用户在制作 cell 的时候，锚点的情况多种多样，只要是标准的锚点类型, 可计算出偏移值， 屏蔽掉这个差异
    private Vector2 mCellOffsetPos;

    /// <summary>
    /// 排列类型
    /// Horizontal:
    ///     1  2  3
    ///     4  5  6
    /// 
    /// Vertical:
    ///     1  4
    ///     2  5
    ///     3  6
    ///
    /// VerticalPage：
    ///     1  2  3  10  11  12
    ///     4  5  6  13  14
    ///     7  8  9
    /// </summary>
    public enum ArrangeType
    {
        Horizontal,       // 从左到右
        Vertical,         // 从上到下
        VerticalPage,     // 从左到右， 页的形式
    }
    #endregion

    public ArrangeType mArrangeType;
    public UGUIGridWrapContentConfig mConfig;
    public UGUIGridArrangeBase mGridArrangeBase;

    #region 数据缓存 

    public ScrollRect mScrollRect;
    public RectTransform mScrollRectTransform;
    public RectTransform mRectTransform;
    public RectTransform mViewPortRectTransform;

    public Vector2 mViewSize;              //可视区域的 长宽

    private List<GameObject> mCellList = new List<GameObject>();   //实例化的cell 缓存


    private int mInstaceCellStartDataIndex = 0;    //实例化的cell 的起始数据索引

    private List<int> mDataIndexList = new List<int>(); //存储实例化的cell 中对应的 数据索引
    private Dictionary<int, GameObject> mCellDic = new Dictionary<int, GameObject>();  //key: 数据索引， 


    private Vector3 mPosition = Vector3.zero;
    private bool mPosChange = false;

    #endregion

    void Awake()
    {
       CacheData();
    }

    // Update is called once per frame
    void Update()
    {
        if (mPosition != this.transform.position)
        {
            //if (mPosChange == false)
            //    Debug.LogError("Positin 改变, 位置 = ");

            mPosChange = true;
            mPosition = this.transform.position;
        }
        else if (mPosition == this.transform.position && mPosChange)
        {
            mPosChange = false;
            OnPosEndChange();
            // Debug.LogError("Positin 停止改变, 位置 = ");

        }
    }

    private void OnPosEndChange()
    {
        RefreshAllCellPos();
    }

    //刷新 Cell 的位置
    private void RefreshAllCellPos()
    {
        //起始点不变的情况， 位置也不需要改变， 直接返回
        int tInstaceCellStartDataIndex = mGridArrangeBase.GetNewStartDataIndex();
        if (tInstaceCellStartDataIndex == mInstaceCellStartDataIndex)
            return;

        //新的数据索引表
        List<int> tNewDataIndexList = mGridArrangeBase.GetNewDataIndexList();

        int tNewI = 0;
        int tOldI = 0;

        while (true)
        {
            //找出新的 数据索引
            int tNewDataIndex = -1;
            for (; tNewI < tNewDataIndexList.Count; ++tNewI)
            {
                int tDataIndex = tNewDataIndexList[tNewI];

                //新的索引在老的也有， 不用处理
                if (mCellDic.ContainsKey(tDataIndex))
                    continue;

                tNewDataIndex = tDataIndex;
                break;
            }

            //没新的直接返回
            if (tNewDataIndex == -1)
                break;

            //将无用的数据索引替换为新的
            for (; tOldI < mDataIndexList.Count; ++tOldI)
            {
                int tOldDataIndex = mDataIndexList[tOldI];

                //老的索引在新的也有， 不用处理
                if (tNewDataIndexList.Contains(tOldDataIndex))
                    continue;

                mDataIndexList[tOldI] = tNewDataIndex;
                GameObject tCell = mCellDic[tOldDataIndex];

                mCellDic.Remove(tOldDataIndex);
                mCellDic.Add(tNewDataIndex, tCell);

                RectTransform tRectTransform = tCell.transform as RectTransform;
                tRectTransform.anchoredPosition = mGridArrangeBase.GetAnchorPosByDataIndex(tNewDataIndex);

                mConfig.mDisplayCellAction(tNewDataIndex, tCell);
            }
        }
    }

    private void CacheData()
    {
        //缓存组件
        mRectTransform = this.transform as RectTransform;

        mViewPortRectTransform = this.transform.parent as RectTransform;
        mScrollRect = mViewPortRectTransform.parent.GetComponent<ScrollRect>();
        mScrollRectTransform = mScrollRectTransform.transform as RectTransform;

        //缓存数据
        mViewSize = new Vector2(mViewPortRectTransform.rect.width, mViewPortRectTransform.rect.height);
    }

    private UGUIGridArrangeBase GetGridArrangeInstance()
    {
        //TODO:YXX 未完成
        switch (mArrangeType)
        {
            case ArrangeType.Horizontal:
                return new UGUIGridArrangeHorizontal(this);

            case ArrangeType.Vertical:
                return null;

            case ArrangeType.VerticalPage:
                return null;
        }

        return null;
    }

    public void Show(UGUIGridWrapContentConfig pConfig)
    {
        mConfig = pConfig;
        SetDefaultValue();

        mGridArrangeBase = GetGridArrangeInstance();

        InitSeeting();

        mGridArrangeBase.AdjustContentSize();
        CreateAllCellInstance();
    }

    //设置一些默认值
    private void SetDefaultValue()
    {
        if (mCellHeight <= 0)
            mCellHeight = 10;

        if (mCellWidth <= 10)
            mCellWidth = 1;

        if (mHorizontalCnt <= 0)
            mHorizontalCnt = 1;

        if (mVerticalCnt <= 0 )
            mVerticalCnt = 1;
    }

    //初始化 Content 的一些设置
    private void InitSeeting()
    {
        mRectTransform.pivot = new Vector2(0, 1);

        mCellOffsetPos = new Vector2(float.MaxValue, float.MaxValue);
    }

    //修改需实例化的cell 数量
    private void CreateAllCellInstance()
    {
        int tViewCnt = mGridArrangeBase.GetCellsCountByViewSize();
        int tInstanceCnt = Mathf.Min(mConfig.mDataCnt, tViewCnt);

        mInstaceCellStartDataIndex = 0;
        mDataIndexList.Clear();
        mCellDic.Clear();

        //创建cell
        int i = 0;
        for (; i < tInstanceCnt; ++i)
        {
            GameObject tCell = mConfig.CreateCell();
            tCell.SetActive(true);

            if (i < mCellList.Count)
            {
                tCell = mCellList[i];
                tCell.SetActive(true);
            }
            else
            {
                tCell = mConfig.CreateCell();
                mCellList.Add(tCell);
            }

            mDataIndexList.Add(i);
            mCellDic.Add(i, tCell);

            RectTransform tRectTransform = tCell.transform as RectTransform;
            Vector2 tGridPos = mGridArrangeBase.GetAnchorPosByDataIndex(i);

            if (mCellOffsetPos.x == float.MaxValue)
                mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            tRectTransform.anchoredPosition =  tGridPos + mCellOffsetPos;

            mConfig.mDisplayCellAction(i, tCell);
        }

        //隐藏多余的cell
        for (; i < mCellList.Count; ++i)
        {
            mCellList[i].SetActive(false);
        }
    }

    //获取cell 的位置偏移， 目的是使其对位是content 的左上角
    private Vector2 GetCellOffsetValue(RectTransform pCell)
    {
#if UNITY_EDITOR
        if (pCell.IsStandAnchorType() == false)
        {
            Debug.LogError("cell 的锚点必须是标准的形式， 就是TopLeft, TopCenter ... ... 这种");
        }
#endif
        RectTransformAnchorType tAnchorType = pCell.GetAnchorType();
        Vector2 tAnchorValue = RectTransformExtension.GetAnchorTypeValue(tAnchorType).Key;
        Vector2 tTLAnchorValue = RectTransformExtension.GetAnchorTypeValue(RectTransformAnchorType.TopLeft).Key;
        Vector2 tAnchorOffset = tTLAnchorValue - tAnchorValue;

        Vector2 tAnchorOffsetPos = new Vector2(mRectTransform.rect.width * tAnchorOffset.x,
           mRectTransform.rect.height * tAnchorOffset.y);

        Vector2 tTLPivotValue = new Vector2(0, 1);
        Vector2 tPivotOffset = pCell.pivot - tTLPivotValue;

        Vector2 tPivotOffsetPos = new Vector2(tPivotOffset.x * pCell.rect.width,
            tPivotOffset.y * pCell.rect.height);

        Vector2 tTotalOffsetPos = new Vector2(tAnchorOffsetPos.x + tPivotOffsetPos.x,
            tAnchorOffsetPos.y + tPivotOffsetPos.y);

        return tTotalOffsetPos;
    }

    #region Unity_Editor

#if UNITY_EDITOR
  
    private bool mInspectorInit = false;
    public void InspectorInit()
    {
        if (mInspectorInit)
            return;

        mInspectorInit = true;

        CacheData();
        SetDefaultValue();
        InitSeeting();
    }

    public void RepositionCellInEditor()
    {
        UGUIGridArrangeBase tGridArrangeBase = GetGridArrangeInstance();

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            RectTransform tRectTransform = this.transform.GetChild(i) as RectTransform;

            Vector2 tGridPos = tGridArrangeBase.GetAnchorPosByDataIndex(i);
            mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            tRectTransform.anchoredPosition = tGridPos + mCellOffsetPos;
        }

    }

#endif
    #endregion
}