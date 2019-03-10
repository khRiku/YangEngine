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
    public RectTransform mRectTransform;
    public RectTransform mViewPortRectTransform;

    public Vector2 mViewSize;              //可视区域的 长宽

    private List<GameObject> mCellList = new List<GameObject>();   //实例化的cell 缓存


    #endregion

    void Awake()
    {
        Init();
    }


    private Vector3 mPosition = Vector3.zero;
    private bool mPosChange = false;

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
           // Debug.LogError("Positin 停止改变, 位置 = ");

        }
    }

    private void OnPosEndChange()
    {

    }

    private void Init()
    {
        CacheData();

    }

    void RegisterEvent()
    {

    }


    private void CacheData()
    {
        //缓存组件
        mRectTransform = this.transform as RectTransform;
        mViewPortRectTransform = this.transform.parent as RectTransform;
        mScrollRect = mViewPortRectTransform.parent.GetComponent<ScrollRect>();

        if (mScrollRect == null)
            Debug.LogError(string.Format("{0} Init 函数中，有必要组件为空", this.GetType().Name));

        //缓存数据
        mViewSize = new Vector2(mViewPortRectTransform.rect.width, mViewPortRectTransform.rect.height);
    }

    private UGUIGridArrangeBase GetGridArrangeInstance()
    {
        //YXX 未完成
        switch (mArrangeType)
        {
            case ArrangeType.Horizontal:
                return null;

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

        mGridArrangeBase = GetGridArrangeInstance();

        ResetContenPos();
        mGridArrangeBase.AdjustContentSize();
        ChangeCellCount();
    }

    //将cell 的父节点 坐标位置重置 
    private void ResetContenPos()
    {
        mRectTransform.anchoredPosition3D = Vector3.zero;
        mRectTransform.anchorMin = new Vector2(0, 1);
        mRectTransform.anchorMax = new Vector2(0, 1);
    }

    //修改需实例化的cell 数量
    private void ChangeCellCount()
    {
        int tViewCnt = mGridArrangeBase.GetCellsCntByViewSize();
        int tInstanceCnt = Mathf.Max(mConfig.mDataCnt, tViewCnt);

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
                mCellList.Add(tCell);
            }

            RectTransform tRectTransform = tCell.transform as RectTransform;
            tRectTransform.anchoredPosition = mGridArrangeBase.GetAnchorPosByDataIndex(i);

            mConfig.mDisplayCellAction(i, tCell);
        }

        //隐藏多余的cell
        for (; i < mCellList.Count; ++i)
        {
            mCellList[i].SetActive(false);
        }
    }
}