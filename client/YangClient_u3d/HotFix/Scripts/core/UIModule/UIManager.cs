using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hotfix
{
    public class UICreateConfig
    {
        public string mUIPrefabIndex;

        //请求者需要设置的变量
        public int mDelayCallFrame = 0;      //延迟调用回调函数， 帧为单位
        public Action<GameObject> mInstanceCallback;

        public UIManager.UILayer mUILayer = UIManager.UILayer.Layer2;    //层次

        public bool mAddGreyMask = false;    //添加灰色遮罩
        public bool mSpaceClose = false;     //界面空白区域点击自动关闭

    }


    public class UIManager
    {
        private class UICreateRequest
        {
            public UICreateConfig mConfig;

            public UnityEngine.Object mObject;
            public bool mUseCache;

            public enum State
            {
                wait,
                running,
                finist,
            }

            public State mState = State.wait;

            public void Run()
            {
                mState = State.running;
                ManagerIndex.AssetManager.LoadAssetAsync(mConfig.mUIPrefabIndex, (pObject, pUseCache) =>
                {
                    mObject = pObject;
                    mUseCache = pUseCache;

                    CreateUIGo();

                    mState = State.finist;

                });
            }

            private void CreateUIGo()
            {
                //创建实例
                //    延迟帧
                //        回调
            }
        }

        public enum UILayer
        {
            Layer1 = 0,       //用于 主界面 等一些较为基础的

            Layer2 = 500,     //用于 一般的UI界面，如：背包， 商店

            Layer3 = 1000,    //用于 弹框界面

            Layer4 = 1500,    //用于 消息通知界面
        }


        private Dictionary<UILayer, List<UICreateRequest>> mUICreateRequestDic = new Dictionary<UILayer, List<UICreateRequest>>();

        public void InitInCreateInstance()
        {
            AppLauncher.Instance.mUpdateAction += Update;
        }


        public UICreateConfig RequestCreateUI(string pUIPrefabIndex, UILayer pUILayer)
        {
            UICreateConfig tConfig = new UICreateConfig()
            {
                mUIPrefabIndex = pUIPrefabIndex,
            };

            UICreateRequest tRequest = new UICreateRequest()
            {
                mConfig = tConfig
            };

            if(mUICreateRequestDic.ContainsKey(pUILayer) == false)
                mUICreateRequestDic.Add(pUILayer, new List<UICreateRequest>());

            mUICreateRequestDic[pUILayer].Add(tRequest);

            return tConfig;
        }

        private void Update()
        {
            foreach (var tKv in mUICreateRequestDic)
            {
                if (tKv.Value.Count <= 0)
                    continue;

                foreach (var tUICreateRequest in tKv.Value)
                {

                    if (tUICreateRequest.mState == UICreateRequest.State.wait)
                    {
                        tUICreateRequest.Run();
                        break;
                    }

                    if (tUICreateRequest.mState == UICreateRequest.State.running)
                        break;

                    if (tUICreateRequest.mState == UICreateRequest.State.finist)
                    {
                        tKv.Value.Remove(tUICreateRequest);
                    }

                }
            }
        }
    }

}
