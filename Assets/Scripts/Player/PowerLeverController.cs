using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class PowerLeverController : MonoBehaviour
{
    [SerializeField] private PowerLever _lever;
    [SerializeField] private Image _powerLevelGauge;
    [SerializeField] private Gradient _powerLevelGaugeGradient;

    public event Action OnShieldGaugeFull;


    // Start is called before the first frame update
    void Start()
    {
        _lever.DragProgressProxyEvent += DragLeverHandler;
    }

    public void DragLeverHandler(float gaugeLevel)
    {
        _powerLevelGauge.color = _powerLevelGaugeGradient.Evaluate(gaugeLevel);

        if (gaugeLevel == 1)
        {
            OnShieldGaugeFull.Invoke();
        }
    }
}
