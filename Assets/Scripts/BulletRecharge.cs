using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class BulletRecharge : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIController _uiController;
    [SerializeField] private CanvasGroup _canvasGroup;

    //State
    private bool _dragging;
    private Vector3 _dragStartPosition;

    public event Action BeginDragProxyEvent;
    public event Action EndDragProxyEvent;


    private void Start()
    {
        _dragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        _dragStartPosition = _rectTransform.localPosition;
        _canvasGroup.blocksRaycasts = false;
        BeginDragProxyEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragging)
        {
            _rectTransform.anchoredPosition += eventData.delta * _canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Release();
        EndDragProxyEvent?.Invoke();
    }

    public void Release()
    {
        _dragging = false;
        _rectTransform.localPosition = _dragStartPosition;
        _canvasGroup.blocksRaycasts = true;

    }
}
