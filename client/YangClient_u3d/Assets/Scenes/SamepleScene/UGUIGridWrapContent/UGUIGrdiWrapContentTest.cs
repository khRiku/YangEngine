
using UnityEditor;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;
using  System.Collections.Generic;
using UnityEngine.UI;

class UGUIGrdiWrapContentTest : MonoBehaviour
{
    public GameObject mCell;
    private UGUIGridWrapContent mGridWrapContent;

    private void Start()
    {
        mGridWrapContent = this.GetComponent<UGUIGridWrapContent>();

        UGUIGridWrapContentConfig tConfig = new UGUIGridWrapContentConfig()
        {
            mDataCnt = this.mDataCount,
            mDisplayCellAction = DisplayCell,
            mCreateFunc = () => { return GameObject.Instantiate(mCell, this.transform); }
        };

        mGridWrapContent.Show(tConfig);
    }

    private void DisplayCell(int pDataIndex, GameObject pGo)
    {
        Text tText = pGo.transform.GetChild(0).transform.GetComponent<Text>();
        if (tText == null)
            return;

        tText.text = pDataIndex.ToString();
    }

    public int mDataCount = 70;
    [ContextMenu("改变数量")]
    private void ChangeCount()
    {
        UGUIGridWrapContentConfig tConfig = mGridWrapContent.mConfig;

        tConfig.mDataCnt = mDataCount;
        mGridWrapContent.Show(tConfig);
    }

    public int mFixTo = 1;
    public int mPosType;
    [ContextMenu("定位")]
    private void FixTo()
    {
        mGridWrapContent.FixToDataIndex(mFixTo, mPosType);
    }
}