using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class TableDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("over table!");
        if (eventData.pointerDrag == null)
        { 
            return;
        }

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null)
        {
            d.overTable = true;
        }
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
        if (d != null)
        {
            d.overTable = false;
        }
    }
    
}
