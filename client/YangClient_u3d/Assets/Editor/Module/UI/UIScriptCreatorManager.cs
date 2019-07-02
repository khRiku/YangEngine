using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIScriptCreatorManager
{
    public Dictionary<string, UIElementData>
        mUIElementDic = new Dictionary<string, UIElementData>(); //key: 变量名， 存放绑定的元素

    List<string> mWaitToDelectList = new List<string>(); //存放待删除的变量名

    public List<UIScriptCreateConfig> mScriptCreateConfigList = new List<UIScriptCreateConfig>();

    //UI Prefab
    public GameObject mPrefabGo { get; private set; }
    public static string mGoTypeName = "GameObject";


    #region 路径信息

    /*  路径位置示例   *****
     
    ui prefab 存放位置：     E:\github\YangEngine\client\YangClient_u3d\Assets\Res\40 ui
    ui 脚本 存放位置：       E:\github\YangEngine\client\YangClient_u3d\HotFix\Scripts\module\UILogicModule
                 
    *******/

    //模块名， 就是相应 prefab 存放的一级目录, 如：  E:\github\YangEngine\client\YangClient_u3d\Assets\Res\40 ui\1 XXX, 模块名就是 XXX 这个文件夹名
    public string mModuleName;

    //Prefab 相对于 Assets 的位置， 例如这样： Assets/GameObject.prefab
    public string mPrefabRelativePath;


    public string
        mViewPath = ""; //view 代码脚本的位置， 如：E:\github\YangEngine\client\YangClient_u3d\HotFix\Scripts\module\UILogicModule\xxx\View\ prefab名称+View.cs

    public string mControllerPath = ""; //跟View 类似， 但相关字符是 Controller
    public string mModelPath = ""; //跟View 类似， 但相关字符是 Model
    public string mProxyPath = ""; //跟View 类似， 但相关字符是 Proxy
    public string mManagerPath = ""; //跟View 类似， 但相关字符是 Manager

    //模板代码的位置
    public string mViewTemplatyPath = "";
    public string mControllerTemplatyPath = "";
    public string mModelTemplatyPath = "";
    public string mProxyTemplatyPath = "";
    public string mManagerTemplatyPath = "";

    #endregion

    //脚本创建的信息

    public void Init()
    {
        //模板路径
        mViewTemplatyPath = Application.dataPath + @"\Editor\Module\UI\ScriptTemplate\ViewTemplate.cs";
        mControllerTemplatyPath = Application.dataPath + @"\Editor\Module\UI\ScriptTemplate\ControllerTemplate.cs";
        mModelTemplatyPath = Application.dataPath + @"\Editor\Module\UI\ScriptTemplate\ModelTemplate.cs";
        mProxyTemplatyPath = Application.dataPath + @"\Editor\Module\UI\ScriptTemplate\ProxyTemplate.cs";
        mManagerTemplatyPath = Application.dataPath + @"\Editor\Module\UI\ScriptTemplate\ManagerTemplate.cs";
    }

    /// <summary>
    /// 设置Prefab
    /// </summary>
    public bool SetUIPrefab(GameObject pGo)
    {
        if (pGo == null)
            return false;

        if (PrefabUtility.GetPrefabAssetType(pGo) == PrefabAssetType.NotAPrefab)
        {
            mPrefabGo = null;
            return false;
        }

        mPrefabGo = pGo;

        ClearData();
        UpdatePath();

        //刷新配置路径
        foreach (var tScriptCreateConfig in mScriptCreateConfigList)
            tScriptCreateConfig.ResetPath();

        ParseViewScriptData();

        return true;
    }

    private void ClearData()
    {
        mUIElementDic.Clear();
        mWaitToDelectList.Clear();
    }

    /// <summary>
    /// 更新路径信息
    /// </summary>
    private void UpdatePath()
    {
        if (mPrefabGo == null)
        {
            mPrefabRelativePath = "";

            mModuleName = "";

            mViewPath = "";
            mControllerPath = "";
            mModelPath = "";
            mProxyPath = "";
            mManagerPath = "";
        }

        //Preafab 相对于Assets 的位置 如 Assets/Res/40 ui/1 MainUI/2 SubView/3 SubUIView.prefab
        // mPrefabRelativePath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(mPrefabGo);这个直接可以获取路径， 但感觉下面这种比较清晰
        Object tObject = PrefabUtility.GetCorrespondingObjectFromSource(mPrefabGo);
        mPrefabRelativePath = AssetDatabase.GetAssetPath(tObject);

        //去掉  Assets/Res/40 ui/ 部分, 如 1 MainUI/2 SubView/3 SubUIView.prefab
        string tStr = mPrefabRelativePath.Replace(@"Assets/Res/40 ui/", "");

        //去掉文件名 如： 1 MainUI/2 SubView
        tStr = tStr.Substring(0, tStr.LastIndexOf('/'));

        //去掉 数字和空格的部分 如： MainUI/SubView
        tStr = Regex.Replace(tStr, @"[\d]+[ ]+", "");

        //一级目录名添加"Module"后缀
        int tIndex = tStr.IndexOf('/');
        tStr = tIndex >= 0 ? tStr.Insert(tIndex, "Module") : string.Format("{0}Module", tStr);

        string tScripPrePath = string.Format(@"{0}\{1}",
            Application.dataPath + @"/../HotFix\Scripts\module\UILogicModule", tStr);

        string tName = Regex.Replace(mPrefabGo.name, @"\d+ ", "");
        tName = tName.Replace(" ", "");

        mViewPath = string.Format(@"{0}\View\{1}View.cs", tScripPrePath, tName);
        mControllerPath = string.Format(@"{0}\Controller\{1}Controller.cs", tScripPrePath, tName);
        mModelPath = string.Format(@"{0}\Model\{1}Model.cs", tScripPrePath, tName);
        mProxyPath = string.Format(@"{0}\Proxy\Proxy{1}.cs", tScripPrePath, tName);
        mManagerPath = string.Format(@"{0}\Manager\{1}Manager.cs", tScripPrePath, tName);

        //脚本创建设置
        mScriptCreateConfigList.Clear();
        mScriptCreateConfigList.Add(new UIScriptCreateConfig()
        {
            mScriptType = UIScriptCreateConfig.ScriptType.View,
            mCover = true,
            mCreate = true,
            DefaultGetPathFunc = () => { return mViewPath; }
        });
        mScriptCreateConfigList.Add(new UIScriptCreateConfig()
        {
            mScriptType = UIScriptCreateConfig.ScriptType.Controller,
            mCover = false,
            mCreate = false,
            DefaultGetPathFunc = () => { return mControllerPath; }
        });
        mScriptCreateConfigList.Add(new UIScriptCreateConfig()
        {
            mScriptType = UIScriptCreateConfig.ScriptType.Model,
            mCover = false,
            mCreate = false,
            DefaultGetPathFunc = () => { return mModelPath; }
        });
        mScriptCreateConfigList.Add(new UIScriptCreateConfig()
        {
            mScriptType = UIScriptCreateConfig.ScriptType.Proxy,
            mCover = false,
            mCreate = false,
            DefaultGetPathFunc = () => { return mProxyPath; }
        });
        mScriptCreateConfigList.Add(new UIScriptCreateConfig()
        {
            mScriptType = UIScriptCreateConfig.ScriptType.Manager,
            mCover = false,
            mCreate = false,
            DefaultGetPathFunc = () => { return mManagerPath; }
        });

    }

    /// <summary>
    /// 解析View脚本中的元素 
    /// </summary>
    private void ParseViewScriptData()
    {

        if (File.Exists(mViewPath) == false)
            return;

        string tViewContent = File.ReadAllText(mViewPath);

        string tStartTag = @"{//CheckNullElementStartTag";
        string tEndTag = @"};//CheckNullElementEndTag";

        int tStartIndex = tViewContent.IndexOf(tStartTag) + tStartTag.Length;
        int tCount = tViewContent.IndexOf(tEndTag) - tStartIndex;

        string tContent = tViewContent.Substring(tStartIndex, tCount);
        string[] tLineSplitStrArr = tContent.Split('\n');

        for (int i = 0; i < tLineSplitStrArr.Length; ++i)
        {
            string tLineStr = tLineSplitStrArr[i];
            if (tLineStr.Contains(',') == false)
            {
                continue;
            }

            string[] tDotSplitStrArr = tLineStr.Split(',');

            string tPattern = "tElementDic.Add\\(new KeyValuePair<string, string>\\(\"([\\s\\S]+)\",\"([\\s\\S]+)\"\\)\\);";

            Match tMatch = Regex.Match(tLineStr, tPattern);
            string tPathStr = tMatch.Groups[1].Value;
            string tTypeName = tMatch.Groups[2].Value;



            int tGoNameStartIndex = tPathStr.LastIndexOf("/") + 1;
            string tGoName = tPathStr.Substring(tGoNameStartIndex);
            if (string.IsNullOrEmpty(tGoName))
                tGoName = mPrefabGo.name;

            string tVarName = GetVariableNameByStr(tGoName, tTypeName);
            UIElementData tElement = UIElementData.Create(mPrefabGo, tVarName, tPathStr, tTypeName);
            tElement.RefreshState();

            mUIElementDic.Add(tElement.mVarName, tElement);
        }
    }

    /// <summary>
    /// 获取可选择的类型名
    /// </summary>
    /// <returns></returns>
    public List<string> GetGoTypeNameList(GameObject pGo)
    {
        List<string> tTypeNameList = new List<string>();

        if (pGo == null || mPrefabGo == null)
            return tTypeNameList;

        if (mPrefabGo == null || pGo == null)
            return tTypeNameList;

        Transform tParent = mPrefabGo.transform;
        Transform tChild = pGo.transform;

        //如果 mChild == mParent 同一个Transform也是返回 true
        if (tChild.IsChildOf(tParent) == false)
            return tTypeNameList;

        tTypeNameList.Add(mGoTypeName);

        Component[] tComponentArray = tChild.GetComponents<Component>();
        if (tComponentArray == null || tComponentArray.Length < 0)
            return tTypeNameList;

        foreach (var tComponent in tComponentArray)
        {
            tTypeNameList.Add(tComponent.GetType().Name);
        }

        return tTypeNameList;
    }

    #region 元素状态判断

    /// <summary>
    /// 是否已添加
    /// </summary>
    public bool IsAdd(GameObject pGo, string pType)
    {
        UIElementData tElement = GetUIElementData(pGo, pType);
        if (tElement == null)
            return false;

        if (tElement.mGo == pGo)
            return true;

        return false;
    }

    /// <summary>
    /// 是否在待删列表中
    /// </summary>
    public bool IsWaitToDelete(string pName)
    {
        if (string.IsNullOrEmpty(pName))
            return false;

        return mWaitToDelectList.Contains(pName);
    }

    /// <summary>
    /// 是否已有同名的Go 被绑定了
    /// </summary>
    /// <param name="pGoName"></param>
    /// <returns></returns>
    public bool HasSameNameGoBind(GameObject pGo)
    {
        string tGoName = pGo.name;

        foreach (var tKv in mUIElementDic)
        {
            UIElementData tElement = tKv.Value;
            if (tElement.mGo.name == tGoName && tElement.mGo != pGo)
                return true;
        }

        return false;
    }

    #endregion

    #region 通用辅助部分

    /// <summary>
    /// 获取变量名, 对象做参数
    /// </summary>
    public static string GetVariableName(GameObject pGo, string pTypeName)
    {
        if (pGo == null)
            return null;

        string tName = GetVariableNameByStr(pGo.name, pTypeName);

        return tName;
    }

    /// <summary>
    /// 获取变量名
    /// </summary>
    public static string GetVariableNameByStr(string pGoName, string pTypeName)
    {
        string tGoName = Regex.Replace(pGoName, @"\d+ ", "");
        tGoName = tGoName.Replace(" ", "");

        string tName = string.Format("{0}_{1}", tGoName, pTypeName);

        return tName;
    }

    #endregion

    #region 操作数据的函数

    /// <summary>
    /// 将该名字 添加进待删列表 或 从待删列表中删除 
    /// </summary>
    public void ToggleWaitToDelete(string pName)
    {
        if (string.IsNullOrEmpty(pName))
            return;

        bool tHave = mWaitToDelectList.Contains(pName);
        if (tHave)
            mWaitToDelectList.Remove(pName);
        else
            mWaitToDelectList.Add(pName);
    }

    /// <summary>
    /// 添加新的元素
    /// </summary>
    public void AddNewUIElement(GameObject pGo, string pTypeName)
    {
        GameObject tGo = pGo as GameObject;
        if (tGo == null)
            return;

        string tName = GetVariableName(pGo, pTypeName);
        if (string.IsNullOrEmpty(tName))
            return;

        GameObject tRootGo = mPrefabGo as GameObject;
        UIElementData tData = UIElementData.Create(tRootGo, tName, tGo.GetParenRelativePath(tRootGo, false), pTypeName);

        UIElementData tAddedData = null;
        if (mUIElementDic.TryGetValue(tName, out tAddedData))
        {
            if (tData.mPath != tAddedData.mPath)
            {
                string tErrorInfo = string.Format(@"添加失败: 存在变量名相同， 但路径不同的其他对象， 冲突的另一个对象路径 = {0}", tAddedData.mPath);
                Debug.LogError(tErrorInfo);
                UIScriptCreatorEW.mInstance.ShowNotification(new GUIContent("添加失败"));
                return;
            }
        }

        mUIElementDic.Add(tData.mVarName, tData);
    }

    /// <summary>
    /// 删除掉待删选项
    /// </summary>
    public int DeleteWaitTodelete()
    {
        int tCount = 0;
        foreach (string tName in mWaitToDelectList)
        {
            ++tCount;
            mUIElementDic.Remove(tName);
        }

        mWaitToDelectList.Clear();

        return tCount;
    }

    /// <summary>
    /// 修复路径发生错误的数据， 名字改了， 父节点改了的情况
    /// </summary>
    public void RepairPathErrorUIElementData()
    {
        List<string> tOldNameList = new List<string>();
        List<UIElementData> tNewNameUIElementDataList = new List<UIElementData>();

        //更新数据
        foreach (var tKv in mUIElementDic)
        {
            UIElementData tUiElementData = tKv.Value;
            if (tUiElementData.mState != UIElementData.eState.Go_Type_PathError)
                continue;

            string tOldName = tUiElementData.mVarName;
            tUiElementData.UpdateDataByGo();

            if (tOldName == tUiElementData.mVarName)
                continue;

            tOldNameList.Add(tOldName);
            tNewNameUIElementDataList.Add(tUiElementData);
        }

        //处理用户 修改了prefab 节点名字的情况
        for (int i = 0; i < tNewNameUIElementDataList.Count; ++i)
        {
            string tOldName = tOldNameList[i];
            UIElementData tNewNameUIElementData = tNewNameUIElementDataList[i];

            mUIElementDic.Remove(tOldName);

            UIElementData tSameNameUIElement = null;
            if (mUIElementDic.TryGetValue(tNewNameUIElementData.mVarName, out tSameNameUIElement) == false)
            {
                //没有同名的情况
                mUIElementDic.Add(tNewNameUIElementData.mVarName, tNewNameUIElementData);
            }
            else
            {
                if (tSameNameUIElement.mGo == tNewNameUIElementData.mGo)
                {
                    //同名且是同一个Go, 直接替换原有的
                    mUIElementDic.Remove(tNewNameUIElementData.mVarName);
                    mUIElementDic.Add(tNewNameUIElementData.mVarName, tNewNameUIElementData);
                }
                else
                {
                    //同名但不同Go, 出错了
                    Debug.LogError("冲突了：相同变量名, 但 GameObject 不是同一个， " +
                                   "\nGameObject 路径分别为：" +
                                   "\n" + tSameNameUIElement.mPath +
                                   "\n" + tNewNameUIElementData.mPath +
                                   "\n已删除 " + tNewNameUIElementData.mPath + " 上的冲突，如需要这个对象, 请处理名字冲突后， 重新绑定");
                }
            }
        }
    }

    #endregion

    #region 获取数据

    /// <summary>
    /// 获取元素
    /// </summary>
    public UIElementData GetUIElementData(GameObject pGo, string pTypeName)
    {
        string tName = GetVariableName(pGo, pTypeName);

        UIElementData tUIElementData = null;
        mUIElementDic.TryGetValue(tName, out tUIElementData);

        return tUIElementData;
    }

    #endregion

    public void UpdateGoReference()
    {
        foreach (var tKv in mUIElementDic)
        {
            UIElementData tUIElementData = tKv.Value;
            tUIElementData.UpdateGo();
        }
    }

    public void RefreshUIElementState()
    {
        foreach (var tKv in mUIElementDic)
        {
            UIElementData tUIElementData = tKv.Value;

            tUIElementData.RefreshState();
        }
    }

    /// <summary>
    /// 获取筛选的元素数据
    /// </summary>
    /// <param name="pFilterStr"></param>
    /// <returns></returns>
    public List<UIElementData> GetElementListByFilterStr(string pFilterStr)
    {
        string tLowFilterStr = pFilterStr == null ? "" : pFilterStr.ToLower();

        List<UIElementData> tElementList = new List<UIElementData>();
        foreach (var tKv in mUIElementDic)
        {
            UIElementData tElement = tKv.Value;
            string tLowName = tElement.mVarName.ToLower();
            if (tLowName.Contains(tLowFilterStr) == false)
                continue;

            tElementList.Add(tKv.Value);
        }

        return tElementList;
    }

    #region 文件创建相关

    /// <summary>
    /// 在创建脚本时， 有脚本已存在了， 但不允许覆盖
    /// </summary>
    /// <returns></returns>
    public bool HasScripNotAllowToCoverInCreate()
    {
        foreach (var tScriptCreateConfig in mScriptCreateConfigList)
        {
            if (tScriptCreateConfig.IsNotAllowCover())
                return true;
        }

        return false;
    }

    public void CreateAllScript()
    {
        if (mPrefabGo == null)
            return;

        foreach (var tScriptCreateConfig in mScriptCreateConfigList)
        {
            if (tScriptCreateConfig.CanCreate())
                CreateScript(tScriptCreateConfig);
        }
    }

    public void CreateScript(UIScriptCreateConfig tConfig)
    {
        if (tConfig == null)
            return;

        //类名
        string tContent = File.ReadAllText(GetTemplatePath(tConfig.mScriptType));
        string tReplaceName = tConfig.mScriptName + "Template";
        string tClassName = GetClassNameByFilePath(tConfig.mPath);

        //特定的代码部分替换
        Func<string, string> tRepaceFunc = null;
        switch (tConfig.mScriptType)
        {
            case UIScriptCreateConfig.ScriptType.View:
                tRepaceFunc = ReplaceViewTag;
                break;
        }

        tContent = tContent.Replace(tReplaceName, tClassName);
        tContent = tRepaceFunc == null ? tContent : tRepaceFunc(tContent);

        //生成脚本文件
        string tDirectoryPath = tConfig.mPath.Substring(0, tConfig.mPath.LastIndexOf('\\'));
        if (Directory.Exists(tDirectoryPath) == false)
            Directory.CreateDirectory(tDirectoryPath);

        File.WriteAllText(tConfig.mPath, tContent);
    }

    public string GetTemplatePath(UIScriptCreateConfig.ScriptType pType)
    {
        switch (pType)
        {
            case UIScriptCreateConfig.ScriptType.View:
                return mViewTemplatyPath;

            case UIScriptCreateConfig.ScriptType.Controller:
                return mControllerTemplatyPath;

            case UIScriptCreateConfig.ScriptType.Model:
                return mModelTemplatyPath;

            case UIScriptCreateConfig.ScriptType.Manager:
                return mManagerTemplatyPath;

            case UIScriptCreateConfig.ScriptType.Proxy:
                return mProxyTemplatyPath;
        }

        return "";
    }

    /// <summary>
    /// 替换掉View模板代码的特定部分
    /// </summary>
    /// <returns></returns>
    public string ReplaceViewTag(string pContent)
    {
        string pPrefabPath = mPrefabRelativePath.Replace("Assets/Res/", "");
        pContent = pContent.Replace("PathTag", pPrefabPath);

        StringBuilder tMemberTagSb = new StringBuilder();
        StringBuilder tCheckNullTabSb = new StringBuilder();
        StringBuilder tBindElementTagTag = new StringBuilder();

        foreach (var tKv in mUIElementDic)
        {
            UIElementData tUIElementData = tKv.Value;

            tMemberTagSb.AppendLine(string.Format("{0,4}{1}  {2};", " ", tUIElementData.mTypeName,
                tUIElementData.mVarName));

            tCheckNullTabSb.AppendLine(string.Format(
                "{0,13}tElementDic.Add(new KeyValuePair<string, string>(\"{1}\",\"{2}\"));", " ", tUIElementData.mPath,
                tUIElementData.mTypeName));

            if (tUIElementData.mTypeName == mGoTypeName)
            {
                tBindElementTagTag.AppendLine(string.Format("{0,8}{1} = mRootTransform.Find(\"{2}\").gameObject;",
                    " ", tUIElementData.mVarName, tUIElementData.mPath));
            }
            else
            {
                tBindElementTagTag.AppendLine(string.Format(
                    "{0,8}{1} = mRootTransform.Find(\"{2}\").GetComponent<{3}>();",
                    " ", tUIElementData.mVarName, tUIElementData.mPath, tUIElementData.mTypeName));
            }
        }

        pContent = pContent.Replace("//MemberTag", tMemberTagSb.ToString());
        pContent = pContent.Replace("//BindElementTag", tBindElementTagTag.ToString());
        pContent = pContent.Replace("//CheckNullTag", tCheckNullTabSb.ToString());

        return pContent;

    }

    public string GetClassNameByFilePath(string pFilePath)
    {
        string tClassName = Regex.Match(pFilePath, @"\w+.cs").Value.Replace(".cs", "");
        return tClassName;

    }

    #endregion
}

