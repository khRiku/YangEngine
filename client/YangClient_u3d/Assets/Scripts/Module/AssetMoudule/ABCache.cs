using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// author: yxx
/// date: 2019-8-5
/// Descritption: 对AB和AB里的Asset 进行缓存和读取
/// 
/// </summary>
public class ABCache
{
    private string mAbPath;
    private Dictionary<string, Object> mAssetDic;

    private int mRefCount;

    private void LoadAb()
    {

    }

    public Object LoadAsset()
    {
        return null;
    }

    public void LoadAbAsync()
    {

    }

    public void LoadAssetAsync(Action<Object> pCallback)
    {

    }

    private void UnLoadAb()
    {

    }

    private void AddRefCount()
    {

    }

    private void DecreaseRefCount()
    {

    }
}

