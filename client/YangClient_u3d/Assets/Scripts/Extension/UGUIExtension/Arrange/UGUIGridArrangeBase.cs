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
}