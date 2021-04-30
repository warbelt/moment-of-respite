using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


public class BulletRechargeSlot : MonoBehaviour, IDropHandler
{
    public event Action onRechargeSlotFilled;

    public void OnDrop(PointerEventData eventData)
    {
        onRechargeSlotFilled.Invoke();
    }

    
}
