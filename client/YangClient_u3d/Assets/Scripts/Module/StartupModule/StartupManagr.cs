using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class StartupManagr
{
    public enum StartParse
    {
        Hotfix,   //热更
    }

    private List<KeyValuePair<StartParse, Action<Action>>> mStartActionList = new List<KeyValuePair<StartParse, Action<Action>>>();

    public int mCurParseIndex = 0;



    //创建实例时，进行初始化
    public void InitInInstance()
    {
        mCurParseIndex = 0;
        mStartActionList.Add(new KeyValuePair<StartParse, Action<Action>>(StartParse.Hotfix, ManagerIndex.HotFixManager.CallInStartupManager));
    }


    /// <summary>
    /// 运行逻辑
    /// </summary>
    public void RunLogic()
    {
        ExcuteCurrentParse();

    }



    private void ListenToNextParse()
    {
        ++mCurParseIndex;
        if (mCurParseIndex >= mStartActionList.Count)
        {
            return;
        }
        ExcuteCurrentParse();
    }

    private void ExcuteCurrentParse()
    {
        mStartActionList[mCurParseIndex].Value(ListenToNextParse);

    }

}
