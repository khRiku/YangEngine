using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Hotfix
{
    public class AssetManager
    {
        public AssetBundleManifest mAbManifest;

        public AssetTotalInfo mAssetTotalInfo;

        private Dictionary<string, AssetObject> mAbInfoDic = new Dictionary<string, AssetObject>();


        //实例化时，进行初始化
        public void InitInCreateInstance()
        {
           
        }


        #region 模块专属接口
        /// <summary>
        /// StartupManager 的专属函数
        /// </summary>
        public IEnumerator CallInStartupManager()
        {
            yield return LoadAssetBunbleManifest();
            yield return LoadResIndexData();

            AppLauncher.Instance.StartCoroutine(AssetObject.CheckToUnloadAb());
        }

        #endregion

        /// <summary>
        /// 加载 assetbunde Manifest文件
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadAssetBunbleManifest()
        {
            if (AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.Editor)
                yield break;

            string pPath = PathHelper.AbManifestFilePath;

            AssetBundleCreateRequest tRequest = AssetBundle.LoadFromFileAsync(pPath);

            yield return tRequest;

            mAbManifest = tRequest.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");         
        }

        /// <summary>
        /// 加载资源映射表
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadResIndexData()
        {

            Thread tThread = AssetTotalInfo.CreateByThread((pData) => { mAssetTotalInfo = pData; });

            while(tThread.ThreadState != ThreadState.Stopped)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        public UnityEngine.Object LoadAsset(string pAssetIndex)
        {
            if (string.IsNullOrEmpty(pAssetIndex))
                return null;

            AssetInfo tAssetInfo = mAssetTotalInfo.GetAssetInfo(pAssetIndex);

            AssetObject tAssetObject = AssetObject.Spawn(tAssetInfo.mPath);
            return tAssetObject.GetAsset();
        }

        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        public void LoadAssetAsync(string pAssetIndex, Action<UnityEngine.Object, bool> pCallback)
        {
            AssetInfo tAssetInfo = mAssetTotalInfo.GetAssetInfo(pAssetIndex);
            AssetObject tAssetObject = AssetObject.Spawn(tAssetInfo.mPath);

            AppLauncher.Instance.StartCoroutine(tAssetObject.GetAssetAsync(pCallback));
        }

        /// <summary>
        /// 卸载资源资源
        /// </summary>
        public void UnloadAsset(string pAssetIndex)
        {
            AssetInfo tAssetInfo = mAssetTotalInfo.GetAssetInfo(pAssetIndex);
            AssetObject tAssetObject = AssetObject.Spawn(tAssetInfo.mPath);
            tAssetObject.ReleaseAsset();
        }


    }
}

