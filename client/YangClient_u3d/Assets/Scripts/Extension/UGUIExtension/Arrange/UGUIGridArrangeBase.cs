using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UGUIGridArrangeBase
{
    public static int mExtraLine = 2;  //在可视区域算出的行或列的基础上再扩展的行或列数

    public UGUIGridWrapContent mGridWrapContent;

    public UGUIGridArrangeBase(UGUIGridWrapContent pGridWrapContent)
    {
        mGridWrapContent = pGridWrapContent;
    }

    /// <summary>
    /// 调整内容区域长宽
    /// </summary>
    public abstract void AdjustContentSize();

    /// <summary>
    /// 根据可视区域的大小， 计算出大小
    /// </summary>
    public abstract int GetCellsCountByViewSize();


    /// <summary>
    /// 根据数据的索引， 算出位置
    /// </summary>
    public abstract Vector2 GetAnchorPosByDataIndex(int pDataIndex);

    /// <summary>
    /// 根据 Content 的位置偏移， 获取新的 起始数据索引
    /// </summary>
    public abstract int GetNewStartDataIndex();

    /// <summary>
    /// 根据 Content 的位置偏移， 获取新的数据索引列表
    /// </summary>
    public abstract List<int> GetNewDataIndexList();

    /// <summary>
    /// 获取 Content 的位置（定位到指定的dataIndex）
    /// </summary>
    public abstract Vector2 GetFixAnchorPos(int pDataIndex, int pPosType);

    /// <summary>
    /// 调整坐标位置, 确保位置不超出合理值
    /// </summary>
    public abstract Vector2 AdjustAnchorPos(Vector2 pAnchorPos);

    /// <summary>
    /// 获取可滑动的最大滑动补足索引
    /// </summary>
    public abstract int GetMaxDragSupplementIndex();

    /// <summary>
    /// 获取索引对应的位置
    /// </summary>
    public abstract Vector2 GetDragSupplemnetAnchorPos(int pDragSuppleMentIndex);

    //将定位的位置调整为滑动补足的位置
    public abstract int GetDragSupplementIndexByFixPos(Vector2 pFixAnchorPos);
}