using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class HandDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("over hand!");
        if (eventData.pointerDrag == null)
        {
            return;
        }

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null)
        {
            d.overHand = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("over hand = false");
        if (eventData.pointerDrag == null)
        {
            return;
        }

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null)
        {
            d.overHand = false;
        }
    }
}
