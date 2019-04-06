using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// author: yangxuxiang(YXX)
/// date: 2019/3/09
/// </summary>

/*Ĭ��ui�Ű�Ҫ��
   ScrollView  �������Ҫ��
     Viewport  ��ScrollRect �����ˣ� ƥ�� ScrollView �ĳ��� 
               ������й�����, ViewPort �ĳ�����ȥ�� ������ռ�õĿռ䣬 ��֤�����������ü�
       Content   ê�㲻�����Ƶ��� ��������Ϊ���Ͻ�

    Ps: UGUIGridWrapContent �ű�����Content �ϣ� �����Go, �����Լ����ģ� �����ֻ�� ugui ����ʱ��Ĭ������
*/

public class UGUIGridWrapContent : MonoBehaviour
{
    #region  �Ű����

    public int mOffsetX = 0;           //���Ͻ� x λ��ƫ��
    public int mOffsetY = 0;           //���Ͻ� y λ��ƫ��

    public int mCellWidth = 100;       //����СUI ���
    public int mCellHeight = 100;      //����СUI �߶�

    public int mHorizontalCnt = 0;     //ˮƽ���� ÿ���м���
    public int mVerticalCnt = 0;       //��ֱ���� ÿ���м���

    //ÿ���û������� cell ��ʱ��ê���������ֶ�����ֻҪ�Ǳ�׼��ê������, �ɼ����ƫ��ֵ�� ���ε��������
    private Vector2 mCellOffsetPos = Vector2.zero;

    /// <summary>
    /// ��������
    /// Horizontal:
    ///     1  2  3
    ///     4  5  6
    /// 
    /// Vertical:
    ///     1  4
    ///     2  5
    ///     3  6
    ///
    /// HorizontalPage��
    ///     1  2  3  10  11  12
    ///     4  5  6  13  14
    ///     7  8  9
    /// </summary>
    public enum ArrangeType
    {
        Horizontal,         // ������
        Vertical,           // ���ϵ���
        HorizontalPage,     // �����ң� ҳ����ʽ
    }
    #endregion

    public ArrangeType mArrangeType;
    public UGUIGridWrapContentConfig mConfig;
    public UGUIGridArrangeBase mGridArrangeBase;

    #region ���ݻ��� 

    public ScrollRect mScrollRect;
    public RectTransform mScrollRectTransform;
    public RectTransform mRectTransform;
    public RectTransform mViewPortRectTransform;

    //�¼�����
    public UGUIGWCEventListenter mUGUIGWCEventListenter { get; private set; }

    public Vector2 mViewSize;              //��������� ����

    private List<GameObject> mCellList = new List<GameObject>();   //ʵ������cell ����


    private int mInstaceCellStartDataIndex = 0;    //ʵ������cell ����ʼ��������

    private List<int> mDataIndexList = new List<int>(); //�洢ʵ������cell �ж�Ӧ�� ��������
    private Dictionary<int, GameObject> mCellDic = new Dictionary<int, GameObject>();  //key: ���������� 


    private Vector3 mPosition = Vector3.zero;
    private bool mPosChange = false;
    #endregion

    #region �¼�

    /// ������Ŀ��λ�ú���¼�֪ͨ
    public Action mScrollToTargetFinishAction { get; private set; } 

    #endregion

    void Awake()
    {
       CacheData();
       RegisterEvent();
    }



    // Update is called once per frame
    void Update()
    {
        //if(mScrollRect.velocity != Vector2.zero)
        //Debug.LogError(mScrollRect.velocity);

        if (mGridArrangeBase == null)
            return;

        //������ָ��λ�õ�Tween����
        if (mStartScrollToTargetPos)
            UpdateScrollToTarget();

        //����λ��ˢ��Cell ����ʾ
        if (mPosition != this.transform.position)
        {
            //if (mPosChange == false)
            //    Debug.LogError("Positin �ı�, λ�� = ");

            mPosChange = true;
            mPosition = this.transform.position;
        }
        else if (mPosition == this.transform.position && mPosChange)
        {
            mPosChange = false;
            OnPosEndChange();
            // Debug.LogError("Positin ֹͣ�ı�, λ�� = ");

        }

        if (mPosChange)
            OnPosEndChange();
    }

    private void OnPosEndChange()
    {
        RefreshAllCellPos();
    }

    //ˢ�� Cell ��λ��
    private void RefreshAllCellPos()
    {
        //��ʼ�㲻�������� λ��Ҳ����Ҫ�ı䣬 ֱ�ӷ���
        int tInstaceCellStartDataIndex = mGridArrangeBase.GetNewStartDataIndex();
        if(mInstaceCellStartDataIndex == tInstaceCellStartDataIndex)
            return;
        
        mInstaceCellStartDataIndex = tInstaceCellStartDataIndex;

        //�µ�����������
        List<int> tNewDataIndexList = mGridArrangeBase.GetNewDataIndexList();

        int tNewI = 0;
        int tOldI = 0;

        int i = 0; 
        while (true)
        {
            ++i;
            if (i > 150)
            {
                Debug.LogError("cell ˢ��λ��ʱ��ѭ����������150�Σ� ������ ���˳�");
                return;
            }

            //�ҳ��µ� ��������
            int tNewDataIndex = -1;
            for (; tNewI < tNewDataIndexList.Count; ++tNewI)
            {
                int tDataIndex = tNewDataIndexList[tNewI];

                //�µ��������ϵ�Ҳ�У� ���ô���
                if (mCellDic.ContainsKey(tDataIndex))
                    continue;

                tNewDataIndex = tDataIndex;
                break;
            }

            //û�µ�ֱ�ӷ���
            if (tNewDataIndex == -1)
                break;

            //�����õ����������滻Ϊ�µ�
            for (; tOldI < mDataIndexList.Count; ++tOldI)
            {
                int tOldDataIndex = mDataIndexList[tOldI];

                //�ϵ��������µ�Ҳ�У� ���ô���
                if (tNewDataIndexList.Contains(tOldDataIndex))
                    continue;

                mDataIndexList[tOldI] = tNewDataIndex;
                GameObject tCell = mCellDic[tOldDataIndex];

                mCellDic.Remove(tOldDataIndex);
                mCellDic.Add(tNewDataIndex, tCell);

                RectTransform tRectTransform = tCell.transform as RectTransform;
                tRectTransform.anchoredPosition = GetAnchorPosByDataIndex(tNewDataIndex);
         
                mConfig.mDisplayCellAction(tNewDataIndex, tCell);

                break;
            }
        }
    }



    private void CacheData()
    {
        //�������
        mRectTransform = this.transform as RectTransform;

        mViewPortRectTransform = this.transform.parent as RectTransform;
        mScrollRect = mViewPortRectTransform.parent.GetComponent<ScrollRect>();
        mScrollRectTransform = mScrollRect.transform as RectTransform;

        mUGUIGWCEventListenter =  mScrollRect.GetComponent<UGUIGWCEventListenter>();
        if (mUGUIGWCEventListenter == null)
            mUGUIGWCEventListenter = mScrollRect.gameObject.AddComponent<UGUIGWCEventListenter>();

        //��������
        mViewSize = new Vector2(mViewPortRectTransform.rect.width, mViewPortRectTransform.rect.height);
    }

    #region �¼�ע�����Ӧ
    private void RegisterEvent()
    {
        mUGUIGWCEventListenter.mOnBeginDrag.AddListener(OnBeginDrag);
        mUGUIGWCEventListenter.mOnEndDrag.AddListener(OnEndDrag);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EndScrollToTargetPos();
        mBeginDragAnchorPos = mRectTransform.anchoredPosition;
        mBeginDragTime = Time.unscaledTime;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 tPosOffset = mRectTransform.anchoredPosition - mBeginDragAnchorPos;
        float tTimeOffset = Time.unscaledTime - mBeginDragTime;

        TrySupplementDrag(tPosOffset, tTimeOffset);
    }

    #endregion


    private UGUIGridArrangeBase GetGridArrangeInstance()
    {
        switch (mArrangeType)
        {
            case ArrangeType.Horizontal:
                return new UGUIGridArrangeHorizontal(this);

            case ArrangeType.HorizontalPage:
                return new UGUIGridArrangeHorizontalPage(this);

            case ArrangeType.Vertical:
                return new UGUIGridArrangeVertical(this);
        }

        Debug.LogError("ƥ�䲻����Ӧ�������㷨��  mArrangeType = " + mArrangeType);
        return null;
    }

    public void Show(UGUIGridWrapContentConfig pConfig)
    {
        mConfig = pConfig;
        SetDefaultValue();

        mGridArrangeBase = GetGridArrangeInstance();

        InitSeeting();

        mGridArrangeBase.AdjustContentSize();
        CreateAllCellInstance();
        int a = 10;
    }

    //����һЩĬ��ֵ
    private void SetDefaultValue()
    {
        if (mCellHeight <= 0)
            mCellHeight = 10;

        if (mCellWidth <= 10)
            mCellWidth = 1;

        if (mHorizontalCnt <= 0)
            mHorizontalCnt = 1;

        if (mVerticalCnt <= 0 )
            mVerticalCnt = 1;
    }

    //��ʼ�� Content ��һЩ����
    private void InitSeeting()
    {
        mRectTransform.pivot = new Vector2(0, 1);

        mCellOffsetPos = new Vector2(float.MaxValue, float.MaxValue);
    }

    //�޸���ʵ������cell ����
    private void CreateAllCellInstance()
    {
        int tViewCnt = mGridArrangeBase.GetCellsCountByViewSize();
        int tInstanceCnt = Mathf.Min(mConfig.mDataCnt, tViewCnt);

        mInstaceCellStartDataIndex = 0;
        mDataIndexList.Clear();
        mCellDic.Clear();

        List<int> tNewDataIndexList = mGridArrangeBase.GetNewDataIndexList();

        //����cell
        int i = 0;
        for (; i < tInstanceCnt; ++i)
        {
            GameObject tCell = null;

            if (i < mCellList.Count)
            {
                tCell = mCellList[i];
                tCell.SetActive(true);
            }
            else
            {
                tCell = mConfig.CreateCell();
                tCell.SetActive(true);

                mCellList.Add(tCell);
            }

            int tDataIndex = tNewDataIndexList[i];
            mDataIndexList.Add(tDataIndex);
            mCellDic.Add(tDataIndex, tCell);

            RectTransform tRectTransform = tCell.transform as RectTransform;

            if (mCellOffsetPos.x == float.MaxValue)
                mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            tRectTransform.anchoredPosition = GetAnchorPosByDataIndex(tDataIndex);

            mConfig.mDisplayCellAction(tDataIndex, tCell);
        }

        //���ض����cell
        for (; i < mCellList.Count; ++i)
        {
            mCellList[i].SetActive(false);
        }
    }

    public Vector2 GetAnchorPosByDataIndex(int pDataIndex)
    {
        Vector2 tGridPos = mGridArrangeBase.GetAnchorPosByDataIndex(pDataIndex);
        Vector2 tAnchorPos = tGridPos + mCellOffsetPos;

        tAnchorPos.x += mOffsetX;
        tAnchorPos.y += mOffsetY;

        return tAnchorPos;
    }

    //��ȡcell ��λ��ƫ�ƣ� Ŀ����ʹ���λ��content �����Ͻ�
    private Vector2 GetCellOffsetValue(RectTransform pCell)
    {
#if UNITY_EDITOR
        if (pCell.IsStandAnchorType() == false)
        {
            Debug.LogError("cell ��ê������Ǳ�׼����ʽ�� ����TopLeft, TopCenter ... ... ����");
        }
#endif
        RectTransformAnchorType tAnchorType = pCell.GetAnchorType();
        Vector2 tAnchorValue = RectTransformExtension.GetAnchorTypeValue(tAnchorType).Key;
        Vector2 tTLAnchorValue = RectTransformExtension.GetAnchorTypeValue(RectTransformAnchorType.TopLeft).Key;
        Vector2 tAnchorOffset = tTLAnchorValue - tAnchorValue;

        Vector2 tAnchorOffsetPos = new Vector2(mRectTransform.rect.width * tAnchorOffset.x, mRectTransform.rect.height * tAnchorOffset.y);

        Vector2 tTLPivotValue = new Vector2(0, 1);
        Vector2 tPivotOffset = pCell.pivot - tTLPivotValue;

        Vector2 tPivotOffsetPos = new Vector2(tPivotOffset.x * pCell.rect.width, tPivotOffset.y * pCell.rect.height);

        Vector2 tTotalOffsetPos = new Vector2(tAnchorOffsetPos.x + tPivotOffsetPos.x, tAnchorOffsetPos.y + tPivotOffsetPos.y);

        return tTotalOffsetPos;
    }

    #region �������㹦��

    public bool mEnalbDragSupplement = false;       //�Ƿ����������㣬 ��Χ�ж���ViewPort�Ŀ�������Ϊ׼
    public float mDragSupplementViewSizeScale = 0.7f;       //�������㷶Χ����, 0f-1f, ���ſ��ӷ�Χ��
    public float mDrageSupplementVelocity = 0.1f;      //���㻬��ʱ���ٶȵ�������

    public int mDragSupplementIndex { get; private set; }   //��ǰ�Ļ�����������

    private Vector2 mBeginDragAnchorPos = new Vector2(-1f, -1f);
    private float mBeginDragTime = 0f;

    public enum SupplementType
    {
        None,     //���ò���
        Floor,    //���²���
        Ceil,     //���ϲ���
    }
    /// <summary>
    /// ���Բ�����ק
    /// </summary>
    private void TrySupplementDrag(Vector2 pDragPosOffset, float pDragTimeOffset)
    {
        if (mEnalbDragSupplement == false)
            return;

        SupplementType tSupplementType = GetDragSupplementType(pDragPosOffset, pDragTimeOffset);
        int tMaxDragSupplementIndex = mGridArrangeBase.GetMaxDragSupplementIndex();

        int tDragSupplementIndex = mDragSupplementIndex;
        switch (tSupplementType)
        {
            case SupplementType.Ceil:
                tDragSupplementIndex = Mathf.Min(tMaxDragSupplementIndex, tDragSupplementIndex + 1);
                break;

            case SupplementType.Floor:
                tDragSupplementIndex = Mathf.Max(0, tDragSupplementIndex - 1);
                break;
        }

        mDragSupplementIndex = tDragSupplementIndex;

        Vector2 tTargetPos = mGridArrangeBase.GetDragSupplemnetAnchorPos(tDragSupplementIndex);
        StartScrollToTargetPos(tTargetPos, mDrageSupplementVelocity);
      //  Debug.LogError(string.Format("index = {0}  tTargetPos = {1}", tDragSupplementIndex, tTargetPos));
    }

    private SupplementType GetDragSupplementType(Vector2 pDragPosOffset, float pDragTimeOffset)
    {
        float tPosOffset = mScrollRect.vertical ? pDragPosOffset.y : pDragPosOffset.x;
        float tAbsPosOffset = Mathf.Abs(tPosOffset);

        float tViewSize = mScrollRect.vertical ? mViewPortRectTransform.rect.height : mViewPortRectTransform.rect.width;
        tViewSize *= mDragSupplementViewSizeScale;

        //���ٻ��������
        if (pDragTimeOffset < 0.3f)
        {
            if (mScrollRect.vertical)
            {
                return pDragPosOffset.y > 0 ? SupplementType.Ceil : SupplementType.Floor;
            }
            else
            {
                return pDragPosOffset.x < 0 ? SupplementType.Ceil : SupplementType.Floor;
            }
        }

        //����������ж�
        if(tAbsPosOffset < tViewSize / 2)
            return SupplementType.None;

        if (mScrollRect.vertical)
        {
            return tPosOffset > 0 ? SupplementType.Ceil : SupplementType.Floor;
        }
        else
        {
            return tPosOffset < 0 ? SupplementType.Ceil : SupplementType.Floor;
        }
    }

    #endregion

    #region ������ָ��λ��

    private bool mStartScrollToTargetPos = false;
    private Vector2 mScrollTargetPos;
    private float mScrollStrength;

    private void StartScrollToTargetPos(Vector2 pTargetPos, float pStrength)
    {
        //�϶�ʱ�� ScrollRect ���¼�ٶ����޸����޸�λ�ã� ����Լ�Ҫ���Ż��������� �ֶ�ֹͣ���ٶ�
        mScrollRect.StopMovement();

        mStartScrollToTargetPos = true;
        mScrollTargetPos = pTargetPos;
        mScrollStrength = pStrength;
    }

    private void EndScrollToTargetPos()
    {
        mStartScrollToTargetPos = false;
    }

    private void UpdateScrollToTarget()
    {
        Vector2 tCurPos = mRectTransform.anchoredPosition;

        if (Vector2.Distance(tCurPos, mScrollTargetPos) < 0.1f)
        {
            EndScrollToTargetPos();
            mRectTransform.anchoredPosition = mScrollTargetPos;

            mScrollToTargetFinishAction?.Invoke();

            return;
        }

        Vector2 tLerpPos = Vector2.Lerp(tCurPos, mScrollTargetPos, mScrollStrength);

        //Debug.LogError(string.Format("CurPos = {0}  mScrollTargetPos = {1}  tLerpPos = {2}",
        //    mRectTransform.anchoredPosition,
        //    mScrollTargetPos,
        //    tLerpPos
        //));

        mRectTransform.anchoredPosition = tLerpPos;


    }

    #endregion


    #region Unity_Editor

#if UNITY_EDITOR

    public void InspectorInit()
    {
        if (mCellOffsetPos != Vector2.zero)
            return;

        CacheData();
        SetDefaultValue();
        InitSeeting();
    }

    public void RepositionCellInEditor()
    {
        //һ��{}�Ĵ�����㴦��ģ� ���� UGUIGridArrageHorizontalPage �л��õ� mConfig Ȼ�󱨴� �����������������ٿ�
        {
            this.mConfig = new UGUIGridWrapContentConfig();
            mConfig.mDataCnt = 30;
        }

        if (Application.isPlaying == false)
            mGridArrangeBase = GetGridArrangeInstance();

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            RectTransform tRectTransform = this.transform.GetChild(i) as RectTransform;
            mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            Vector2 tAnchorPos = GetAnchorPosByDataIndex(i);

            tRectTransform.anchoredPosition = tAnchorPos;
        }
    }

#endif
    #endregion

    #region �ⲿ�ӿ� ��������

    public enum FixPosType
    {
        Center = -100,     //��λ������������м�
        Last = -200,       //��λ��������������
    }

    /// <summary>
    /// ��λ��ָ������������
    /// </summary>
    /// <param name="pDataIndex"></param>
    /// <param name="pPostType">��λ����������ĵڼ���, 1 Ϊ �� 1 ��(�У��� 2 Ϊ �� 2 �У��У�, ����λ�ÿ� FixPosType </param>
    public void FixToDataIndex(int pDataIndex, float pVelocity = 1f, int pPosType = 1)
    {
        Vector2 tFixAnchorPos = mGridArrangeBase.GetFixAnchorPos(pDataIndex, pPosType);
        StartScrollToTargetPos(tFixAnchorPos, pVelocity);
        
        //mRectTransform.anchoredPosition = tFixAnchorPos;

        //RefreshAllCellPos();
    }

    // <summary>
    /// �ı��������� ������ˢ��cell ����ʾ�� �ᱣ����ǰ�Ļ���״̬
    /// </summary>
    /// <param name="pFixToLast">�޸ĺ� �Զ���λ�����һ��</param>
    public void ChangeDataCount(int pCount, bool pFixToLast = false)
    {
        Vector2 tCurPos = mRectTransform.anchoredPosition;

        mConfig.mDataCnt = pCount;
        Show(mConfig);

        if (pFixToLast)
        {
            FixToDataIndex(pCount - 1, (int)FixPosType.Last);
        }
        else
        {
            mRectTransform.anchoredPosition = mGridArrangeBase.AdjustAnchorPos(tCurPos);
        }
    }

    /// <summary>
    ///ˢ������cell ����ʾ
    /// </summary>
    public void RefreshAll()
    {
        for (int i = 0; i < mDataIndexList.Count; ++i)
        {
            int tDataIndex = mDataIndexList[i];
            GameObject tGo = mCellDic[tDataIndex];
            mConfig.mDisplayCellAction(tDataIndex, tGo);
        }
    }

    /// <summary>
    /// ͨ����������ˢ��ָ����cell
    /// </summary>
    public void RefrehsByDataIndex(int pDataIndex)
    {
        if (mDataIndexList.Contains(pDataIndex) == false)
            return;

        GameObject pGo = mCellDic[pDataIndex];
        mConfig.mDisplayCellAction(pDataIndex, pGo);
    }

    /// <summary>
    /// ͨ��GameObject����ˢ��ָ����cell
    /// </summary>
    public void RefrehsByGameObject(GameObject pGo)
    {
        int tDataIndex = GetDataIndexByGo(pGo);
        if (tDataIndex < 0)
            return;

        mConfig.mDisplayCellAction(tDataIndex, pGo);
    }

    /// <summary>
    /// ���� Go ����ȡ������������ͬ�Ļ���״̬�� ͬһGo���ܻ��Ӧ��ͬ��DataIndex)
    /// </summary>
    public int GetDataIndexByGo(GameObject pGo)
    {
        foreach (var tKv in mCellDic)
        {
            if (tKv.Value == pGo)
                return tKv.Key;
        }

        return -1;
    }

    /// <summary>
    /// ��������������ȡGo, ����Ϊ��(������������ʱû����֮��Ӧ��Go)
    /// </summary>
    public GameObject GetGoByDataIndex(int pDataIndex)
    {
        GameObject tGo = null;
        mCellDic.TryGetValue(pDataIndex, out tGo);

        return tGo;
    }
    #endregion


}