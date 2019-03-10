using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UGUIGridWrapContentConfig
{
    public int mDataCnt = 0;

    public Action<int, GameObject> mDisplayCellAction = null;

    public GameObject CreateCell()
    {
        //Yxx 创建cell
        return null;
    }
}
