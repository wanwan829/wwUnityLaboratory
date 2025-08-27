using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelfButton : Button
{
    public ButtonClickedEvent onSelfBtnEnter = new ButtonClickedEvent();
    public ButtonClickedEvent onSelfBtnDown = new ButtonClickedEvent();
    public ButtonClickedEvent onSelfBtnUp= new ButtonClickedEvent();
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        //Debug.Log("Pointer Enter ");
        onSelfBtnEnter?.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //Debug.Log("Pointer Down ");
        onSelfBtnDown?.Invoke();
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //Debug.Log("Pointer Up ");
        onSelfBtnUp?.Invoke();
    }
}
