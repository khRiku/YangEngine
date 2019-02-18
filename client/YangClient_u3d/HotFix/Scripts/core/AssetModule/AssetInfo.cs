using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Hotfix
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class AssetInfo
    {
        public string mIndex;
        public string mName;
        public string mPath;
    }

    /// <summary>
    /// 资源总信息
    /// </summary>
    public class AssetTotalInfo
    {
        //key: 1,2,3   value: 基于Asset下的位置信息, 1 a/ 2 b/ 3 c 这样
        private Dictionary<string, string> mAssetPathDic = new Dictionary<string, string>();
        private Dictionary<string, AssetInfo> mAssetInfoDic = new Dictionary<string, AssetInfo>();


        public static Thread CreateByThread(Action<AssetTotalInfo> pAssetTotalInfo)
        {
            string tFilePath = PathHelper.ResIndexDataFilePath;

            Thread tThread = new Thread(() =>
            {
                string tText = File.ReadAllText(tFilePath);

                AssetTotalInfo tAssetTotalInfo = new AssetTotalInfo();

                tAssetTotalInfo.mAssetPathDic = LitJson.JsonMapper.ToObject<Dictionary<string, string>>(tText);

                pAssetTotalInfo(tAssetTotalInfo);
            });

            tThread.Start();

            return tThread;
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="pAssetIndex"></param>
        /// <returns></returns>
        public AssetInfo GetAssetInfo(string pAssetIndex)
        {
            if (String.IsNullOrEmpty(pAssetIndex))
            {
                Debug.LogError("输入的资源索引字符串为空");
                return null;
            }

            if (mAssetInfoDic.ContainsKey(pAssetIndex))
                return mAssetInfoDic[pAssetIndex];

            if (mAssetPathDic.ContainsKey(pAssetIndex) == false)
            {
                Debug.LogError(string.Format("找不到资源 id = {0} 的资源 ", pAssetIndex));
                return null;
            }

            AssetInfo tAssetInfo = new AssetInfo();
            mAssetInfoDic[pAssetIndex] = tAssetInfo;

            tAssetInfo.mIndex = pAssetIndex;
            tAssetInfo.mPath = mAssetPathDic[pAssetIndex];

            int tSperatIndex = tAssetInfo.mPath.LastIndexOf('/');
            tAssetInfo.mName = tAssetInfo.mPath.Substring(tSperatIndex + 1);

            return tAssetInfo;

        }
    }
}
