using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class DraggableGraphic<T> : MonoBehaviour, IDragHandler, IEndDragHandler
{
    protected Image Icon;
    public bool isHeld;

    [HideInInspector]
    public RectTransform rectTransform;
    public T thisData;

    protected float fitTo;
    public bool isDragging = false;

    private void Awake()
    {
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        fitTo = Icon.rectTransform.sizeDelta.x;
    }

    public virtual void Create(T data) { }

    public void StartDragBasic()
    {
        transform.SetAsLastSibling();
        StartDrag();
    }
    public virtual void StartDrag() { }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            isDragging = true;
            StartDragBasic();
        }

        rectTransform.anchoredPosition += eventData.delta;
    }

    public virtual void OnEndDrag(PointerEventData eventData) { }

    public virtual void GetIcon() { }
}
