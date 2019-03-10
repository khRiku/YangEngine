using UnityEngine;

public class UIElementData
{
    public GameObject mRootGo;       //ui prefab 实例

    public string mVarName;          //变量名
    public string mPath;             //相对路径
    public string mTypeName;         //要绑定的类型名

    public GameObject mGo;           //对应的GameObject
    public object mTypeInstance;     //要绑定的类型实例



    public static UIElementData Create(GameObject pRootGo, string pName, string pPath, string pTypeName)
    {
        if (pRootGo == null)
            return null;

        UIElementData tData = new UIElementData();

        tData.mRootGo = pRootGo;

        tData.mVarName = pName;
        tData.mPath = pPath;
        tData.mTypeName = pTypeName;

        tData.mGo = tData.GetGo();
        tData.mTypeInstance = tData.GetTypeInstance();

        return tData;
    }

    public GameObject GetGo()
    {
        if (mGo != null)
            return mGo;

        Transform tGoTransform = mRootGo.transform.Find(mPath);

        if (tGoTransform == null)
            return null;

        return tGoTransform.gameObject;
    }

    public object GetTypeInstance()
    {
        GameObject tGo = GetGo();
        if (tGo == null)
            return null;

        if (mTypeName == UIScriptCreatorManager.mGoTypeName)
            return tGo;

        return tGo.GetComponent(mTypeName);
    }

    public enum eState
    {
        NotError,           //没有错误

        Go_TypeNull,         //Go 还在， 但组件删除了
        Go_Type_PathError,   //Go, 组件 都在， 但通过路径查找不到(节点改名，改父节点）

        GoNull,         //对象不在了， 用户删除了对象
    }

    public eState mState = eState.NotError;

    public void RefreshState()
    {
        mState = GetState();
    }

    /// <summary>
    /// 获取当前的状态
    /// </summary>
    /// <returns></returns>
    public eState GetState()
    {
        bool tGoNull = IsGoNull();
        bool tGo_TypeNull = IsGo_TypeNull();
        bool tGo_Type_PathError = IsGo_Type_PathError();

        eState tState = eState.NotError;

        if (tGoNull)
        {
            tState = eState.GoNull;
        }
        else if (tGo_TypeNull)
        {
            tState = eState.Go_TypeNull;
        }
        else if (tGo_Type_PathError)
        {
            tState = eState.Go_Type_PathError;
        }

        return tState;
    }

    /// <summary>
    /// 对象为空， 用户删除了改Go
    /// </summary>
    /// <returns></returns>
    public bool IsGoNull()
    {
        GameObject tGo = GetGo();

        return tGo == null;
    }

    /// <summary>
    /// Go 还在， 但 组件 被删除了
    /// </summary>
    /// <returns></returns>
    public bool IsGo_TypeNull()
    {
        GameObject tGo = GetGo();
        object tComponent = GetTypeInstance();

        if (tGo != null && tComponent == null)
            return true;

        return false;
    }

    /// <summary>
    /// Go, 组件 都在， 但通过路径查找 不到
    /// </summary>
    /// <returns></returns>
    public bool IsGo_Type_PathError()
    {
        GameObject tGo = GetGo();
        object tComponent = GetTypeInstance();

        if (tGo != null
            && tComponent != null
            && tGo.GetParenRelativePath(mRootGo, false) != mPath)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 根据 GameObject 来更新数据
    /// </summary>
    public void UpdateDataByGo()
    {
        GameObject tGo = GetGo();
        if (tGo == null)
            return;

        mGo = tGo;
        mTypeInstance = GetTypeInstance();

        mVarName = UIScriptCreatorManager.GetVariableName(tGo, mTypeName);
        mPath = tGo.GetParenRelativePath(mRootGo, false);

        RefreshState();
    }

    public void UpdateGo()
    {
        mGo = GetGo();
    }
}