
using UnityEditor;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;
using  System.Collections.Generic;
using UnityEngine.UI;

class Test : MonoBehaviour
{
    private Texture2D mTexture2D;
    private UGUIGridWrapContent mGridWrapContent;

    private void Start()
    {
        mGridWrapContent = this.GetComponent<UGUIGridWrapContent>();

        UGUIGridWrapContentConfig tConfig = new UGUIGridWrapContentConfig()
        {
            mDataCnt = this.mDataCount,
            mDisplayCellAction = DisplayCell,
            mCreateFunc = () => { return GameObject.Instantiate(transform.GetChild(0).gameObject, this.transform); }
        };

        mGridWrapContent.Show(tConfig);
    }

    private void DisplayCell(int pDataIndex, GameObject pGo)
    {
        Text tText = pGo.transform.GetComponent<Text>();
        if (tText == null)
            return;

        tText.text = pDataIndex.ToString();
    }

    public int mDataCount = 50;
    [ContextMenu("改变数量")]
    private void ChangeCount()
    {
        UGUIGridWrapContentConfig tConfig = mGridWrapContent.mConfig;

        tConfig.mDataCnt = mDataCount;
        mGridWrapContent.Show(tConfig);
    }
}