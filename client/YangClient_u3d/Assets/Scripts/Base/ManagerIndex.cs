using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class ManagerIndex 
{
    /// <summary>
    /// 启动管理器
    /// </summary>
    private static StartupManagr mStartupManagr = null;
    public static StartupManagr StartupManagr
    {
        get
        {
            if (mStartupManagr == null)
            {
                mStartupManagr = new StartupManagr();
                mStartupManagr.InitInInstance();
            }

            return mStartupManagr;
        }
    }


    /// <summary>
    /// Hotfix管理器
    /// </summary>
    private static HotFixManager mHotFixManager = null;
    public static HotFixManager HotFixManager
    {
        get
        {
            if (mHotFixManager == null)
            {
                mHotFixManager = new HotFixManager();                 
            }

            return mHotFixManager;
        }  
    }
}
