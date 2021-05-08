using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PowerLever : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private UIController _uiController;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float maxY;
    [SerializeField] private float minY;
    

    //State
    private bool _dragging;
    private Vector3 _dragStartPosition;
    private Vector3 _dragTarget;
    private bool _draggingUp;

    public event Action BeginDragProxyEvent;
    public event Action EndDragProxyEvent;
    public event Action<float> DragProgressProxyEvent;


    private void Start()
    {
        _dragging = false;
        _draggingUp = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        _canvasGroup.blocksRaycasts = false;
        BeginDragProxyEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float newY = Mathf.Clamp(_rectTransform.anchoredPosition.y + eventData.delta.y * _canvas.scaleFactor, minY, maxY);
        
        if ((_draggingUp && eventData.delta.y > 0) ||
            !_draggingUp && eventData.delta.y < 0)
        {
            _rectTransform.anchoredPosition = new Vector3(_rectTransform.anchoredPosition.x, newY);

            if (_draggingUp && newY == maxY)
            {
                _draggingUp = false;
            }
            else if (!_draggingUp && newY == minY)
            {
                _draggingUp = true;
            }

            DragProgressProxyEvent?.Invoke((newY - minY)/ (maxY - minY));
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        _canvasGroup.blocksRaycasts = true;
        EndDragProxyEvent?.Invoke();
    }
}
