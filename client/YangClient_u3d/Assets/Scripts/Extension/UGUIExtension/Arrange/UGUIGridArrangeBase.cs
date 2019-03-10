using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UGUIGridArrangeBase
{
    public UGUIGridWrapContent mGridWrapContent;

    public UGUIGridArrangeBase(UGUIGridWrapContent pGridWrapContent)
    {
        mGridWrapContent = pGridWrapContent;
    }

    //调整内容区域长宽
    public virtual void AdjustContentSize()
    {
        //设置 Content 的长宽
    }

    public static int mExtraLine = 2;  //在可视区域算出的行或列的基础上再扩展的行或列数

    //根据可视区域的大小， 计算出大小
    public virtual int GetCellsCntByViewSize()
    {
        return 0;
    }

    //根据数据的索引， 算出位置
    public Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        //TODO:YXX

        return Vector2.zero;
    }
}