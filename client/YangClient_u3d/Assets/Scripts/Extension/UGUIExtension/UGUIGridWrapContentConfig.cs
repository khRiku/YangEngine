using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UGUIGridWrapContentConfig
{
    public int mDataCnt = 0;
    public Action<int, GameObject> mDisplayCellAction = null;

    public Func<GameObject> mCreateFunc = null;

    public GameObject CreateCell()
    {
        if (mCreateFunc == null)
            return null;

        return mCreateFunc();
    }
}
