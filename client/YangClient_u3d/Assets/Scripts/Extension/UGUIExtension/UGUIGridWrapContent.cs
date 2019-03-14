using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
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
    /// VerticalPage��
    ///     1  2  3  10  11  12
    ///     4  5  6  13  14
    ///     7  8  9
    /// </summary>
    public enum ArrangeType
    {
        Horizontal,       // ������
        Vertical,         // ���ϵ���
        VerticalPage,     // �����ң� ҳ����ʽ
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

    public Vector2 mViewSize;              //��������� ����

    private List<GameObject> mCellList = new List<GameObject>();   //ʵ������cell ����


    private int mInstaceCellStartDataIndex = 0;    //ʵ������cell ����ʼ��������

    private List<int> mDataIndexList = new List<int>(); //�洢ʵ������cell �ж�Ӧ�� ��������
    private Dictionary<int, GameObject> mCellDic = new Dictionary<int, GameObject>();  //key: ���������� 


    private Vector3 mPosition = Vector3.zero;
    private bool mPosChange = false;

    #endregion

    void Awake()
    {
       CacheData();
    }

    // Update is called once per frame
    void Update()
    {
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

        if(mPosChange)
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

        //��������
        mViewSize = new Vector2(mViewPortRectTransform.rect.width, mViewPortRectTransform.rect.height);
    }

    private UGUIGridArrangeBase GetGridArrangeInstance()
    {
        switch (mArrangeType)
        {
            case ArrangeType.Horizontal:
                return new UGUIGridArrangeHorizontal(this);

            case ArrangeType.VerticalPage:
                return new UGUIGridArrangeHorizontalPage(this);

            case ArrangeType.Vertical:
                return new UGUIGridArrangeVertical(this);
        }

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

            mDataIndexList.Add(i);
            mCellDic.Add(i, tCell);

            RectTransform tRectTransform = tCell.transform as RectTransform;

            if (mCellOffsetPos.x == float.MaxValue)
                mCellOffsetPos = GetCellOffsetValue(tRectTransform);

            tRectTransform.anchoredPosition = GetAnchorPosByDataIndex(i);

            mConfig.mDisplayCellAction(i, tCell);
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

        Vector2 tAnchorOffsetPos = new Vector2(mRectTransform.rect.width * tAnchorOffset.x,
           mRectTransform.rect.height * tAnchorOffset.y);

        Vector2 tTLPivotValue = new Vector2(0, 1);
        Vector2 tPivotOffset = pCell.pivot - tTLPivotValue;

        Vector2 tPivotOffsetPos = new Vector2(tPivotOffset.x * pCell.rect.width,
            tPivotOffset.y * pCell.rect.height);

        Vector2 tTotalOffsetPos = new Vector2(tAnchorOffsetPos.x + tPivotOffsetPos.x,
            tAnchorOffsetPos.y + tPivotOffsetPos.y);

        return tTotalOffsetPos;
    }

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
    public void FixToDataIndex(int pDataIndex, int pPosType = 1)
    {
        Vector2 tFixAnchorPos = mGridArrangeBase.GetFixAnchorPos(pDataIndex, pPosType);
        mRectTransform.anchoredPosition = tFixAnchorPos;

        RefreshAllCellPos();
    }

    #endregion
}