using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// author: yangxuxiang(YXX)
/// date: 2019/3/09
/// </summary>

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
    private Vector2 mCellOffsetPos = Vector2.zero;

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
    /// HorizontalPage：
    ///     1  2  3  10  11  12
    ///     4  5  6  13  14
    ///     7  8  9
    /// </summary>
    public enum ArrangeType
    {
        Horizontal,         // 从左到右
        Vertical,           // 从上到下
        HorizontalPage,     // 从左到右， 页的形式
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

    //事件监听
    public UGUIGWCEventListenter mUGUIGWCEventListenter { get; private set; }

    public Vector2 mViewSize;              //可视区域的 长宽

    private List<GameObject> mCellList = new List<GameObject>();   //实例化的cell 缓存


    private int mInstaceCellStartDataIndex = 0;    //实例化的cell 的起始数据索引

    private List<int> mDataIndexList = new List<int>(); //存储实例化的cell 中对应的 数据索引
    private Dictionary<int, GameObject> mCellDic = new Dictionary<int, GameObject>();  //key: 数据索引， 


    private Vector3 mPosition = Vector3.zero;
    private bool mPosChange = false;
    #endregion

    #region 事件

    /// 滑动到目标位置后的事件通知
    public Action mScrollToTargetFinishAction { get; private set; } 

    #endregion

    void Awake()
    {
       CacheData();
       RegisterEvent();
    }



    // Update is called once per frame
    void Update()
    {
        //if(mScrollRect.velocity != Vector2.zero)
        //Debug.LogError(mScrollRect.velocity);

        if (mGridArrangeBase == null)
            return;

        //滑动至指定位置的Tween动画
        if (mStartScrollToTargetPos)
            UpdateScrollToTarget();

        //根据位置刷新Cell 的显示
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

        if (mPosChange)
            OnPosEndChange();
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
        if(mInstaceCellStartDataIndex == tInstaceCellStartDataIndex)
            return;
        
        mInstaceCellStartDataIndex = tInstaceCellStartDataIndex;

        //新的数据索引表
        List<int> tNewDataIndexList = mGridArrangeBase.GetNewDataIndexList();

        int tNewI = 0;
        int tOldI = 0;

        int i = 0; 
        while (true)
        {
            ++i;
            if (i > 150)
            {
                Debug.LogError("cell 刷新位置时，循环次数超出150次， 不合理， 已退出");
                return;
            }

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
                tRectTransform.anchoredPosition = GetAnchorPosByDataIndex(tNewDataIndex);
         
                mConfig.mDisplayCellAction(tNewDataIndex, tCell);

                break;
            }
        }
    }



    private void CacheData()
    {
        //缓存组件
        mRectTransform = this.transform as RectTransform;

        mViewPortRectTransform = this.transform.parent as RectTransform;
        mScrollRect = mViewPortRectTransform.parent.GetComponent<ScrollRect>();
        mScrollRectTransform = mScrollRect.transform as RectTransform;

        mUGUIGWCEventListenter =  mScrollRect.GetComponent<UGUIGWCEventListenter>();
        if (mUGUIGWCEventListenter == null)
            mUGUIGWCEventListenter = mScrollRect.gameObject.AddComponent<UGUIGWCEventListenter>();

        //缓存数据
        mViewSize = new Vector2(mViewPortRectTransform.rect.width, mViewPortRectTransform.rect.height);
    }

    #region 事件注册和响应
    private void RegisterEvent()
    {
        mUGUIGWCEventListenter.mOnBeginDrag.AddListener(OnBeginDrag);
        mUGUIGWCEventListenter.mOnEndDrag.AddListener(OnEndDrag);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EndScrollToTargetPos();
        mBeginDragAnchorPos = mRectTransform.anchoredPosition;
        mBeginDragTime = Time.unscaledTime;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 tPosOffset = mRectTransform.anchoredPosition - mBeginDragAnchorPos;
        float tTimeOffset = Time.unscaledTime - mBeginDragTime;

        TrySupplementDrag(tPosOffset, tTimeOffset);
    }

    #endregion


    private UGUIGridArrangeBase GetGridArrangeInstance()
    {
        switch (mArrangeType)
        {
            case ArrangeType.Horizontal:
                return new UGUIGridArrangeHorizontal(this);

            case ArrangeType.HorizontalPage:
                return new UGUIGridArrangeHorizontalPage(this);

            case ArrangeType.Vertical:
                return new UGUIGridArrangeVertical(this);
        }

        Debug.LogError("匹配不到对应的排序算法类  mArrangeType = " + mArrangeType);
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
        int a = 10;
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

        List<int> tNewDataIndexList = mGridArrangeBase.GetNewDataIndexList();

        //创建cell
        int i = 0;
        for (; i < tInstanceCnt; ++i)
        {
            GameObject tCell = null;

            if (i < mCellList.Count)
            {
                tCell = mCellList[i];
                tCell.SetActive(true);
            }
            else
            {
                tCell = mConfig.CreateCell();
                tCell.SetActive(true);

                mCellList.Add(tCell);
            }

            int tDataIndex = tNewDataIndexList[i];
            mDataIndexList.Add(tDataIndex);
            mCellDic.Add(tDataIndex, tCell);

            RectTransform tRectTransform = tCell.transform as RectTransform;

            if (mCellOffsetPos.x == float.MaxValue)
                mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            tRectTransform.anchoredPosition = GetAnchorPosByDataIndex(tDataIndex);

            mConfig.mDisplayCellAction(tDataIndex, tCell);
        }

        //隐藏多余的cell
        for (; i < mCellList.Count; ++i)
        {
            mCellList[i].SetActive(false);
        }
    }

    public Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        Vector2 tGridPos = mGridArrangeBase.GetAnchorPosByDataIndex(pDataIndex);
        Vector2 tAnchorPos = tGridPos + mCellOffsetPos;

        tAnchorPos.x += mOffsetX;
        tAnchorPos.y += mOffsetY;

        return tAnchorPos;
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

        Vector2 tAnchorOffsetPos = new Vector2(mRectTransform.rect.width * tAnchorOffset.x, mRectTransform.rect.height * tAnchorOffset.y);

        Vector2 tTLPivotValue = new Vector2(0, 1);
        Vector2 tPivotOffset = pCell.pivot - tTLPivotValue;

        Vector2 tPivotOffsetPos = new Vector2(tPivotOffset.x * pCell.rect.width, tPivotOffset.y * pCell.rect.height);

        Vector2 tTotalOffsetPos = new Vector2(tAnchorOffsetPos.x + tPivotOffsetPos.x, tAnchorOffsetPos.y + tPivotOffsetPos.y);

        return tTotalOffsetPos;
    }

    #region 滑动补足功能

    public bool mEnalbDragSupplement = false;       //是否开启滑动补足， 范围判断已ViewPort的可视区域为准
    public float mDragSupplementViewSizeScale = 0.7f;       //滑动补足范围缩放, 0f-1f, 缩放可视范围的
    public float mDrageSupplementVelocity = 0.1f;      //补足滑动时的速度调整参数

    public int mDragSupplementIndex { get; private set; }   //当前的滑动补足索引

    private Vector2 mBeginDragAnchorPos = new Vector2(-1f, -1f);
    private float mBeginDragTime = 0f;

    public enum SupplementType
    {
        None,     //不用补足
        Floor,    //向下补足
        Ceil,     //向上补足
    }
    /// <summary>
    /// 尝试补足拖拽
    /// </summary>
    private void TrySupplementDrag(Vector2 pDragPosOffset, float pDragTimeOffset)
    {
        if (mEnalbDragSupplement == false)
            return;

        SupplementType tSupplementType = GetDragSupplementType(pDragPosOffset, pDragTimeOffset);
        int tMaxDragSupplementIndex = mGridArrangeBase.GetMaxDragSupplementIndex();

        int tDragSupplementIndex = mDragSupplementIndex;
        switch (tSupplementType)
        {
            case SupplementType.Ceil:
                tDragSupplementIndex = Mathf.Min(tMaxDragSupplementIndex, tDragSupplementIndex + 1);
                break;

            case SupplementType.Floor:
                tDragSupplementIndex = Mathf.Max(0, tDragSupplementIndex - 1);
                break;
        }

        mDragSupplementIndex = tDragSupplementIndex;

        Vector2 tTargetPos = mGridArrangeBase.GetDragSupplemnetAnchorPos(tDragSupplementIndex);
        StartScrollToTargetPos(tTargetPos, mDrageSupplementVelocity);
      //  Debug.LogError(string.Format("index = {0}  tTargetPos = {1}", tDragSupplementIndex, tTargetPos));
    }

    private SupplementType GetDragSupplementType(Vector2 pDragPosOffset, float pDragTimeOffset)
    {
        float tPosOffset = mScrollRect.vertical ? pDragPosOffset.y : pDragPosOffset.x;
        float tAbsPosOffset = Mathf.Abs(tPosOffset);

        float tViewSize = mScrollRect.vertical ? mViewPortRectTransform.rect.height : mViewPortRectTransform.rect.width;
        tViewSize *= mDragSupplementViewSizeScale;

        //快速滑动的情况
        if (pDragTimeOffset < 0.3f)
        {
            if (mScrollRect.vertical)
            {
                return pDragPosOffset.y > 0 ? SupplementType.Ceil : SupplementType.Floor;
            }
            else
            {
                return pDragPosOffset.x < 0 ? SupplementType.Ceil : SupplementType.Floor;
            }
        }

        //滑动距离的判断
        if(tAbsPosOffset < tViewSize / 2)
            return SupplementType.None;

        if (mScrollRect.vertical)
        {
            return tPosOffset > 0 ? SupplementType.Ceil : SupplementType.Floor;
        }
        else
        {
            return tPosOffset < 0 ? SupplementType.Ceil : SupplementType.Floor;
        }
    }

    #endregion

    #region 滑动至指定位置

    private bool mStartScrollToTargetPos = false;
    private Vector2 mScrollTargetPos;
    private float mScrollStrength;

    private void StartScrollToTargetPos(Vector2 pTargetPos, float pStrength)
    {
        //拖动时， ScrollRect 会记录速度来修改来修改位置， 如果自己要播放滑动动画， 手动停止这速度
        mScrollRect.StopMovement();

        mStartScrollToTargetPos = true;
        mScrollTargetPos = pTargetPos;
        mScrollStrength = pStrength;
    }

    private void EndScrollToTargetPos()
    {
        mStartScrollToTargetPos = false;
    }

    private void UpdateScrollToTarget()
    {
        Vector2 tCurPos = mRectTransform.anchoredPosition;

        if (Vector2.Distance(tCurPos, mScrollTargetPos) < 0.1f)
        {
            EndScrollToTargetPos();
            mRectTransform.anchoredPosition = mScrollTargetPos;

            mScrollToTargetFinishAction?.Invoke();

            return;
        }

        Vector2 tLerpPos = Vector2.Lerp(tCurPos, mScrollTargetPos, mScrollStrength);

        //Debug.LogError(string.Format("CurPos = {0}  mScrollTargetPos = {1}  tLerpPos = {2}",
        //    mRectTransform.anchoredPosition,
        //    mScrollTargetPos,
        //    tLerpPos
        //));

        mRectTransform.anchoredPosition = tLerpPos;


    }

    #endregion


    #region Unity_Editor

#if UNITY_EDITOR

    public void InspectorInit()
    {
        if (mCellOffsetPos != Vector2.zero)
            return;

        CacheData();
        SetDefaultValue();
        InitSeeting();
    }

    public void RepositionCellInEditor()
    {
        //一下{}的代码随便处理的， 否则 UGUIGridArrageHorizontalPage 中会用到 mConfig 然后报错。 后面如有其它问题再看
        {
            this.mConfig = new UGUIGridWrapContentConfig();
            mConfig.mDataCnt = 30;
        }

        if (Application.isPlaying == false)
            mGridArrangeBase = GetGridArrangeInstance();

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            RectTransform tRectTransform = this.transform.GetChild(i) as RectTransform;
            mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            Vector2 tAnchorPos = GetAnchorPosByDataIndex(i);

            tRectTransform.anchoredPosition = tAnchorPos;
        }
    }

#endif
    #endregion

    #region 外部接口 辅助函数

    public enum FixPosType
    {
        Center = -100,     //定位到可视区域的中间
        Last = -200,       //定位到可视区域的最后
    }

    /// <summary>
    /// 定位到指定的数据索引
    /// </summary>
    /// <param name="pDataIndex"></param>
    /// <param name="pPostType">定位到可视区域的第几行, 1 为 第 1 行(列）， 2 为 第 2 行（列）, 特殊位置看 FixPosType </param>
    public void FixToDataIndex(int pDataIndex, float pVelocity = 1f, int pPosType = 1)
    {
        Vector2 tFixAnchorPos = mGridArrangeBase.GetFixAnchorPos(pDataIndex, pPosType);
        StartScrollToTargetPos(tFixAnchorPos, pVelocity);
        
        //mRectTransform.anchoredPosition = tFixAnchorPos;

        //RefreshAllCellPos();
    }

    // <summary>
    /// 改变数据量， 并重新刷新cell 的显示， 会保留当前的滑动状态
    /// </summary>
    /// <param name="pFixToLast">修改后， 自动定位到最后一个</param>
    public void ChangeDataCount(int pCount, bool pFixToLast = false)
    {
        Vector2 tCurPos = mRectTransform.anchoredPosition;

        mConfig.mDataCnt = pCount;
        Show(mConfig);

        if (pFixToLast)
        {
            FixToDataIndex(pCount - 1, (int)FixPosType.Last);
        }
        else
        {
            mRectTransform.anchoredPosition = mGridArrangeBase.AdjustAnchorPos(tCurPos);
        }
    }

    /// <summary>
    ///刷新所有cell 的显示
    /// </summary>
    public void RefreshAll()
    {
        for (int i = 0; i < mDataIndexList.Count; ++i)
        {
            int tDataIndex = mDataIndexList[i];
            GameObject tGo = mCellDic[tDataIndex];
            mConfig.mDisplayCellAction(tDataIndex, tGo);
        }
    }

    /// <summary>
    /// 通过数据索引刷新指定的cell
    /// </summary>
    public void RefrehsByDataIndex(int pDataIndex)
    {
        if (mDataIndexList.Contains(pDataIndex) == false)
            return;

        GameObject pGo = mCellDic[pDataIndex];
        mConfig.mDisplayCellAction(pDataIndex, pGo);
    }

    /// <summary>
    /// 通过GameObject索引刷新指定的cell
    /// </summary>
    public void RefrehsByGameObject(GameObject pGo)
    {
        int tDataIndex = GetDataIndexByGo(pGo);
        if (tDataIndex < 0)
            return;

        mConfig.mDisplayCellAction(tDataIndex, pGo);
    }

    /// <summary>
    /// 根据 Go 来获取数据索引（不同的滑动状态， 同一Go可能会对应不同的DataIndex)
    /// </summary>
    public int GetDataIndexByGo(GameObject pGo)
    {
        foreach (var tKv in mCellDic)
        {
            if (tKv.Value == pGo)
                return tKv.Key;
        }

        return -1;
    }

    /// <summary>
    /// 根据数据索引获取Go, 可能为空(该数据索引暂时没有与之对应的Go)
    /// </summary>
    public GameObject GetGoByDataIndex(int pDataIndex)
    {
        GameObject tGo = null;
        mCellDic.TryGetValue(pDataIndex, out tGo);

        return tGo;
    }
    #endregion


}