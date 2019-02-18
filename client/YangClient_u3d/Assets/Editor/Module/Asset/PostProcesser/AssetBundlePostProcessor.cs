using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;

public class AssetBundlePostProcessor :AssetPostprocessor
{  
    static void OnPostprocessAllAssets(string[] pImportedAssets, string[] pDeletedAssets, string[] pMovedAssets,
        string[] pMovedFromAssetPaths)
    {
        for (int i = 0; i < pImportedAssets.Length; ++i)
        {
            string tRelativePath = pImportedAssets[i];

            SetAbName(tRelativePath);

        }
    }


    /// <summary>
    /// 设置资源的ab名，
    ///  规则：
    ///  id资源取文件路劲为ab名
    ///  在res 路径下的非id资源， 已目录路径为ab名
    /// </summary>
    private static void SetAbName(string pAssetRelativePath)
    {
        string tResPath = AssetInfoPostprocessor.mResPath;
        bool tIsIdAssetPath = AssetInfoPostprocessor.CheckPathName(pAssetRelativePath);
        bool tIsInResPath = pAssetRelativePath.StartsWith(tResPath);

        if (tIsInResPath == false)
            return;

        DirectoryInfo tResDirectoryInfo = new DirectoryInfo(tResPath);
        FileInfo tFileInfo = new FileInfo(pAssetRelativePath);
      
        if (Directory.Exists(tFileInfo.FullName) == true)
            return;

        string tAbName = tFileInfo.FullName.Replace(tResDirectoryInfo.FullName + "\\", "");
        if (tIsIdAssetPath == false)
        {
            tAbName = tAbName.Replace("\\" + tFileInfo.Name, "");
        }

        string tLowAbName = tAbName.Replace("\\", "/").ToLower();
        AssetImporter tImporter = AssetImporter.GetAtPath(pAssetRelativePath);
        if (tImporter.assetBundleName != tLowAbName)
        {
            tImporter.assetBundleName = tLowAbName;
            tImporter.SaveAndReimport();           
        }

        string[] mAbAssetPathList = HotFixAgent.AssetDataBase.GetDependencies(pAssetRelativePath);

        for (int i = 0; i < mAbAssetPathList.Length; ++i)
        {
            string tAssetRelativePath = mAbAssetPathList[i];
            if (tAssetRelativePath.StartsWith(tResPath) == false)
            {
                Debug.LogError(string.Format("依赖的资源， 不在{0}目录下", tResPath));
                continue;
            }          

            SetAbName(tAssetRelativePath);
        }
    }

    /// <summary>
    /// 刷新资源的ab名
    /// </summary>
    public static void RefreshAbName()
    {
      string[] tAssetPathArr =  AssetDatabase.GetAllAssetPaths();
        foreach (var tAssetPath in tAssetPathArr)
        {
            SetAbName(tAssetPath);
        }
    }

}
