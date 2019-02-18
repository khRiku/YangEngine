using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathHelper
{
    public static string AbManifestFilePath
    {
        get
        {
#if UNITY_EDITOR
            return string.Format("{0}/../AssetBundle/Windows/Res/Res", Application.dataPath);
#else
            return string.Format("{0}/../Res/Res", Application.persistentDataPath);
#endif
        }

    }

    public static string ResPath
    {
        get
        {
#if UNITY_EDITOR
            if(AppConfig.mAssetLoadMode == AppConfig.AssetLoadMode.AssetBundle)
                return string.Format("{0}/../AssetBundle/Windows/Res", Application.dataPath);


            //AppConfig.AssetLoadMode.Editor 的情况    
            return string.Format("{0}/Res", Application.dataPath);

#else
            return string.Format("{0}/Res", Application.persistentDataPath);
#endif
        }
    }
    /// <summary>
    /// 获取资源完整位置
    /// </summary>
    public static string GetAssetAbsolutePath(string pRelativePath)
    {
        return string.Format("{0}/{1}", ResPath, pRelativePath);
    }


    public string DataPath
    {
        get
        {
#if UNITY_EDITOR
            return string.Format("{0}/Data", Application.dataPath);
#else
            return string.Format("{0}/Data", Application.persistentDataPath);
#endif
        }
    }

    /// <summary>
    /// 资源索引数据文件的路径
    /// </summary>
    public static string ResIndexDataFilePath
    {
        get
        {
#if UNITY_EDITOR
            return string.Format("{0}/Data/Config/ResIndexData.txt", Application.dataPath);
#else
            return string.Format("{0}/Data/Config/ResIndexData.txt", Application.persistentDataPath);
#endif

        }
    }

}
