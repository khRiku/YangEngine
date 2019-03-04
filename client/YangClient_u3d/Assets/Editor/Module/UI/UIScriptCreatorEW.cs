using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityEditor.IMGUI.Controls;

/// <summary>
/// UI脚本创建器
/// @author:yxx
/// date: 2019/2/14
/// </summary>
public class UIScriptCreatorEW : BaseEditorWindow
{
    public static UIScriptCreatorEW mInstance;
    UIScriptCreatorManager mManager;

    public Rect windowRect = new Rect(0, 0, 200, 200);

    private Rect mBottomStartRec;                    //底部起始位置
    private Rect mLeftComponentSlectInfoRect;        //底部左边， 
    private Rect mRightComponentSlectInfoRect;       //底部右边， cell 的绘制起始处
     
    private int mLeftSlectIndex = -1;                //左边选择的 Index
    private UIElementData mBrSelectUiElementData;    //底右选中的元素

    private Vector2 mScrollPos = Vector2.zero;       //整个显示范围的滑动位置
    private Vector2 mBRScrollPos = Vector2.zero;     //底部右边显示已添加的元素的滑动位置

    private bool mShowCreateToggle = true;

    private SearchField mSearchField;              //搜索框


    #region 搜索框 相关字段
    private Rect mBRSerarchRect;                //搜索框显示范围
    private string mSearchFieldStr;             //搜索的字符

    private string mElementListFilterStr;       //筛选时用的字符
    private List<UIElementData> mFilterElementList = new List<UIElementData>();

    #endregion

    #region 颜色

    private Color mAddColor = Color.green;

    private Color mSelectColor = new Color(173f/255f, 216f/255f, 230f/255f);

    private Color mWarnColor = new Color(255f / 255f, 255f / 255f, 32f / 255f);
    private Color mErrorColor = new Color(255/255f, 59/255f, 59/255f);

    #endregion

    [MenuItem(MenuItemNameDefine.mUIScriptCreator)]
    private static void OpenWindow()
    {
        if (mInstance)
            return;

        mInstance = EditorWindow.CreateInstance<UIScriptCreatorEW>();
        mInstance.Show();
    }

    protected override void Init()
    {
        mManager = new UIScriptCreatorManager();
        mManager.Init();

        mSearchField = new SearchField();
        mSearchField.downOrUpArrowKeyPressed += OnSearchFildEditor;
    }


    protected override void DrawGUI()
    {
        mScrollPos = GUILayout.BeginScrollView(mScrollPos, false, true, GUILayout.Width(Screen.width - 10),GUILayout.Height(Screen.height - 30));
        {
            DrawTopInfo();

           GUILayout.Label("", "dockareaStandalone", GUILayout.Height(2f));

            DrawBottomInfo();
        }
        GUILayout.EndScrollView();
    }

    #region 事件监听函数

    private GameObject mInspectorUpdateSelectGo = null;
    private int mInspectorUpdateSelectGoComponentCount = 0;

    private void OnInspectorUpdate()
    {
        if (mManager == null || mManager.mPrefabGo == null)
            return;

        //检测组件删除的情况
        GameObject tActiveGo = Selection.activeGameObject;
        if (tActiveGo == null)
            return;

        if (tActiveGo != mInspectorUpdateSelectGo)
        {
            mInspectorUpdateSelectGo = tActiveGo;
            mInspectorUpdateSelectGoComponentCount = mInspectorUpdateSelectGo.GetComponents<Component>().Length;
        }

        //非当前操作 prefab 直接返回
        if (mInspectorUpdateSelectGo.transform.IsChildOf(mManager.mPrefabGo.transform) == false)
            return;


        int tComponentCount = mInspectorUpdateSelectGo.GetComponents<Component>().Length;

        if (mInspectorUpdateSelectGoComponentCount != tComponentCount)
        {
            mInspectorUpdateSelectGoComponentCount = tComponentCount;
            mManager.RefreshUIElementState();
        }
    }

    private void OnHierarchyChange()
    {
        if (mManager == null || mManager.mPrefabGo == null)
            return;

        mManager.UpdateGoReference();
        mManager.RefreshUIElementState();
    }

    /// <summary>
    /// 搜索框信息
    /// </summary>
    private void OnSearchFildEditor()
    {
        Debug.LogError("搜索框获取了焦点");
    }

    #endregion

    #region 绘制顶部的信息
    private void DrawTopInfo()
    {
        string tTriangleStr = mShowCreateToggle ? "▼" : "▶";
        if (GUILayout.Button(string.Format("{0}脚本生成", tTriangleStr), "dragtabdropwindow"))
            mShowCreateToggle = !mShowCreateToggle;
        GUILayout.Space(3f);

        if (mShowCreateToggle == false)
        {
            GUILayout.Space(8f);
            return;
        }

        //绘制创建的代码
        for (int i = 0; i < mManager.mScriptCreateConfigList.Count; ++i)
        {
            UIScriptCreateConfig tConfig = mManager.mScriptCreateConfigList[i];

            GUILayout.BeginHorizontal("OL box NoExpand");
            {
                tConfig.mCreate = EditorGUILayout.ToggleLeft(tConfig.mScriptName + " 脚本", tConfig.mCreate, GUILayout.Width(120f));
                tConfig.mCover = EditorGUILayout.ToggleLeft("可覆盖", tConfig.mCover, GUILayout.Width(70f));

                GUILayout.Label("路径:", GUILayout.Width(32));
                Color tOriginColor = GUI.color;
                Color tColor = tConfig.IsNotAllowCover()? Color.red : tOriginColor;
                GUI.color = tColor;
                tConfig.mPath = EditorGUILayout.TextField(tConfig.mPath);
                GUI.color = tOriginColor;

                if (GUILayout.Button("默认路径", GUILayout.Width(60f)))
                    tConfig.ResetPath();

                if (GUILayout.Button("创建", GUILayout.Width(40f)))
                {
                    if (mManager.HasScripNotAllowToCoverInCreate())
                    {
                        ShowNotification(new GUIContent("创建失败：文件已存在且不允许覆盖，重新设置操作"));
                    }
                    else
                    {
                        mManager.CreateScript(tConfig);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("一 键 创 建", "LargeButtonMid"))
        {
            mManager.CreateAllScript();
        }
        GUILayout.Space(5f);

    }
    #endregion

    #region 绘制底部的信息

    /// <summary>
    /// 显示底部信息
    /// </summary>
    private void DrawBottomInfo()
    {
        DrawBottomUIPrefabInfo();

        Rect tStartRect = GUILayoutUtility.GetRect(new GUIContent(""), "label");
        if (Event.current.type == EventType.Repaint)
            mBottomStartRec = tStartRect;

        Rect tThisRect = this.position;

        float tYPos = mBottomStartRec.y + 3;
        float tWidth = (float) (tThisRect.width * 0.32);
        Rect tLeftRect = new Rect(6f, tYPos, tWidth, tThisRect.height - tYPos - 15);
        Rect tRightRect = new Rect(tLeftRect.x + tLeftRect.width + 3f, tYPos,
            tThisRect.width - tWidth - 38, tLeftRect.height);

        GUILayout.BeginArea(tLeftRect, "", "HelpBox");
        DrawBottomLeft(tLeftRect);
        GUILayout.EndArea();

        GUILayout.BeginArea(tRightRect, "", "HelpBox");
        DrawBottomRightInfo(tRightRect);
        GUILayout.EndArea();

        Repaint();
    }

    private void DrawBottomUIPrefabInfo()
    {
        GUILayout.Label("▼元素选择", "dragtabdropwindow");

        GUI.changed = false;
        GameObject tGo = (GameObject) EditorGUILayout.ObjectField("Prefab", mManager.mPrefabGo, typeof(GameObject), true);
        if (GUI.changed == true)
        {
            bool tResult = mManager.SetUIPrefab(tGo);
            if (tResult == false && tGo != null)
            {
                ShowNotification(new GUIContent("请设置 Prefab 游戏对象"));
            }
            else
            {
                UpdateFilterElementList();
            }
        }

        EditorGUILayout.TextField("Prefab 位置：", mManager.mPrefabRelativePath);
    }

    #region 底部左边的绘制
    private GameObject mBLGoSelect = null;

    private void DrawBottomLeft(Rect pRect)
    {
        GUILayout.Label("游戏对象操作组件选择", "SettingsHeader");

        //调用GUILayoutUtility.GetRect 之后， 就相当于有这个 rect 的占用了
        Rect tRect = GUILayoutUtility.GetRect(new GUIContent(""), "OL box NoExpand", GUILayout.Width(pRect.width));
        if (Event.current.type == EventType.Repaint)
        {
            mLeftComponentSlectInfoRect = tRect;
            mLeftComponentSlectInfoRect.width -= 8;
            mLeftComponentSlectInfoRect.height += 6;
        }

       GameObject tSelectGo = Selection.activeGameObject;
        //处理选中状态
        if (mBLGoSelect != tSelectGo)
        {
            mBLGoSelect = tSelectGo;
            mLeftSlectIndex = -1;
        }

        List<string> tTypeNameList = mManager.GetGoTypeNameList(Selection.activeGameObject);
        for (int i = 0; i < tTypeNameList.Count; ++i)
        {
            string tTypeName = tTypeNameList[i];

            bool tIsAdd = mManager.IsAdd(tSelectGo, tTypeName);
            bool tIsSelect = mLeftSlectIndex == i;

            //修改背景颜色
            Color tColor = Color.white;
            if (tIsSelect)
            {
                tColor = mSelectColor;
            }
            else if (tIsAdd)
            {
                tColor = mAddColor;
            }

            Rect tBgBtnRect = new Rect(mLeftComponentSlectInfoRect.x,
                mLeftComponentSlectInfoRect.y + i * mLeftComponentSlectInfoRect.height,
                mLeftComponentSlectInfoRect.width,
                mLeftComponentSlectInfoRect.height);

            //绘制空label充当背景
            Color tTempColor = GUI.backgroundColor;
            GUI.backgroundColor = tColor;
            {
                GUI.Label(tBgBtnRect, "", "OL box NoExpand");
            }
            GUI.backgroundColor = tTempColor;

            Rect tContentRect = new Rect(tBgBtnRect);
            tContentRect.y += 3f;
            tContentRect.x += 3;
            tContentRect.width -= 5;

            GUILayout.BeginArea(tContentRect);
            GUILayout.BeginHorizontal();
            {
                //类型名
                GUILayout.Label(tTypeName);

                //按钮
                float tOpBtnWidth = 95f;
                if (tIsAdd)
                {
                    string tName = UIScriptCreatorManager.GetVariableName(Selection.activeGameObject, tTypeName);
                    bool tWaitToDelect = mManager.IsWaitToDelete(tName);
                    string tBtnName = tWaitToDelect ? "取消待删" : "待删";
                    if (GUILayout.Button(tBtnName, GUILayout.Width(tOpBtnWidth)))
                    {
                        mManager.ToggleWaitToDelete(tName);
                    }
                }
                else
                {              
                    if (GUILayout.Button("添加", GUILayout.Width(tOpBtnWidth)))
                    {
                        Action tAddAction = () =>
                        {
                            if (mManager.HasSameNameGoBind(tSelectGo))
                            {
                                //已有同名对象的被绑定了
                                mSearchFieldStr = tSelectGo.name;
                                ShowNotification(new GUIContent("添加失败， 已有同名的GameObject 绑定了, 请修改命名后再添加\n右边已筛选出名字相同的元素"));
                            }
                            else
                            {
                                mManager.AddNewUIElement(tSelectGo, tTypeName);
                                UpdateFilterElementList();
                            }
                        };

                        if (tSelectGo.name.Contains(" ") == true)
                        {
                            if (EditorUtility.DisplayDialog("错误", "选中的 GameObject 的名字有空格, 无法添加\n是否自动去掉空格后添加？", "好的", "不用，我自己处理"))
                            {
                                tSelectGo.name = tSelectGo.name.Replace(" ", "");
                                tAddAction();
                            }
                        }
                        else
                        {
                            tAddAction();
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //背景点击响应
            //放在单个cell的绘制最后， 这样如果前面有响应事件的ui， 就会消耗掉这个事件， 这边就不会检测到
            //ps: Unity显示的ui 是按显示顺序的先后来检测点击的， 先显示的会优先检测到事件， 用完可能会标记消耗， 后面就用不了了
            //根据上面的规则, ig: 两个按钮重叠， 点击后， 被覆盖到的那个会响应， 前面那个不会
            if (Event.current.type == EventType.MouseDown && tBgBtnRect.Contains(Event.current.mousePosition))
            {
                if (mLeftSlectIndex == i)
                    mLeftSlectIndex = -1;
                else
                    mLeftSlectIndex = i;
            }
        }
    }
    #endregion

    #region 底部右边的绘制

    private void DrawBottomRightInfo(Rect pRect)
    {
        //头部信息
        DrawBRTopInfo();
        DrawBRElementInfo(pRect);      
    }

    private void DrawBRTopInfo()
    {
        GUILayout.BeginHorizontal();
        {
            //绘制名字
            GUILayout.Label("已绑定的组件", "SettingsHeader", GUILayout.Width(130f));

            DrawBRTopOptionBtn();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            DrawBRTopSearchUI();
            DrawBRTopStateInfo();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制底部右边顶部的操作按钮
    /// </summary>
    private void DrawBRTopOptionBtn()
    {
        if (GUILayout.Button("删除待删", "LargeButton", GUILayout.Width(100f)))
        {
            mManager.DeleteWaitTodelete();
            UpdateFilterElementList();
        }

        if (GUILayout.Button(new GUIContent("刷新", " prefab 修改后进行刷新 "), "LargeButton", GUILayout.Width(80f)))
        {
            mManager.RepairPathErrorUIElementData();
            UpdateFilterElementList();

        }

        if (GUILayout.Button(new GUIContent("匹配修复", "处理的问题：\n位置修改\n名称修改\n名称存在空格"), "LargeButton", GUILayout.Width(100f)))
        {
            mManager.RepairPathErrorUIElementData();
            UpdateFilterElementList();
        }
    }

    /// <summary>
    /// 绘制底部右边顶部的 搜索按钮
    /// </summary>
    private void DrawBRTopSearchUI()
    {
        Rect tRect = GUILayoutUtility.GetRect(new GUIContent(""), "label", GUILayout.Width(315f));
        if (Event.current.type == EventType.Repaint)
        {
            mBRSerarchRect = tRect;

            mBRSerarchRect.y += 3;
        }

        mSearchFieldStr = mSearchField.OnGUI(mBRSerarchRect, mSearchFieldStr);
    }

    private Rect mBRTopStateInfoRect;

    /// <summary>
    /// 显示状态信息
    /// </summary>
    private void DrawBRTopStateInfo()
    {
        Rect tRect = GUILayoutUtility.GetRect(new GUIContent(""), "CN EntryErrorIconSmall");
        if (Event.current.type == EventType.Repaint)
        {
            mBRTopStateInfoRect = tRect;
            mBRTopStateInfoRect.x -= 3f;
            mBRTopStateInfoRect.y -= 3f;
        }

        Rect tStartRect = mBRTopStateInfoRect;

        if (mHasPathError)
        {
            GUI.Label(tStartRect, "", "CN EntryWarnIconSmall");
            tStartRect.x += 18f;
        }

        if (mHasReferenceNull)
        {
            GUI.Label(tStartRect, "", "CN EntryErrorIconSmall");
        }
    }

    private bool mHasPathError = false;
    private bool mHasReferenceNull = false;

    /// <summary>
    /// 绘制已添加的元素
    /// </summary>
    private void DrawBRElementInfo(Rect pRect)
    {
        mBRScrollPos = GUILayout.BeginScrollView(mBRScrollPos, "GroupBox");
        {
            //调用GUILayoutUtility.GetRect 之后， 就相当于有这个 rect 的占用了
            Rect tRect = GUILayoutUtility.GetRect(new GUIContent(""), "OL box NoExpand",GUILayout.Width(pRect.width - 45));
            if (Event.current.type == EventType.Repaint)
            {
                mRightComponentSlectInfoRect = tRect;

                mRightComponentSlectInfoRect.width += 5;
                mRightComponentSlectInfoRect.height += 6;
            }

            if (mSearchFieldStr != mElementListFilterStr)
            {
                mElementListFilterStr = mSearchFieldStr;
                UpdateFilterElementList();
            }

            mHasPathError = false;
            mHasReferenceNull = false;

            for (int i = 0; i < mFilterElementList.Count; ++i)
            {
                UIElementData tUIElementData = mFilterElementList[i];

                //背景Rect
                Rect tBgRect = new Rect(mRightComponentSlectInfoRect);
                tBgRect.height -= 2;
                tBgRect.y += tBgRect.height * i;

                bool tIsSelect = mBrSelectUiElementData == tUIElementData;

                //空的高度占位符
                GUILayout.Label("", GUILayout.Height(tBgRect.height));

                //修改背景颜色
                Color tColor = Color.white;
                if (tIsSelect)
                {
                    tColor = mSelectColor;
                }
                else if (tUIElementData != null)
                {
                    switch (tUIElementData.mState)
                    {
                        case UIElementData.eState.GoNull:
                        case UIElementData.eState.Go_TypeNull:
                            tColor = mErrorColor;
                            mHasReferenceNull = true;
                            break;

                        case UIElementData.eState.Go_Type_PathError:
                            tColor = mWarnColor;
                            mHasPathError = true;
                            break;
                    }
                }

                //绘制空label 充当背景颜色
                Color tTempColor = GUI.backgroundColor;
                GUI.backgroundColor = tColor;
                {
                    GUI.Label(tBgRect, "", "OL box NoExpand");
                }
                GUI.backgroundColor = tTempColor;

                //内容用的Rect
                Rect tContentRect = new Rect(tBgRect);
                tContentRect.x += 3;
                tContentRect.y += 2;
                tContentRect.width -= 5;

                GUILayout.BeginArea(tContentRect);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(tUIElementData.mVarName);

                    //选中按钮
                    if (GUILayout.Button("选中", GUILayout.Width(35f)))
                    {
                       Selection.activeObject = tUIElementData.mGo;
                       EditorGUIUtility.PingObject(tUIElementData.mGo);
                    }


                    //删除按钮
                    float tBtnMaxWidth = 85f;

                    bool tWaitToDelect = mManager.IsWaitToDelete(tUIElementData.mVarName);
                    string tBtnName = tWaitToDelect ? "取消待删" : "待删";
                    if (GUILayout.Button(tBtnName, GUILayout.MaxWidth(tBtnMaxWidth)))
                    {
                        mManager.ToggleWaitToDelete(tUIElementData.mVarName);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                //背景点击监听
                if (Event.current.type == EventType.MouseDown && tBgRect.Contains(Event.current.mousePosition))
                {

                    if (mBrSelectUiElementData == tUIElementData)
                        mBrSelectUiElementData = null;
                    else
                        mBrSelectUiElementData = tUIElementData;
                }
            }
        }
        GUILayout.EndScrollView();
    }

    #endregion
    #endregion

    #region 数据操作

    /// <summary>
    /// 更新筛选的元素
    /// </summary>
    private void UpdateFilterElementList()
    {
        mFilterElementList = mManager.GetElementListByFilterStr(mElementListFilterStr);
    }

    #endregion
}
