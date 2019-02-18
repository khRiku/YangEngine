using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotFixAgent
{
    public class AssetDataBase
    {
        public static UnityEngine.Object LoadAssetAtPath(string pAssetPath)
        {
#if !UNITY_EDITOR
        return null;
#else
        return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pAssetPath);
#endif
        }

        public static string[] GetDependencies(string pAssetPath)
        {
#if !UNITY_EDITOR
        return null;
#else
            List<string> mDependList = new List<string>();
            mDependList.AddRange(UnityEditor.AssetDatabase.GetDependencies(pAssetPath));

            mDependList.RemoveAll((pDepend) =>
            {
                return pDepend.StartsWith("Assets/Res") == false
                       || pDepend == pAssetPath; 

            });

            return mDependList.ToArray();
#endif

        }
    }

}


