using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 事件监听类
/// </summary>
public class UGUIGWCEventListenter : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public class UnityEvent<T0> : UnityEngine.Events.UnityEvent<T0>
    {

    }

    public UnityEvent<PointerEventData> mOnBeginDrag { get; private set; }
    public UnityEvent<PointerEventData> mOnEndDrag { get; private set; }
    public UnityEvent<PointerEventData> mOnDrag { get; private set; }

    public void SetUp()
    {
        mOnBeginDrag = new UnityEvent<PointerEventData>();
        mOnEndDrag = new UnityEvent<PointerEventData>();
        mOnDrag = new UnityEvent<PointerEventData>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mOnBeginDrag.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mOnEndDrag.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        mOnDrag.Invoke(eventData);
    }
}
