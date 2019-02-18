using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIScriptCreatorManager
{
    private static UIScriptCreatorManager mInstance;

    public static UIScriptCreatorManager Instance
    {
        get
        {
            if(mInstance == null)
                mInstance = new UIScriptCreatorManager();

            return mInstance;
        }
    }

    public bool mCreateViewScript = true;

    //UI Prefab
    public Object mUiPrefab {  get; private set; }

    /// <summary>
    /// 设置Prefab
    /// </summary>
    public bool SetUIPrefab(Object pGo)
    {
        if (pGo == null)
            return false;

        if (PrefabUtility.GetPrefabAssetType(pGo) == PrefabAssetType.NotAPrefab)
        {
            mUiPrefab = null;
            return false;
        }

        mUiPrefab = pGo;
        return true;
    }
}
