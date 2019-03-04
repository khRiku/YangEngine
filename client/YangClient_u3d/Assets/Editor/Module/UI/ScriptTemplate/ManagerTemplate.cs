using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ManagerTemplate 
{
    #region 基本函数

    /// 创建类实例后， 首个调用的函数， 可添加必要参数
    public bool SetUp()
    {         
        //如有参数， 先保存参数

        InitData();
        RegisterEvent();
        Star();
        return true;
    }

    /// 初始化需要用到的数据
    public bool InitData()
    {

        return true;
    }

    /// 注册事件
    private void RegisterEvent()
    {
        
    }

    /// 移除事件
    private void UnRegisterEvent()
    {

    }

    /// 对象删除或缓存的时候需要调用， 处理一些清理操作
    public void Dispose()
    {
        UnRegisterEvent();
    }

    /// 相关的处理逻辑从这个函数开始
    private void Star()
    {

    }

    #endregion
}
