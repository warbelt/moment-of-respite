using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UpgradeGauge : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private Image _movingGauge;

    [SerializeField] RectTransform _gaugeMoveLeftLimit;
    [SerializeField] RectTransform _gaugeMoveRightLimit;
    [SerializeField] float _gaugeSpeed;
    private float minX, maxX;

    [Header("Breakpoints")]
    [SerializeField] private float _startLow;
    [SerializeField] private float _startMid;
    [SerializeField] private float _startHigh;
    [SerializeField] private float _endHigh;
    [SerializeField] private float _endMid;
    [SerializeField] private float _endLow;

    // Events
    public event Action OnGaugeFull;

    //State
    private bool _movingright;
    private int _pointsNeededForBar = 10;
    private int _pointsProgress;
    private int PointsProgress
    {
        get { return _pointsProgress; }
        set
        {
            _pointsProgress = value;
            _progressBar.fillAmount = (float)_pointsProgress / (float)_pointsNeededForBar;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        minX = _gaugeMoveLeftLimit.anchoredPosition.x;
        maxX = _gaugeMoveRightLimit.anchoredPosition.x;
    }

    private void Awake()
    {
        _movingright = true;
        PointsProgress = 0;
    }

    private void Update()
    {
        MoveGauge();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int points = CheckPoints();
        UpdateProgress(points);
    }
    
    private void MoveGauge()
    {
        Vector2 movementDirection = new Vector2(1, 0);
        if (!_movingright)
        {
            movementDirection *= -1;
        }

        _movingGauge.rectTransform.anchoredPosition += movementDirection * _gaugeSpeed * Time.deltaTime;

        if (_movingGauge.rectTransform.anchoredPosition.x >= maxX && _movingright ||
            _movingGauge.rectTransform.anchoredPosition.x <= minX && !_movingright)
        {
            _movingright = !_movingright;
        }
    }

    private int CheckPoints()
    {

        float progress = (_movingGauge.rectTransform.anchoredPosition.x - minX) / (maxX - minX);
        print(progress);

        if (progress < _startLow || progress > _endLow)
        {
            return 0;
        }
        else if (progress < _startMid || progress > _endMid)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    private void UpdateProgress(int points)
    {
        print(points);

        PointsProgress += points;
        if (PointsProgress >= _pointsNeededForBar)
        {
            OnGaugeFull?.Invoke();
            PointsProgress = 0;
            _pointsNeededForBar = (int)(_pointsNeededForBar * 1.2);
        }
    }
}
