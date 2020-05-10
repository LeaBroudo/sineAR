using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform imageTarget;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        gameObject.tag = "dragged";
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
    }
}
