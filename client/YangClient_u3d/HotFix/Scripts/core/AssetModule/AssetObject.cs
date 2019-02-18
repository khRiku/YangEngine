using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix
{
    public class AssetObject
    {
        public string mPath;      //Assets 为根节点的地址， 如  texture/1.png
        public string mName;      //包含根式如： 1.png

        public int mRefCount;     //引用计数
        public List<AssetObject> mDependObj = null;  //以来的资源对象：无依赖的 到 有依赖的

        public AssetBundle mAb;
        public UnityEngine.Object[] mObjectArr;

        private float mCacheObjectTime;       //缓存时间戳， 引用为0， 且超出这个时间戳的 资源 会被卸载


        //key: AssetObject.mPath, value: AssetObject
        private static Dictionary<string, AssetObject> mAssetObjDic = new Dictionary<string, AssetObject>();

        //待卸载资源的对象
        private static List<AssetObject> mWaitToUnloadAbInfo = new List<AssetObject>();

        private AssetObject()
        {

        }

        /// <summary>
        /// 获取资源对象
        /// </summary>
        public static AssetObject Spawn(string pPath)
        {
            if (mAssetObjDic.ContainsKey(pPath) == false)
            {
                AssetObject tAssetObject = new AssetObject();
                tAssetObject.mPath = pPath;

                if (AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.Editor
                    && pPath.StartsWith("Assets/Res") == false)
                {
                    tAssetObject.mPath = string.Format("Assets/Res/{0}", pPath);

                }

                int tLastIndex = tAssetObject.mPath.LastIndexOf("/");
                tAssetObject.mName = tAssetObject.mPath.Substring(tLastIndex + 1);

                tAssetObject.mRefCount = 0;
                tAssetObject.mDependObj = tAssetObject.GetSortDependAssetObject();

                mAssetObjDic.Add(pPath, tAssetObject);
            }

            return mAssetObjDic[pPath];
        }

        /// <summary>
        /// 获取依赖的AbInfo, 
        /// </summary>
        private List<AssetObject> GetSortDependAssetObject()
        {
            List<AssetObject> tDependAbInfoList = new List<AssetObject>();

            string[] tDependAssetPath = null;
            if (AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.AssetBundle)
            {
                tDependAssetPath = global::Hotfix.ManagerIndex.AssetManager.mAbManifest.GetDirectDependencies(mPath);
            }
            else
            {
                tDependAssetPath = HotFixAgent.AssetDataBase.GetDependencies(mPath);
            }

            foreach (var tDependAb in tDependAssetPath)
            {
                AssetObject tDepenAssetObject = Spawn(tDependAb);
                tDependAbInfoList.AddRange(tDepenAssetObject.mDependObj);
                tDependAbInfoList.Add(tDepenAssetObject);
            }

            return tDependAbInfoList;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public UnityEngine.Object GetAsset()
        {
            foreach (var tDependAbInfo in mDependObj)
            {
                tDependAbInfo.GetAsset();
            }

            if (IsAssetWaitToUnload())
                mWaitToUnloadAbInfo.Remove(this);



            if (AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.AssetBundle)
            {
                if (mAb == null)
                {
                    if (mRefCount != 0)
                    {
                        Debug.LogError(string.Format("错误：首次加载的资源对象的引用计数不为 0, 引用计数：{0} ，资源路径 ： {1}",
                            mRefCount, mPath));
                    }

                    string tPath = PathHelper.GetAssetAbsolutePath(mPath);
                    mAb = AssetBundle.LoadFromFile(tPath);
                    mObjectArr = mAb.LoadAllAssets();
                }

                ++mRefCount;

                return mAb.LoadAsset<UnityEngine.Object>(mName);
            }
            else
            {
                if (mObjectArr == null)
                {
                    if (mRefCount != 0)
                    {
                        Debug.LogError(string.Format("错误：首次加载的资源对象的引用计数不为 0, 引用计数：{0} ，资源路径 ： {1}",
                            mRefCount, mPath));
                    }

                    UnityEngine.Object tAsset = HotFixAgent.AssetDataBase.LoadAssetAtPath(mPath);

                    mObjectArr = new UnityEngine.Object[]
                    {
                        tAsset,
                    };
                }

                ++mRefCount;

                return mObjectArr[0];
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        public IEnumerator GetAssetAsync(Action<UnityEngine.Object, bool> pCallback)
        {
            foreach (var tDependAbInfo in mDependObj)
            {
                tDependAbInfo.GetAsset();
            }

            if (IsAssetWaitToUnload())
                mWaitToUnloadAbInfo.Remove(this);

            if (AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.AssetBundle)
            {
                bool tUseCache = mAb != null;
                if (tUseCache == false)
                {
                    if (mRefCount != 0)
                    {
                        Debug.LogError(string.Format("错误：首次加载的资源对象的引用计数不为 0, 引用计数：{0} ，资源路径 ： {1}",
                            mRefCount, mPath));
                    }

                    string tPath = PathHelper.GetAssetAbsolutePath(mPath);

                    AssetBundleCreateRequest tRequest = AssetBundle.LoadFromFileAsync(tPath);
                    yield return tRequest;

                    mAb = tRequest.assetBundle;
                    mObjectArr = mAb.LoadAllAssets();
                }

                ++mRefCount;
                UnityEngine.Object tObject = mAb.LoadAsset<UnityEngine.Object>(mName);

                pCallback(tObject, tUseCache);
            }
            else
            {
                if (mObjectArr == null)
                {
                    if (mRefCount != 0)
                    {
                        Debug.LogError(string.Format("错误：首次加载的资源对象的引用计数不为 0, 引用计数：{0} ，资源路径 ： {1}",
                            mRefCount, mPath));
                    }

                    UnityEngine.Object tAsset = HotFixAgent.AssetDataBase.LoadAssetAtPath(mPath);

                    mObjectArr = new UnityEngine.Object[]
                    {
                        tAsset,
                    };
                }

                ++mRefCount;
                pCallback(mObjectArr[0], false);
            }
        }

        /// <summary>
        /// 释放AssetBundle 
        /// </summary>
        /// <returns></returns>
        public int ReleaseAsset()
        {
            //释放依赖
            foreach (var tDependAbInfo in mDependObj)
            {
                tDependAbInfo.ReleaseAsset();
            }

            --mRefCount;

            if (mRefCount < 0)
                Debug.LogError(string.Format("错误：卸载引用计数为 0 的AssetBundle, 资源路径 ： {0}", mPath));

            if (mRefCount == 0)
            {
                //TODO:Ab 暂定缓存40 秒， 后期可用配置控制， 实现更精细管理
                mCacheObjectTime = Time.unscaledTime + 3;
                mWaitToUnloadAbInfo.Add(this);
            }

            return mRefCount;
        }


        /// <summary>
        /// assetbundle 是否等待被卸载
        /// </summary>
        private bool IsAssetWaitToUnload()
        {
            return mObjectArr != null && mRefCount == 0;
        }

        /// <summary>
        /// 根据缓存时间， 删除0引用的 AssetBundle
        /// </summary>
        public static IEnumerator CheckToUnloadAb()
        {
            while (true)
            {
                List<AssetObject> tAssetInfoList = new List<AssetObject>();

                foreach (var tAssetObject in mWaitToUnloadAbInfo)
                {
                    if (tAssetObject.mCacheObjectTime >= Time.unscaledTime)
                        continue;

                    tAssetInfoList.Add(tAssetObject);
                    if (AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.AssetBundle)
                    {
                        tAssetObject.mAb.Unload(true);
                    }

                    tAssetObject.mAb = null;
                    tAssetObject.mObjectArr = null;

                }

                mWaitToUnloadAbInfo.RemoveAll((pAssetObject) => { return tAssetInfoList.Contains(pAssetObject); });

                yield return new WaitForSeconds(1);
            }


        }
    }

}
