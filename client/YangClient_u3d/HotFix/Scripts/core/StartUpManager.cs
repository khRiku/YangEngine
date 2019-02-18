using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix
{
    public class StartUpManager
    {

        //初始化
        public void InitInInstance()
        {
            mCurParseIndex = 0;
            mStartActionList.Add(new KeyValuePair<StartParse, Func<IEnumerator>>(StartParse.AssetManager, ManagerIndex.AssetManager.CallInStartupManager));
        }

        public enum StartParse
        {
           AssetManager,
           ObjectPoolManager,
           AudioManager,
            
        }

        private List<KeyValuePair<StartParse, Func<IEnumerator>>> mStartActionList = new List<KeyValuePair<StartParse, Func<IEnumerator>>>();

        public int mCurParseIndex = 0;
        /// <summary>
        /// 运行逻辑
        /// </summary>
        public void RunLogic()
        {
            AppLauncher.Instance.StartCoroutine(ExcuteStartAction());
        }

        private IEnumerator ExcuteStartAction()
        {
            for (int i = 0; i < mStartActionList.Count; ++i)
            {
                yield return mStartActionList[i].Value();

            }

            //测试用代码
            AppLauncher.Instance.gameObject.AddComponent<HotFixTest>();

            //测试用代码
        }

    }

}

