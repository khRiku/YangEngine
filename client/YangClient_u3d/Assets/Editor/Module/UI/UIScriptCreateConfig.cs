using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System;
using System.IO;

public class UIScriptCreateConfig
{
    public string mPath; //路径
    public bool mCreate; //是否创建文件
    public bool mCover; //文件存在的情况下是否覆盖

    public enum ScriptType
    {
        View,
        Controller,
        Model,
        Manager,
        Proxy,
    }

    public ScriptType mScriptType;

    private Func<string> mGetDefaultPathFunc;
    //获取默认的路径
    public Func<string> DefaultGetPathFunc
    {
        set
        {
            mGetDefaultPathFunc = value;
            ResetPath();
        }
    }

    //脚本名
    public string mScriptName
    {
        get { return mScriptType.ToString(); }
    }

    /// <summary>
    /// 是否需要设置 mCover = true
    /// </summary>
    public bool IsNotAllowCover()
    {
        if (mCreate && File.Exists(mGetDefaultPathFunc()) && mCover == false)
            return true;

        return false;
    }

    /// <summary>
    /// 恢复默认路径
    /// </summary>
    public void ResetPath()
    {
        mPath = mGetDefaultPathFunc();
    }
}