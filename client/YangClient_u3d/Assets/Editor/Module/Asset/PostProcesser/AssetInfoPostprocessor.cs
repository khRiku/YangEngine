using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;

public class AssetInfoPostprocessor : AssetPostprocessor
{
    public static string mResPath = "Assets/Res";

    static void OnPostprocessAllAssets(string[] pImportedAssets, string[] pDeletedAssets, string[] pMovedAssets,
        string[] pMovedFromAssetPaths)
    {
        List<String> tChangeAssets = new List<string>();
        tChangeAssets.AddRange(pImportedAssets);
        tChangeAssets.AddRange(pDeletedAssets);
        tChangeAssets.AddRange(pMovedAssets);

        bool tAssetIndexChange = false;
        foreach (var tAssetPath in tChangeAssets)
        {
            if (tAssetPath.StartsWith(mResPath) == false)
                continue; 

            if (CheckPathName(tAssetPath) && IsFilePath(tAssetPath))
            {
                tAssetIndexChange = true;
                break;
            }
        }

        if (tAssetIndexChange)
            RefreshAssetIndexFromRes();
    }

    /// <summary>
    /// 从res文件夹中， 获取资源信息， 进行刷新
    /// </summary>
    public static void RefreshAssetIndexFromRes()
    {
        Dictionary<string, string> mAssetIndexData = GetResIndexData();
        string tJsonData = LitJson.JsonMapper.ToJson(mAssetIndexData);

        File.WriteAllText(PathHelper.ResIndexDataFilePath, tJsonData);
    }

    public static Dictionary<string, string> GetResIndexData()
    {
        Dictionary<string, string> mAssetIndexData = new Dictionary<string, string>();
        string tResPath = string.Format("{0}/Res", Application.dataPath);
        List<string> mAssetFilePathList = GetAssetFileFromDirectory(tResPath);

        foreach (var tIndexAssetPath in mAssetFilePathList)
        {
            string tRelativePath = tIndexAssetPath.Replace(tResPath + "/", "");
            string tIndex = GetAssetIndex(tRelativePath);

            if (mAssetIndexData.ContainsKey(tIndex))
            {
                Debug.LogError("索引重复， index = ：" + tIndex);
                continue;                
            }
            mAssetIndexData.Add(tIndex, tRelativePath);
        }

        return mAssetIndexData;
    }

    /// <summary>
    /// 获取索引资源的路径
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAssetFileFromDirectory(string pDiretoryPath)
    {
        List<string> mAssetPathList = new List<string>();

        DirectoryInfo tDf = new DirectoryInfo(pDiretoryPath);


        foreach (var tFileInfo in tDf.GetFiles())
        {
            if(tFileInfo.Extension == ".meta")
                continue;            
        
            if (IsIndexNameFormat(tFileInfo.Name))
            {
                string tFilePath = tFileInfo.FullName.Replace("\\", "/");
                mAssetPathList.Add(tFilePath);
            }
        }
        
        foreach(var tDirectoryInfo in tDf.GetDirectories())
        {
            if (IsIndexNameFormat(tDirectoryInfo.Name) == false)
                continue;
            ;
            mAssetPathList.AddRange(GetAssetFileFromDirectory(tDirectoryInfo.FullName));
        }

        return mAssetPathList;
    }

    public static string GetAssetIndex(string tRelativePath)
    {
        StringBuilder tSb = new StringBuilder();
        string[] tIndexNameArr = tRelativePath.Split('/');
        for(int i = 0; i < tIndexNameArr.Length; ++i)
        {
            string[] tNameSperate = tIndexNameArr[i].Split(' ');

            if (i == 0)
                tSb.Append(tNameSperate[0]);
            else
                tSb.Append(string.Format(",{0}", tNameSperate[0]));
        }

        return tSb.ToString();
    }

    /// <summary>
    /// 检查路径的命名是否满足 索引格式
    /// </summary>
    public static bool CheckPathName(string pResRelativePath)
    {
        if (pResRelativePath.StartsWith(mResPath) == false || pResRelativePath.Length == mResPath.Length)
            return false;

        //检查资源路径是否合法
        string tIdPath = pResRelativePath.Substring(mResPath.Length + 1);
        string[] tPathSperateArr = tIdPath.Split('/');

        if(tPathSperateArr == null || tPathSperateArr.Length <= 0)
            return false;

        //检查从res目录开始， 路径名中的每一段是否符合索引格式
        foreach (var tPathSperate in tPathSperateArr)
        {
            if (IsIndexNameFormat(tPathSperate) == false)
            {
                return false;
            }
        }

        return true;
    }


    //是否符合索引形式: 数字 + 空格 + 名字
    public static bool IsIndexNameFormat(string pStr)
    {
        string[] tSignArr = pStr.Split(' ');

        if (tSignArr.Length <= 0)
        {
            return false;
        }

        int tId = 0;
        if (int.TryParse(tSignArr[0], out tId) == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 是文件的路径， 而不是文件夹的路径
    /// </summary>
    /// <param name="pRelativePath"></param>
    /// <returns></returns>
    public static bool IsFilePath(string pRelativePath)
    {
        string tPath = string.Format("{0}/../{1}", Application.dataPath, pRelativePath);

        //删除的情况下面代码失效， 因为没有了具体的文件， 所以只能判断名字了
        //return File.Exists(tPath);

        FileInfo tFileInfo = new FileInfo(tPath);
        bool tHasExtension = string.IsNullOrEmpty(tFileInfo.Extension) == false;
        return tHasExtension;

    }


}
